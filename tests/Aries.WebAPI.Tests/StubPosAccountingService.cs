using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubPosAccountingService : IPosAccountingService
    {
        public Task<PosAccountMap> GetAccountMapAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PosAccountMap { CompanyId = companyId });

        public Task SaveAccountMapAsync(PosAccountMap map, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<PosAccountMap> EnsureSuggestedAccountsAsync(string companyId, int userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PosAccountMap { CompanyId = companyId });

        public Task<IEnumerable<SalesRegisterSession>> GetUnpostedSessionsAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<SalesRegisterSession>>(new List<SalesRegisterSession>());

        public Task<PosPostingPreview> PreviewSessionAsync(int sessionId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PosPostingPreview());

        public Task<PosPostingPreview> PostSessionAsync(int sessionId, int userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PosPostingPreview { JournalEntryId = 1, AlreadyPosted = true });

        public Task<IEnumerable<PosSessionReconciliationRow>> GetReconciliationAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<PosSessionReconciliationRow>>(new List<PosSessionReconciliationRow>());
    }
}
