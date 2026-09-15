using AriesContador.Core.Models.Purchases;

namespace Aries.Contabilidad.Services
{
    public interface IPurchaseAccountingClient
    {
        Task<PurchaseAccountMap> GetAccountMapAsync(string companyId);
        Task SaveAccountMapAsync(PurchaseAccountMap map);
        Task<PurchaseAccountMap> EnsureSuggestedAccountsAsync(string companyId);
        Task<List<Purchase>> GetUnpostedPurchasesAsync(string companyId);
        Task<PurchasePostingPreview> PreviewPurchaseAsync(int purchaseId);
        Task<PurchasePostingPreview> PostPurchaseAsync(int purchaseId);
    }
}
