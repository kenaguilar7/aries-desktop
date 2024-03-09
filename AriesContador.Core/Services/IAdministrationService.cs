using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Services
{
    public interface IAdministrationService
    {
        Task<string> GetCompanyConsecutive(); 
        Task CreateCompany(Company company);
        void CreateUser(User user);
        Task<IEnumerable<Company>> GetAllCompanies();
        Task DeleteCompany(Company company); 
        Company FindByCode(string code); 
        IEnumerable<User> GetAllUsers();
        IEnumerable<Company> GetAllInactiveCompanies();
        IEnumerable<User> GetAllInactiveUsers();
        User FinUserById(int id); 
        void InactivateUser(User user);
        void UpdateCompany(Company company);
        void UpdateUser(User user);
    }
}
