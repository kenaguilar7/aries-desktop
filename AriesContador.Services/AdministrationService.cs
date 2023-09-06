using AriesContador.Core;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AriesContador.Services
{   
    public class AdministrationService : IAdministrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdministrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateCompany(Company compañia)
        {
            _unitOfWork.CompanyRepository.Add(compañia);
        }

        public void CreateUser(User usuario)
        {
            _unitOfWork.UserRepository.Add(usuario);
        }

        public Task DeleteCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public Company FindByCode(string code)
        {
            throw new NotImplementedException();
        }

        public User FinUserById(int id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            return user;
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            var output = await _unitOfWork.CompanyRepository.GetAll();
            return output;
        }

        public IEnumerable<Company> GetAllInactiveCompanies()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllInactiveUsers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllUsers()
        {
            var output = _unitOfWork.UserRepository.GetAll();
            return output;
        }

        public void InactivateUser(User usuario)
        {
            throw new NotImplementedException();
        }

        public void UpdateCompany(Company compania)
        {
            _unitOfWork.CompanyRepository.Update(compania);
        }

        public void UpdateUser(User usuario)
        {
            _unitOfWork.UserRepository.Update(usuario);
        }

        Task<string> IAdministrationService.GetCompanyConsecutive()
        {
            throw new NotImplementedException();
        }

    }



}
