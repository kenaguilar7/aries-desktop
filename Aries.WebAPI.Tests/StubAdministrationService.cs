using System.Collections.Generic;
using System.Linq;
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

        public WebToken Login(Login param)
        {
            if (param != null && param.UserId == "kenneth" && param.Password == "96321")
            {
                return new WebToken
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
                };
            }

            return new WebToken();
        }

        public Task<string> GetCompanyConsecutive() => Task.FromResult("C002");

        public Task CreateCompany(Company company)
        {
            LastCreated = company;
            return Task.CompletedTask;
        }

        public void CreateUser(User user) { }

        public Task<IEnumerable<Company>> GetAllCompanies() =>
            Task.FromResult<IEnumerable<Company>>(new[]
            {
                new Company { Code = "C001", CompanyName = "Demo", Active = true }
            });

        public Task<IEnumerable<Company>> GetAllCompanies(User currentUser) => GetAllCompanies();

        public Task DeleteCompany(Company company)
        {
            LastDeletedCode = company.Code;
            return Task.CompletedTask;
        }

        public Company FindByCode(string code) =>
            GetAllCompanies().GetAwaiter().GetResult().FirstOrDefault(c => c.Code == code);

        public IEnumerable<User> GetAllUsers() =>
            new[]
            {
                new User { Id = 7, UserName = "kenneth", Active = true, UserType = UserType.Administrador }
            };

        public IEnumerable<Company> GetAllInactiveCompanies() => Enumerable.Empty<Company>();

        public IEnumerable<User> GetAllInactiveUsers() => Enumerable.Empty<User>();

        public User FinUserById(int id) => GetAllUsers().FirstOrDefault(u => u.Id == id);

        public void InactivateUser(User user) { }

        public void UpdateCompany(Company company) { }

        public void UpdateUser(User user) { }

        public bool UserNameTaken(string userName) => false;
    }
}
