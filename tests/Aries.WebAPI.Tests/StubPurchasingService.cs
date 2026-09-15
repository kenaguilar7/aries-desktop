using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubPurchasingService : IPurchasingService
    {
        public List<Supplier> Items { get; } = new List<Supplier>();
        public List<Purchase> Purchases { get; } = new List<Purchase>();
        public int? LastCancelledPurchaseId { get; private set; }
        public int LastCancelledUserId { get; private set; }

        public Task<IEnumerable<Supplier>> GetSuppliersAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Supplier>>(Items.Where(s => s.CompanyId == companyId && s.Active).ToList());

        public Task<Supplier> FindSupplierAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(s => s.Id == id));

        public Task CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            supplier.Id = Items.Count + 1;
            supplier.Active = true;
            Items.Add(supplier);
            return Task.CompletedTask;
        }

        public Task UpdateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteSupplierAsync(int id, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IEnumerable<Purchase>> GetPurchasesAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Purchase>>(Purchases.Where(p => p.CompanyId == companyId && p.Active).ToList());

        public Task<Purchase> FindPurchaseAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Purchases.FirstOrDefault(p => p.Id == id));

        public Task<Purchase> ConfirmPurchaseAsync(Purchase purchase, CancellationToken cancellationToken = default)
        {
            purchase.Id = Purchases.Count + 1;
            purchase.Status = PurchaseStatus.Confirmed;
            purchase.Active = true;
            Purchases.Add(purchase);
            return Task.FromResult(purchase);
        }

        public Task CancelPurchaseAsync(int purchaseId, int userId, CancellationToken cancellationToken = default)
        {
            LastCancelledPurchaseId = purchaseId;
            LastCancelledUserId = userId;
            var purchase = Purchases.FirstOrDefault(p => p.Id == purchaseId);
            if (purchase != null)
            {
                purchase.Active = false;
                purchase.Status = PurchaseStatus.Cancelled;
            }
            return Task.CompletedTask;
        }
    }
}
