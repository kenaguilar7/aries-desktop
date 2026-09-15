using AriesContador.Core.Models.Purchases;

namespace Aries.Contabilidad.Services
{
    public interface IPurchasingClient
    {
        Task<List<Supplier>> GetAllAsync(string companyId);
        Task<Supplier?> FindAsync(int id);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);

        Task<List<Purchase>> GetPurchasesAsync(string companyId);
        Task<Purchase?> FindPurchaseAsync(int id);
        Task<Purchase> ConfirmPurchaseAsync(Purchase purchase);
        Task CancelPurchaseAsync(int id);
    }
}
