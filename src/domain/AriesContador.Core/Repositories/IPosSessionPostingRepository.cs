using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Repositories
{
    public interface IPosSessionPostingRepository
    {
        Task<PosSessionPosting> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PosSessionPosting>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task AddAsync(PosSessionPosting posting, CancellationToken cancellationToken = default);
    }
}
