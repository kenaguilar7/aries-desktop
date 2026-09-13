using System.Net.Http.Json;
using AriesContador.Core.Models.Accounts;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class AccountService : BaseHttpService, IAccountService
    {
        public AccountService(IHttpClientFactory httpClientFactory, ILogger<AccountService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<Account>> GetAccountsAsync(string companyId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Account>>(
                $"account/{companyId}/accounts", _jsonOptions);
            return response ?? new List<Account>();
        }

        public async Task<Account> FindAccountAsync(int accountId)
        {
            var response = await _httpClient.GetFromJsonAsync<Account>(
                $"account/FindAccount/{accountId}", _jsonOptions);
            return response ?? throw new InvalidOperationException("Account not found");
        }

        public async Task<Account> GetAccountBalanceAsync(string companyId, int accountId, DateTime startMonth, DateTime endMonth)
        {
            var url = $"account/balance/{accountId}?companyId={Uri.EscapeDataString(companyId)}"
                      + $"&startMonth={startMonth:yyyy-MM-dd}&endMonth={endMonth:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<Account>(url, _jsonOptions);
            return response ?? throw new InvalidOperationException("Account balance not found");
        }
    }
}
