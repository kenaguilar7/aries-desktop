using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Repositories
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<Sale> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> FindBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> FindByCompanyAndDateRangeAsync(string companyId, DateTime fromInclusive, DateTime toExclusive, CancellationToken cancellationToken = default);
        Task CreateWithEffectsAsync(Sale sale, CancellationToken cancellationToken = default);
    }
}
