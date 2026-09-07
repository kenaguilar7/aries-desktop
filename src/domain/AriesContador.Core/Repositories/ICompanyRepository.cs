using AriesContador.Core.Models.Companies;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<string> LatestCodeAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetCodesAllowedForUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
