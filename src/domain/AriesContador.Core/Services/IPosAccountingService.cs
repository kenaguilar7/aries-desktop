using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Services
{
    public interface IPosAccountingService
    {
        Task<PosAccountMap> GetAccountMapAsync(string companyId, CancellationToken cancellationToken = default);
        Task SaveAccountMapAsync(PosAccountMap map, CancellationToken cancellationToken = default);
        Task<PosAccountMap> EnsureSuggestedAccountsAsync(string companyId, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesRegisterSession>> GetUnpostedSessionsAsync(string companyId, CancellationToken cancellationToken = default);
        Task<PosPostingPreview> PreviewSessionAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<PosPostingPreview> PostSessionAsync(int sessionId, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PosSessionReconciliationRow>> GetReconciliationAsync(string companyId, CancellationToken cancellationToken = default);
    }
}
