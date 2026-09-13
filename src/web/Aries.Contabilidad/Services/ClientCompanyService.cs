using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.Companies;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class ClientCompanyService : BaseHttpService, IClientCompanyService
    {
        public ClientCompanyService(IHttpClientFactory httpClientFactory, ILogger<ClientCompanyService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            var response = await _httpClient.GetAsync("company/getAll");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Company>>(content, _jsonOptions)
                   ?? Enumerable.Empty<Company>();
        }

        public async Task<Company> GetCompanyByCodeAsync(string code)
        {
            var response = await _httpClient.GetAsync($"company/{code}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Company>(content, _jsonOptions);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            var response = await _httpClient.PostAsJsonAsync("company/Create", company, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Company>(content, _jsonOptions);
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            var response = await _httpClient.PostAsJsonAsync("company/Update", company, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCompanyAsync(string code)
        {
            var response = await _httpClient.DeleteAsync($"company/delete/{code}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetNextCompanyCode()
        {
            var response = await _httpClient.GetAsync("company/BuildCode");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var company = JsonSerializer.Deserialize<Company>(content, _jsonOptions);
            return company?.Code;
        }
    }
}
