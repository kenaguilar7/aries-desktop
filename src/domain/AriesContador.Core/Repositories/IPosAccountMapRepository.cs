using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Repositories
{
    public interface IPosAccountMapRepository
    {
        Task<PosAccountMap> GetByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task UpsertAsync(PosAccountMap map, CancellationToken cancellationToken = default);
    }
}
