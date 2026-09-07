using AriesContador.Core.Models.Users;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
