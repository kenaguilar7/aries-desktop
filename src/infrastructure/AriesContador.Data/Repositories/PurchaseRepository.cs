using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IConnectionString _connectionString;

        public PurchaseRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public Task AddAsync(Purchase entity, CancellationToken cancellationToken = default) =>
            CreateWithEffectsAsync(entity, cancellationToken);

        public Task UpdateAsync(Purchase entity, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Las facturas de compra confirmadas no se editan");

        public Task RemoveAsync(Purchase entity, CancellationToken cancellationToken = default) =>
            CancelWithEffectsAsync(entity, cancellationToken);

        public async Task<Purchase> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PurchaseRow, object>(
                PurchasesQuery.SelectPurchaseById, new { Id = id }, cancellationToken).ConfigureAwait(false);
            var purchase = Map(rows.FirstOrDefault());
            if (purchase != null)
                purchase.Lines = await LoadLinesAsync(dataAccess, purchase.Id, cancellationToken).ConfigureAwait(false);
            return purchase;
        }

        public async Task<IEnumerable<Purchase>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PurchaseRow, object>(
                PurchasesQuery.SelectPurchasesByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
            var purchases = rows.Select(Map).Where(p => p != null).ToList();
            foreach (var purchase in purchases)
                purchase.Lines = await LoadLinesAsync(dataAccess, purchase.Id, cancellationToken).ConfigureAwait(false);
            return purchases;
        }

        public async Task<Purchase> FindByDocumentAsync(
            string companyId,
            int supplierId,
            string documentNumber,
            CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PurchaseRow, object>(
                PurchasesQuery.SelectPurchaseByDocument,
                new { CompanyId = companyId, SupplierId = supplierId, DocumentNumber = documentNumber },
                cancellationToken).ConfigureAwait(false);
            return Map(rows.FirstOrDefault());
        }

        public async Task CreateWithEffectsAsync(Purchase purchase, CancellationToken cancellationToken = default)
        {
            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    foreach (var line in purchase.Lines)
                    {
                        var unitCost = line.Quantity > 0
                            ? Math.Round(line.NetAmount / line.Quantity, 2, MidpointRounding.AwayFromZero)
                            : line.NetAmount;
                        var affected = await dataAccess.ExecuteTextInTransactionAsync(
                            PurchasesQuery.IncrementStockAndCost,
                            new
                            {
                                Quantity = line.Quantity,
                                Cost = unitCost,
                                line.ProductId,
                                purchase.CompanyId,
                                purchase.UpdatedBy
                            },
                            cancellationToken).ConfigureAwait(false);
                        if (affected == 0)
                            throw new InvalidOperationException("Producto no encontrado: " + line.ProductName);
                    }

                    purchase.Id = await dataAccess.InsertAndGetIdInTransactionAsync(
                        PurchasesQuery.InsertPurchase,
                        new
                        {
                            purchase.CompanyId,
                            purchase.SupplierId,
                            purchase.DocumentNumber,
                            purchase.PurchasedAt,
                            PaymentMethodDb = PurchaseSettlementNames.ToDb(purchase.PaymentMethod),
                            purchase.PaymentReference,
                            purchase.Total,
                            purchase.NetAmount,
                            purchase.TaxAmount,
                            purchase.Notes,
                            StatusDb = PurchaseStatusNames.ToDb(purchase.Status),
                            purchase.CreatedBy,
                            purchase.UpdatedBy
                        },
                        cancellationToken).ConfigureAwait(false);

                    foreach (var line in purchase.Lines)
                    {
                        line.PurchaseId = purchase.Id;
                        line.CreatedBy = purchase.CreatedBy;
                        line.UpdatedBy = purchase.UpdatedBy;
                        line.Id = await dataAccess.InsertAndGetIdInTransactionAsync(
                            PurchasesQuery.InsertPurchaseLine, line, cancellationToken).ConfigureAwait(false);
                    }

                    await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
        }

        public async Task CancelWithEffectsAsync(Purchase purchase, CancellationToken cancellationToken = default)
        {
            if (purchase == null)
                throw new InvalidOperationException("La compra es requerida");
            if (purchase.Lines == null)
                purchase.Lines = new List<PurchaseLine>();

            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    foreach (var line in purchase.Lines)
                    {
                        var affected = await dataAccess.ExecuteTextInTransactionAsync(
                            PurchasesQuery.DecrementStockOnly,
                            new
                            {
                                Quantity = line.Quantity,
                                line.ProductId,
                                purchase.CompanyId,
                                purchase.UpdatedBy
                            },
                            cancellationToken).ConfigureAwait(false);
                        if (affected == 0)
                            throw new InvalidOperationException(
                                "No se puede anular: stock insuficiente de " + line.ProductName
                                + " (posiblemente ya se vendió)");
                    }

                    var deactivated = await dataAccess.ExecuteTextInTransactionAsync(
                        PurchasesQuery.DeactivatePurchase,
                        new
                        {
                            purchase.Id,
                            StatusDb = PurchaseStatusNames.ToDb(PurchaseStatus.Cancelled),
                            purchase.UpdatedBy
                        },
                        cancellationToken).ConfigureAwait(false);
                    if (deactivated == 0)
                        throw new InvalidOperationException("Compra no encontrada o ya anulada");

                    await dataAccess.ExecuteTextInTransactionAsync(
                        PurchasesQuery.DeactivatePurchaseLines,
                        new { PurchaseId = purchase.Id, purchase.UpdatedBy },
                        cancellationToken).ConfigureAwait(false);

                    purchase.Active = false;
                    purchase.Status = PurchaseStatus.Cancelled;
                    if (purchase.Lines != null)
                    {
                        foreach (var line in purchase.Lines)
                            line.Active = false;
                    }

                    await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
        }

        private static async Task<List<PurchaseLine>> LoadLinesAsync(
            MySqlDataAccess dataAccess,
            int purchaseId,
            CancellationToken cancellationToken)
        {
            return await dataAccess.ExecuteQueryAsync<PurchaseLine, object>(
                PurchasesQuery.SelectPurchaseLines, new { PurchaseId = purchaseId }, cancellationToken)
                .ConfigureAwait(false);
        }

        private static Purchase Map(PurchaseRow row)
        {
            if (row == null)
                return null;
            return new Purchase
            {
                Id = row.Id,
                CompanyId = row.CompanyId,
                SupplierId = row.SupplierId,
                DocumentNumber = row.DocumentNumber,
                PurchasedAt = row.PurchasedAt,
                PaymentMethod = PurchaseSettlementNames.FromDb(row.PaymentMethodDb),
                PaymentReference = row.PaymentReference,
                Total = row.Total,
                NetAmount = row.NetAmount,
                TaxAmount = row.TaxAmount,
                Notes = row.Notes,
                Status = PurchaseStatusNames.FromDb(row.StatusDb),
                SupplierName = row.SupplierName,
                CreatedAt = row.CreatedAt,
                UpdateAt = row.UpdateAt,
                CreatedBy = row.CreatedBy,
                UpdatedBy = row.UpdatedBy,
                Active = row.Active
            };
        }

        private sealed class PurchaseRow
        {
            public int Id { get; set; }
            public string CompanyId { get; set; }
            public int SupplierId { get; set; }
            public string DocumentNumber { get; set; }
            public DateTime PurchasedAt { get; set; }
            public string PaymentMethodDb { get; set; }
            public string PaymentReference { get; set; }
            public decimal Total { get; set; }
            public decimal NetAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public string Notes { get; set; }
            public string StatusDb { get; set; }
            public string SupplierName { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public int CreatedBy { get; set; }
            public int UpdatedBy { get; set; }
            public bool Active { get; set; }
        }
    }
}
