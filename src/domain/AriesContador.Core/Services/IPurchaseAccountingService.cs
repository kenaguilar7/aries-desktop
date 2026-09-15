using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Services
{
    public interface IPurchaseAccountingService
    {
        Task<PurchaseAccountMap> GetAccountMapAsync(string companyId, CancellationToken cancellationToken = default);
        Task SaveAccountMapAsync(PurchaseAccountMap map, CancellationToken cancellationToken = default);
        Task<PurchaseAccountMap> EnsureSuggestedAccountsAsync(string companyId, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Purchase>> GetUnpostedPurchasesAsync(string companyId, CancellationToken cancellationToken = default);
        Task<PurchasePostingPreview> PreviewPurchaseAsync(int purchaseId, CancellationToken cancellationToken = default);
        Task<PurchasePostingPreview> PostPurchaseAsync(int purchaseId, int userId, CancellationToken cancellationToken = default);
    }
}
