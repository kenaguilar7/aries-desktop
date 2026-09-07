using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Services
{
    public interface IAdministrationService
    {
        Task<string> GetCompanyConsecutiveAsync(CancellationToken cancellationToken = default);
        Task CreateCompanyAsync(Company company, CancellationToken cancellationToken = default);
        Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<IEnumerable<Company>> GetAllCompaniesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Company>> GetAllCompaniesAsync(User currentUser, CancellationToken cancellationToken = default);
        Task DeleteCompanyAsync(Company company, CancellationToken cancellationToken = default);
        Task<Company> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Company>> GetAllInactiveCompaniesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllInactiveUsersAsync(CancellationToken cancellationToken = default);
        Task<User> FinUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task InactivateUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateCompanyAsync(Company company, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<WebToken> LoginAsync(Login param, CancellationToken cancellationToken = default);
        Task<bool> UserNameTakenAsync(string userName, CancellationToken cancellationToken = default);
    }
}
