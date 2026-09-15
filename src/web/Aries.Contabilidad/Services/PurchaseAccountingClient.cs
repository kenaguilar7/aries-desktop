using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.Purchases;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PurchaseAccountingClient : BaseHttpService, IPurchaseAccountingClient
    {
        public PurchaseAccountingClient(IHttpClientFactory httpClientFactory, ILogger<PurchaseAccountingClient> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<PurchaseAccountMap> GetAccountMapAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"integration/purchase-account-map/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<PurchaseAccountMap>(response) ?? new PurchaseAccountMap { CompanyId = companyId };
        }

        public async Task SaveAccountMapAsync(PurchaseAccountMap map)
        {
            var response = await _httpClient.PostAsJsonAsync("integration/purchase-account-map", map, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task<PurchaseAccountMap> EnsureSuggestedAccountsAsync(string companyId)
        {
            var response = await _httpClient.PostAsync(
                $"integration/purchase-account-map/{Uri.EscapeDataString(companyId)}/ensure-accounts", null);
            await EnsureSuccess(response);
            return await Read<PurchaseAccountMap>(response) ?? new PurchaseAccountMap { CompanyId = companyId };
        }

        public async Task<List<Purchase>> GetUnpostedPurchasesAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"integration/purchase-accounting/{Uri.EscapeDataString(companyId)}/unposted");
            await EnsureSuccess(response);
            return await Read<List<Purchase>>(response) ?? new List<Purchase>();
        }

        public async Task<PurchasePostingPreview> PreviewPurchaseAsync(int purchaseId)
        {
            var response = await _httpClient.GetAsync($"integration/purchase-accounting/preview/{purchaseId}");
            await EnsureSuccess(response);
            return await Read<PurchasePostingPreview>(response) ?? new PurchasePostingPreview();
        }

        public async Task<PurchasePostingPreview> PostPurchaseAsync(int purchaseId)
        {
            var response = await _httpClient.PostAsync($"integration/purchase-accounting/post/{purchaseId}", null);
            await EnsureSuccess(response);
            return await Read<PurchasePostingPreview>(response) ?? new PurchasePostingPreview();
        }

        public async Task<PurchaseDetail> GetPurchaseDetailAsync(int purchaseId)
        {
            var response = await _httpClient.GetAsync($"integration/purchase-accounting/detail/{purchaseId}");
            await EnsureSuccess(response);
            return await Read<PurchaseDetail>(response) ?? new PurchaseDetail();
        }

        public async Task<SupplierPayment> PayPurchaseAsync(SupplierPayment payment)
        {
            var response = await _httpClient.PostAsJsonAsync("integration/purchase-accounting/pay", payment, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<SupplierPayment>(response) ?? payment;
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
