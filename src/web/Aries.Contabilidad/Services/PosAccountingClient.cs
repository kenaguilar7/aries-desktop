using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.PointOfSale;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PosAccountingClient : BaseHttpService, IPosAccountingClient
    {
        public PosAccountingClient(IHttpClientFactory httpClientFactory, ILogger<PosAccountingClient> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<PosAccountMap> GetAccountMapAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"integration/account-map/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<PosAccountMap>(response) ?? new PosAccountMap { CompanyId = companyId };
        }

        public async Task SaveAccountMapAsync(PosAccountMap map)
        {
            var response = await _httpClient.PostAsJsonAsync("integration/account-map", map, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task<PosAccountMap> EnsureSuggestedAccountsAsync(string companyId)
        {
            var response = await _httpClient.PostAsync(
                $"integration/account-map/{Uri.EscapeDataString(companyId)}/ensure-accounts", null);
            await EnsureSuccess(response);
            return await Read<PosAccountMap>(response) ?? new PosAccountMap { CompanyId = companyId };
        }

        public async Task<List<SalesRegisterSession>> GetUnpostedSessionsAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"integration/pos-accounting/{Uri.EscapeDataString(companyId)}/unposted");
            await EnsureSuccess(response);
            return await Read<List<SalesRegisterSession>>(response) ?? new List<SalesRegisterSession>();
        }

        public async Task<PosPostingPreview> PreviewSessionAsync(int sessionId)
        {
            var response = await _httpClient.GetAsync($"integration/pos-accounting/preview/{sessionId}");
            await EnsureSuccess(response);
            return await Read<PosPostingPreview>(response) ?? new PosPostingPreview();
        }

        public async Task<PosPostingPreview> PostSessionAsync(int sessionId)
        {
            var response = await _httpClient.PostAsync($"integration/pos-accounting/post/{sessionId}", null);
            await EnsureSuccess(response);
            return await Read<PosPostingPreview>(response) ?? new PosPostingPreview();
        }

        public async Task<List<PosSessionReconciliationRow>> GetReconciliationAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"integration/reconciliation/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<PosSessionReconciliationRow>>(response) ?? new List<PosSessionReconciliationRow>();
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
