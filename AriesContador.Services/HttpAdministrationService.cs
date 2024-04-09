using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Company = AriesContador.Core.Models.Companies.Company; 

namespace AriesContador.Services
{
    public interface IHttpAdministrationService
    {
        Task<List<Company>> GetAllCompanies();
        Task<WebToken> Login(Login param);

        Task DeleteCompany(Company company);
        Task<Company> BuildNewCompanyCode();
    }

    public class HttpAdministrationService : IHttpAdministrationService
    {
        private readonly IHttpClientService _httpClientService;

        public HttpAdministrationService(IHttpClientService httpClientService)
        {
            this._httpClientService = httpClientService;
        }

        public async Task<Company> BuildNewCompanyCode()
        {
            return await _httpClientService.GetAsync<Company>(string.Concat(EnvironmentVariable.ApiUrl, $"company/BuildCode"));
        }

        public async Task DeleteCompany(Company company)
        {
            await _httpClientService.DeleteAsync(string.Concat(EnvironmentVariable.ApiUrl, $"company/delete/{company.Code}"));
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            var response = await _httpClientService.GetAsync<List<Company>>(string.Concat(EnvironmentVariable.ApiUrl, "company/getAll"));
            return response;
        }

        public async Task<WebToken> Login(Login param)
        {
            var response = await _httpClientService.PostAsync<WebToken, Login>(string.Concat(EnvironmentVariable.ApiUrl, "auth/login"), param);
            return response;
        }

    }
}
