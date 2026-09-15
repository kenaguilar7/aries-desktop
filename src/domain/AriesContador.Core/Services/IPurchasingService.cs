using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Services
{
    public interface IPurchasingService
    {
        Task<IEnumerable<Supplier>> GetSuppliersAsync(string companyId, CancellationToken cancellationToken = default);
        Task<Supplier> FindSupplierAsync(int id, CancellationToken cancellationToken = default);
        Task CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task UpdateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task DeleteSupplierAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Purchase>> GetPurchasesAsync(string companyId, CancellationToken cancellationToken = default);
        Task<Purchase> FindPurchaseAsync(int id, CancellationToken cancellationToken = default);
        Task<Purchase> ConfirmPurchaseAsync(Purchase purchase, CancellationToken cancellationToken = default);
    }
}
