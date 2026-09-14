using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.Purchases;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PurchasingClient : BaseHttpService, IPurchasingClient
    {
        public PurchasingClient(IHttpClientFactory httpClientFactory, ILogger<PurchasingClient> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<Supplier>> GetAllAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"supplier/GetAll/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<Supplier>>(response) ?? new List<Supplier>();
        }

        public async Task<Supplier?> FindAsync(int id)
        {
            var response = await _httpClient.GetAsync($"supplier/Find/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            await EnsureSuccess(response);
            return await Read<Supplier>(response);
        }

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            var response = await _httpClient.PostAsJsonAsync("supplier/Create", supplier, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<Supplier>(response) ?? supplier;
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            var response = await _httpClient.PostAsJsonAsync("supplier/Update", supplier, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"supplier/Delete/{id}");
            await EnsureSuccess(response);
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
