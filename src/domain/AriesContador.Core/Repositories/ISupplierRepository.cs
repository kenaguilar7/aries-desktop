using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Repositories
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Supplier>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task<Supplier> FindByNumberIdAsync(string companyId, string numberId, CancellationToken cancellationToken = default);
    }
}
