using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Reports;
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
            var response = await _httpClient.GetAsync($"account/{Uri.EscapeDataString(companyId)}/accounts");
            await EnsureSuccess(response);
            return await Read<List<Account>>(response) ?? new List<Account>();
        }

        public async Task<Account> FindAccountAsync(int accountId)
        {
            var response = await _httpClient.GetAsync($"account/FindAccount/{accountId}");
            await EnsureSuccess(response);
            return await Read<Account>(response) ?? throw new InvalidOperationException("Account not found");
        }

        public async Task<Account> GetAccountBalanceAsync(string companyId, int accountId, DateTime startMonth, DateTime endMonth)
        {
            var url = $"account/balance/{accountId}?companyId={Uri.EscapeDataString(companyId)}"
                      + $"&startMonth={startMonth:yyyy-MM-dd}&endMonth={endMonth:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            await EnsureSuccess(response);
            return await Read<Account>(response) ?? throw new InvalidOperationException("Account balance not found");
        }

        public async Task<EvaluateParentResult> EvaluateParentAsync(Account parent)
        {
            var response = await _httpClient.PostAsJsonAsync("account/EvaluateParent", parent, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<EvaluateParentResult>(response) ?? new EvaluateParentResult { CanProceed = true };
        }

        public async Task CreateAccountAsync(Account account)
        {
            var response = await _httpClient.PostAsJsonAsync("account/Create", account, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            var response = await _httpClient.PostAsJsonAsync("account/Update", account, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task DeleteAccountAsync(Account account)
        {
            var response = await _httpClient.PostAsJsonAsync("account/Delete", account, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task<List<Account>> EnsurePurchaseAccountsAsync(string companyId)
        {
            var response = await _httpClient.PostAsync(
                $"account/{Uri.EscapeDataString(companyId)}/ensure-purchase-accounts", null);
            await EnsureSuccess(response);
            return await Read<List<Account>>(response) ?? new List<Account>();
        }

        public async Task<List<AccountMovementRow>> GetAccountMovementsAsync(int accountId)
        {
            var response = await _httpClient.GetAsync($"account/{accountId}/movements");
            await EnsureSuccess(response);
            return await Read<List<AccountMovementRow>>(response) ?? new List<AccountMovementRow>();
        }

        private async Task<T?> Read<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return default;
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(ExtractDetail(body) ?? response.ReasonPhrase ?? "Error de API");
        }

        private static string? ExtractDetail(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("detail", out var detail))
                    return detail.GetString();
                if (doc.RootElement.TryGetProperty("Detail", out var detailPascal))
                    return detailPascal.GetString();
            }
            catch
            {
                // not JSON
            }
            return body.Length > 240 ? body.Substring(0, 240) : body;
        }
    }
}
