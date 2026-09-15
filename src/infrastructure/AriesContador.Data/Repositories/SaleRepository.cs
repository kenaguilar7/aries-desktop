using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;
using Dapper;

namespace AriesContador.Data.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly IConnectionString _connectionString;

        public SaleRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public Task AddAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            return CreateWithEffectsAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Las ventas POS no se editan");
        }

        public Task RemoveAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Las ventas POS no se eliminan de forma individual");
        }

        public async Task<Sale> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Sale, object>(
                PosQuery.SelectSaleById, new { Id = id }, cancellationToken).ConfigureAwait(false);
            var sale = rows.FirstOrDefault();
            if (sale != null)
                sale.Lines = await LoadLinesAsync(dataAccess, sale.Id, cancellationToken).ConfigureAwait(false);
            return sale;
        }

        public async Task<IEnumerable<Sale>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var sales = await dataAccess.ExecuteQueryAsync<Sale, object>(
                PosQuery.SelectSalesByCompany, new { CompanyId = companyId }, cancellationToken).ConfigureAwait(false);
            await AttachLinesAsync(dataAccess, sales, cancellationToken).ConfigureAwait(false);
            return sales;
        }

        public async Task<IEnumerable<Sale>> FindBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var sales = await dataAccess.ExecuteQueryAsync<Sale, object>(
                PosQuery.SelectSalesBySession, new { SessionId = sessionId }, cancellationToken).ConfigureAwait(false);
            await AttachLinesAsync(dataAccess, sales, cancellationToken).ConfigureAwait(false);
            return sales;
        }

        public async Task<IEnumerable<Sale>> FindByCompanyAndDateRangeAsync(
            string companyId,
            DateTime fromInclusive,
            DateTime toExclusive,
            CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<Sale, object>(
                PosQuery.SelectSalesByDateRange,
                new { CompanyId = companyId, FromInclusive = fromInclusive, ToExclusive = toExclusive },
                cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateWithEffectsAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var locked = await dataAccess.ExecuteQueryInTransactionAsync<int>(
                        PosQuery.LockOpenSession,
                        new { sale.SalesRegisterId },
                        cancellationToken).ConfigureAwait(false);
                    if (locked.Count == 0 || locked[0] != sale.SessionId)
                        throw new InvalidOperationException("La caja no tiene una sesión abierta");

                    await DecrementStockAsync(dataAccess, sale, cancellationToken).ConfigureAwait(false);

                    sale.Id = await dataAccess.InsertAndGetIdInTransactionAsync(
                        PosQuery.InsertSale,
                        new
                        {
                            sale.CompanyId,
                            sale.SalesRegisterId,
                            sale.SessionId,
                            PaymentMethodDb = PaymentMethodNames.ToDb(sale.PaymentMethod),
                            sale.PaymentReference,
                            sale.Total,
                            sale.NetAmount,
                            sale.TaxAmount,
                            sale.CostAmount,
                            sale.SoldAt,
                            sale.CreatedBy,
                            sale.UpdatedBy
                        },
                        cancellationToken).ConfigureAwait(false);

                    await InsertLinesAsync(dataAccess, sale, cancellationToken).ConfigureAwait(false);

                    await dataAccess.ExecuteTextInTransactionAsync(@"
UPDATE sales_register_sessions SET
  cash_sales = cash_sales + @Cash,
  card_sales = card_sales + @Card,
  transfer_sales = transfer_sales + @Transfer,
  expected_closing_amount = opening_amount + cash_sales + @Cash,
  updated_by = @UpdatedBy
WHERE session_id = @Id AND closed_at IS NULL",
                        new
                        {
                            Cash = sale.PaymentMethod == PaymentMethod.Efectivo ? sale.Total : 0m,
                            Card = sale.PaymentMethod == PaymentMethod.Tarjeta ? sale.Total : 0m,
                            Transfer = sale.PaymentMethod == PaymentMethod.Transferencia ? sale.Total : 0m,
                            sale.UpdatedBy,
                            Id = sale.SessionId
                        },
                        cancellationToken).ConfigureAwait(false);

                    await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
        }

        private static async Task DecrementStockAsync(
            MySqlDataAccess dataAccess,
            Sale sale,
            CancellationToken cancellationToken)
        {
            var decrements = sale.Lines
                .GroupBy(line => line.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(line => line.StockToDecrement),
                    ProductName = group.First().ProductName
                })
                .ToList();

            if (decrements.Count == 0)
                return;

            if (decrements.Count == 1)
            {
                var only = decrements[0];
                var affected = await dataAccess.ExecuteTextInTransactionAsync(
                    PosQuery.DecrementStock,
                    new
                    {
                        only.Quantity,
                        only.ProductId,
                        sale.CompanyId,
                        sale.UpdatedBy
                    },
                    cancellationToken).ConfigureAwait(false);
                if (affected == 0)
                    throw new InvalidOperationException("Stock insuficiente para " + only.ProductName);
                return;
            }

            var sql = new StringBuilder();
            var args = new DynamicParameters();
            sql.Append("UPDATE products SET stock = stock - CASE product_id");
            for (var i = 0; i < decrements.Count; i++)
            {
                sql.Append(" WHEN @P").Append(i).Append(" THEN @Q").Append(i);
                args.Add("P" + i, decrements[i].ProductId);
                args.Add("Q" + i, decrements[i].Quantity);
            }
            sql.Append(" END, updated_by = @UpdatedBy WHERE company_id = @CompanyId AND active = 1 AND product_id IN (");
            for (var i = 0; i < decrements.Count; i++)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append("@P").Append(i);
            }
            sql.Append(") AND stock >= CASE product_id");
            for (var i = 0; i < decrements.Count; i++)
                sql.Append(" WHEN @P").Append(i).Append(" THEN @Q").Append(i);
            sql.Append(" END");
            args.Add("UpdatedBy", sale.UpdatedBy);
            args.Add("CompanyId", sale.CompanyId);

            var updated = await dataAccess.ExecuteTextInTransactionAsync(sql.ToString(), args, cancellationToken)
                .ConfigureAwait(false);
            if (updated != decrements.Count)
            {
                var name = decrements.Select(d => d.ProductName).FirstOrDefault(n => !string.IsNullOrEmpty(n));
                throw new InvalidOperationException("Stock insuficiente para " + (name ?? "el producto"));
            }
        }

        private static async Task InsertLinesAsync(
            MySqlDataAccess dataAccess,
            Sale sale,
            CancellationToken cancellationToken)
        {
            if (sale.Lines == null || sale.Lines.Count == 0)
                return;

            var sql = new StringBuilder();
            sql.Append(@"INSERT INTO sale_lines
  (sale_id, product_id, product_name, quantity, unit_price, sold_by_weight, price_per_kilo, weight_grams, line_total, net_amount, tax_amount, cost_amount, tax_exempt, created_by, updated_by, active)
VALUES ");
            var args = new DynamicParameters();
            args.Add("SaleId", sale.Id);
            args.Add("CreatedBy", sale.CreatedBy);
            args.Add("UpdatedBy", sale.UpdatedBy);
            for (var i = 0; i < sale.Lines.Count; i++)
            {
                var line = sale.Lines[i];
                line.SaleId = sale.Id;
                line.CreatedBy = sale.CreatedBy;
                line.UpdatedBy = sale.UpdatedBy;
                if (i > 0)
                    sql.Append(", ");
                sql.Append("(@SaleId, @ProductId").Append(i)
                    .Append(", @ProductName").Append(i)
                    .Append(", @Quantity").Append(i)
                    .Append(", @UnitPrice").Append(i)
                    .Append(", @SoldByWeight").Append(i)
                    .Append(", @PricePerKilo").Append(i)
                    .Append(", @WeightGrams").Append(i)
                    .Append(", @LineTotal").Append(i)
                    .Append(", @NetAmount").Append(i)
                    .Append(", @TaxAmount").Append(i)
                    .Append(", @CostAmount").Append(i)
                    .Append(", @TaxExempt").Append(i)
                    .Append(", @CreatedBy, @UpdatedBy, 1)");
                args.Add("ProductId" + i, line.ProductId);
                args.Add("ProductName" + i, line.ProductName);
                args.Add("Quantity" + i, line.Quantity);
                args.Add("UnitPrice" + i, line.UnitPrice);
                args.Add("SoldByWeight" + i, line.SoldByWeight);
                args.Add("PricePerKilo" + i, line.PricePerKilo);
                args.Add("WeightGrams" + i, line.WeightGrams);
                args.Add("LineTotal" + i, line.LineTotal);
                args.Add("NetAmount" + i, line.NetAmount);
                args.Add("TaxAmount" + i, line.TaxAmount);
                args.Add("CostAmount" + i, line.CostAmount);
                args.Add("TaxExempt" + i, line.TaxExempt);
            }

            await dataAccess.ExecuteTextInTransactionAsync(sql.ToString(), args, cancellationToken)
                .ConfigureAwait(false);

            var ids = await dataAccess.ExecuteQueryInTransactionAsync<int>(
                PosQuery.SelectInsertedSaleLineIds,
                new { SaleId = sale.Id },
                cancellationToken).ConfigureAwait(false);
            for (var i = 0; i < sale.Lines.Count && i < ids.Count; i++)
                sale.Lines[i].Id = ids[i];
        }

        private static async Task AttachLinesAsync(
            MySqlDataAccess dataAccess,
            List<Sale> sales,
            CancellationToken cancellationToken)
        {
            if (sales == null || sales.Count == 0)
                return;

            foreach (var sale in sales)
                sale.Lines = new List<SaleLine>();

            var ids = sales.Select(s => s.Id).Distinct().ToList();
            var lines = await dataAccess.ExecuteQueryAsync<SaleLine, object>(
                PosQuery.SelectSaleLinesBySaleIds, new { SaleIds = ids }, cancellationToken).ConfigureAwait(false);
            var bySale = lines.ToLookup(line => line.SaleId);
            foreach (var sale in sales)
                sale.Lines = bySale[sale.Id].ToList();
        }

        private static async Task<List<SaleLine>> LoadLinesAsync(
            MySqlDataAccess dataAccess,
            int saleId,
            CancellationToken cancellationToken)
        {
            return await dataAccess.ExecuteQueryAsync<SaleLine, object>(
                PosQuery.SelectSaleLines, new { SaleId = saleId }, cancellationToken).ConfigureAwait(false);
        }
    }
}
