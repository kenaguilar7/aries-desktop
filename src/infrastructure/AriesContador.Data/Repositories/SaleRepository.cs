using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

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
            var sales = await dataAccess.ExecuteQueryAsync<Sale, object>(
                PosQuery.SelectSalesByDateRange,
                new { CompanyId = companyId, FromInclusive = fromInclusive, ToExclusive = toExclusive },
                cancellationToken).ConfigureAwait(false);
            await AttachLinesAsync(dataAccess, sales, cancellationToken).ConfigureAwait(false);
            return sales;
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

                    foreach (var line in sale.Lines)
                    {
                        var affected = await dataAccess.ExecuteTextInTransactionAsync(
                            PosQuery.DecrementStock,
                            new
                            {
                                Quantity = line.StockToDecrement,
                                line.ProductId,
                                sale.CompanyId,
                                sale.UpdatedBy
                            },
                            cancellationToken).ConfigureAwait(false);
                        if (affected == 0)
                            throw new InvalidOperationException("Stock insuficiente para " + line.ProductName);
                    }

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

                    foreach (var line in sale.Lines)
                    {
                        line.SaleId = sale.Id;
                        line.CreatedBy = sale.CreatedBy;
                        line.UpdatedBy = sale.UpdatedBy;
                        line.Id = await dataAccess.InsertAndGetIdInTransactionAsync(
                            PosQuery.InsertSaleLine, line, cancellationToken).ConfigureAwait(false);
                    }

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

        private static async Task AttachLinesAsync(
            MySqlDataAccess dataAccess,
            List<Sale> sales,
            CancellationToken cancellationToken)
        {
            foreach (var sale in sales)
                sale.Lines = await LoadLinesAsync(dataAccess, sale.Id, cancellationToken).ConfigureAwait(false);
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
