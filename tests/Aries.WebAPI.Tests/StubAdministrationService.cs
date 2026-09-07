using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubAdministrationService : IAdministrationService
    {
        public Company LastCreated { get; private set; }
        public string LastDeletedCode { get; private set; }

        public Task<WebToken> LoginAsync(Login param, CancellationToken cancellationToken = default)
        {
            if (param != null && param.UserId == "kenneth" && param.Password == "96321")
            {
                return Task.FromResult(new WebToken
                {
                    Token = "local",
                    User = new User
                    {
                        Id = 7,
                        UserName = "kenneth",
                        Name = "K",
                        Active = true,
                        UserType = UserType.Administrador
                    }
                });
            }

            return Task.FromResult(new WebToken());
        }

        public Task<string> GetCompanyConsecutiveAsync(CancellationToken cancellationToken = default) => Task.FromResult("C002");

        public Task CreateCompanyAsync(Company company, CancellationToken cancellationToken = default)
        {
            LastCreated = company;
            return Task.CompletedTask;
        }

        public Task CreateUserAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Company>> GetAllCompaniesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Company>>(new[]
            {
                new Company { Code = "C001", CompanyName = "Demo", Active = true }
            });

        public Task<IEnumerable<Company>> GetAllCompaniesAsync(User currentUser, CancellationToken cancellationToken = default) =>
            GetAllCompaniesAsync(cancellationToken);

        public Task DeleteCompanyAsync(Company company, CancellationToken cancellationToken = default)
        {
            LastDeletedCode = company.Code;
            return Task.CompletedTask;
        }

        public async Task<Company> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var all = await GetAllCompaniesAsync(cancellationToken);
            return all.FirstOrDefault(c => c.Code == code);
        }

        public Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<User>>(new[]
            {
                new User { Id = 7, UserName = "kenneth", Active = true, UserType = UserType.Administrador }
            });

        public Task<IEnumerable<Company>> GetAllInactiveCompaniesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<Company>());

        public Task<IEnumerable<User>> GetAllInactiveUsersAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<User>());

        public async Task<User> FinUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var users = await GetAllUsersAsync(cancellationToken);
            return users.FirstOrDefault(u => u.Id == id);
        }

        public Task InactivateUserAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateCompanyAsync(Company company, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateUserAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> UserNameTakenAsync(string userName, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }
}
