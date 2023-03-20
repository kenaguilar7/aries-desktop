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
        string GetCompanyConsecutive(); 
        void CreateCompany(Company compañia);
        void CreateUser(User usuario);
        Task<IEnumerable<Company>> GetAllCompanies();
        Company FindByCode(string code); 
        IEnumerable<User> GetAllUsers();
        IEnumerable<Company> GetAllInactiveCompanies();
        IEnumerable<User> GetAllInactiveUsers();
        User FinUserById(int id); 
        void InactivateCompany(Company compania);
        void InactivateUser(User usuario);
        void UpdateCompany(Company compania);
        void UpdateUser(User usuario);
        Task<WebToken> Login(Login param);
    }
}
