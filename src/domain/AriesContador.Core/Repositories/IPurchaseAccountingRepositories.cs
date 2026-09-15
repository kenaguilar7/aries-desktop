using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Repositories
{
    public interface IPurchaseAccountMapRepository
    {
        Task<PurchaseAccountMap> GetByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task UpsertAsync(PurchaseAccountMap map, CancellationToken cancellationToken = default);
    }

    public interface IPurchasePostingRepository
    {
        Task<PurchasePosting> GetByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PurchasePosting>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task AddAsync(PurchasePosting posting, CancellationToken cancellationToken = default);
    }
}
