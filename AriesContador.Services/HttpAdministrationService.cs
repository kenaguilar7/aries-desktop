using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Services
{
    public interface IHttpAdministrationService
    {
        Task<List<Company>> GetAllCompanies();
        Task<WebToken> Login(Login param);
    }

    public class HttpAdministrationService : IHttpAdministrationService
    {
        private readonly IHttpClientService _httpClientService;

        public HttpAdministrationService(IHttpClientService httpClientService)
        {
            this._httpClientService = httpClientService;
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
