using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.PointOfSale;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PosService : BaseHttpService, IPosService
    {
        public PosService(IHttpClientFactory httpClientFactory, ILogger<PosService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<Product>> GetProductsAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"product/byCompany/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<Product>>(response) ?? new List<Product>();
        }

        public async Task<Product?> FindByBarcodeAsync(string companyId, string barcode)
        {
            var response = await _httpClient.GetAsync(
                $"product/barcode/{Uri.EscapeDataString(companyId)}/{Uri.EscapeDataString(barcode)}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            await EnsureSuccess(response);
            return await Read<Product>(response);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("product/Create", product, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<Product>(response) ?? product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("product/Update", product, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"product/delete/{id}");
            await EnsureSuccess(response);
        }

        public async Task<List<SalesRegister>> GetRegistersAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"salesRegister/byCompany/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<SalesRegister>>(response) ?? new List<SalesRegister>();
        }

        public async Task<SalesRegister> CreateRegisterAsync(SalesRegister register)
        {
            var response = await _httpClient.PostAsJsonAsync("salesRegister/Create", register, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<SalesRegister>(response) ?? register;
        }

        public async Task UpdateRegisterAsync(SalesRegister register)
        {
            var response = await _httpClient.PostAsJsonAsync("salesRegister/Update", register, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task DeleteRegisterAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"salesRegister/delete/{id}");
            await EnsureSuccess(response);
        }

        public async Task<CashRegisterStatus> GetStatusAsync(int registerId)
        {
            var response = await _httpClient.GetAsync($"caja/estado/{registerId}");
            await EnsureSuccess(response);
            return await Read<CashRegisterStatus>(response) ?? CashRegisterStatus.Closed();
        }

        public async Task<SalesRegisterSession> OpenAsync(OpenCashRegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("caja/apertura", request, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<SalesRegisterSession>(response)
                   ?? throw new InvalidOperationException("No se pudo abrir la caja");
        }

        public async Task<SalesRegisterSession> CloseAsync(CloseCashRegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("caja/cierre", request, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<SalesRegisterSession>(response)
                   ?? throw new InvalidOperationException("No se pudo cerrar la caja");
        }

        public async Task<List<SalesRegisterSession>> GetHistoryAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"caja/historial/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<SalesRegisterSession>>(response) ?? new List<SalesRegisterSession>();
        }

        public async Task<List<Sale>> GetSalesByCompanyAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"sale/byCompany/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<Sale>>(response) ?? new List<Sale>();
        }

        public async Task<List<Sale>> GetSalesBySessionAsync(int sessionId)
        {
            var response = await _httpClient.GetAsync($"sale/bySession/{sessionId}");
            await EnsureSuccess(response);
            return await Read<List<Sale>>(response) ?? new List<Sale>();
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            var response = await _httpClient.PostAsJsonAsync("sale/Create", sale, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<Sale>(response) ?? sale;
        }

        public async Task<PosTodaySalesReport> GetTodaySalesAsync(string companyId, DateTime? date = null)
        {
            var url = $"posReport/ventasHoy/{Uri.EscapeDataString(companyId)}";
            if (date.HasValue)
                url += $"?date={date.Value:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            await EnsureSuccess(response);
            return await Read<PosTodaySalesReport>(response) ?? new PosTodaySalesReport();
        }

        public async Task<List<Product>> GetLowStockAsync(string companyId, decimal? minimum = null)
        {
            var url = $"posReport/stockBajo/{Uri.EscapeDataString(companyId)}";
            if (minimum.HasValue)
                url += $"?minimo={minimum.Value}";
            var response = await _httpClient.GetAsync(url);
            await EnsureSuccess(response);
            return await Read<List<Product>>(response) ?? new List<Product>();
        }

        public async Task<List<SalesRegisterSession>> GetClosingsAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"posReport/cierres/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<SalesRegisterSession>>(response) ?? new List<SalesRegisterSession>();
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
