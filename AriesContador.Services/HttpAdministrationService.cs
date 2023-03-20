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
        public async Task<List<Company>> GetAllCompanies()
        {
            var response = await HttpClientService.GetAsync<List<Company>>(string.Concat(EnvironmentVariable.ApiUrl, "company/getAll"));
            return response;
        }

        public async Task<WebToken> Login(Login param)
        {
            var response = await HttpClientService.GetAsync<WebToken, Login>(string.Concat(EnvironmentVariable.ApiUrl, "auth/login"), param);
            return response;
        }
    }
}
