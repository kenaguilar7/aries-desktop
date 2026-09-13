using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Repositories
{
    public interface ISalesRegisterRepository : IRepository<SalesRegister>
    {
        Task<SalesRegister> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesRegister>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task<SalesRegisterSession> GetOpenSessionAsync(int registerId, CancellationToken cancellationToken = default);
        Task<SalesRegisterSession> GetSessionByIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task AddSessionAsync(SalesRegisterSession session, CancellationToken cancellationToken = default);
        Task CloseSessionAsync(SalesRegisterSession session, CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesRegisterSession>> GetSessionHistoryAsync(string companyId, CancellationToken cancellationToken = default);
        Task UpdateSessionTotalsAsync(SalesRegisterSession session, CancellationToken cancellationToken = default);
    }
}
