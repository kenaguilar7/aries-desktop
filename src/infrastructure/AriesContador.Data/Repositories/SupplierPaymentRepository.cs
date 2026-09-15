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
    public class SupplierPaymentRepository : ISupplierPaymentRepository
    {
        private readonly IConnectionString _connectionString;

        public SupplierPaymentRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<SupplierPayment> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PaymentRow, object>(
                SupplierPaymentQuery.SelectById, new { Id = id }, cancellationToken).ConfigureAwait(false);
            return Map(rows.FirstOrDefault());
        }

        public async Task<SupplierPayment> GetByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PaymentRow, object>(
                SupplierPaymentQuery.SelectByPurchase, new { PurchaseId = purchaseId }, cancellationToken)
                .ConfigureAwait(false);
            return Map(rows.FirstOrDefault());
        }

        public async Task<IEnumerable<SupplierPayment>> FindByCompanyIdAsync(
            string companyId,
            CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PaymentRow, object>(
                SupplierPaymentQuery.SelectByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
            return rows.Select(Map).Where(p => p != null).ToList();
        }

        public async Task AddAsync(SupplierPayment payment, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            payment.Id = await dataAccess.InsertAndGetIdAsync(
                SupplierPaymentQuery.Insert,
                new
                {
                    payment.CompanyId,
                    payment.SupplierId,
                    payment.PurchaseId,
                    payment.Amount,
                    payment.PaidAt,
                    PaymentMethodDb = PurchaseSettlementNames.ToDb(payment.PaymentMethod),
                    payment.PaymentReference,
                    payment.JournalEntryId,
                    payment.Notes,
                    payment.CreatedBy,
                    payment.UpdatedBy
                },
                cancellationToken).ConfigureAwait(false);
        }

        private static SupplierPayment Map(PaymentRow row)
        {
            if (row == null)
                return null;
            return new SupplierPayment
            {
                Id = row.Id,
                CompanyId = row.CompanyId,
                SupplierId = row.SupplierId,
                PurchaseId = row.PurchaseId,
                Amount = row.Amount,
                PaidAt = row.PaidAt,
                PaymentMethod = PurchaseSettlementNames.FromDb(row.PaymentMethodDb),
                PaymentReference = row.PaymentReference,
                JournalEntryId = row.JournalEntryId,
                Notes = row.Notes,
                SupplierName = row.SupplierName,
                DocumentNumber = row.DocumentNumber,
                CreatedAt = row.CreatedAt,
                UpdateAt = row.UpdateAt,
                CreatedBy = row.CreatedBy,
                UpdatedBy = row.UpdatedBy,
                Active = row.Active
            };
        }

        private sealed class PaymentRow
        {
            public int Id { get; set; }
            public string CompanyId { get; set; }
            public int SupplierId { get; set; }
            public int PurchaseId { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaidAt { get; set; }
            public string PaymentMethodDb { get; set; }
            public string PaymentReference { get; set; }
            public int JournalEntryId { get; set; }
            public string Notes { get; set; }
            public string SupplierName { get; set; }
            public string DocumentNumber { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public int CreatedBy { get; set; }
            public int UpdatedBy { get; set; }
            public bool Active { get; set; }
        }
    }
}
