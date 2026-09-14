using System.Net.Http.Json;
using System.Text.Json;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PostingPeriodService : BaseHttpService, IPostingPeriodService
    {
        private readonly ILocalStorageService _localStorageService;

        public PostingPeriodService(
            IHttpClientFactory httpClientFactory,
            ILogger<PostingPeriodService> logger,
            ILocalStorageService localStorageService)
            : base(httpClientFactory, logger)
        {
            _localStorageService = localStorageService;
        }

        public async Task<List<PostingPeriod>> GetPostingPeriodsAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"postingPeriod/GetPostingPeriods/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<PostingPeriod>>(response) ?? new List<PostingPeriod>();
        }

        public async Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreatedAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"postingPeriod/GetAvailablePostingPeriodsForBeCreated/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<PostingPeriod>>(response) ?? new List<PostingPeriod>();
        }

        public async Task CreatePostingPeriodAsync(PostingPeriod postingPeriod)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            if (postingPeriod.CreatedBy == 0)
                postingPeriod.CreatedBy = user.Id;
            postingPeriod.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync("postingPeriod/Create", postingPeriod, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task ClosePostingPeriodAsync(PostingPeriodEndClosing closing)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            if (closing.CreatedBy == 0)
                closing.CreatedBy = user.Id;
            closing.UpdatedBy = user.Id;
            if (closing.PostingPeriods != null)
            {
                foreach (var period in closing.PostingPeriods)
                    period.UpdatedBy = user.Id;
            }

            var response = await _httpClient.PostAsJsonAsync("postingPeriod/ClosePostingPeriod", closing, _jsonOptions);
            await EnsureSuccess(response);
        }

        public async Task<List<PostingPeriodInfoReport>> GetPostingPeriodInfoAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"postingPeriod/GetPostingPeriodInfo/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<PostingPeriodInfoReport>>(response) ?? new List<PostingPeriodInfoReport>();
        }

        public async Task<List<ClosingPostingPeriodReport>> GetClosingPostingPeriodReportAsync(string companyId)
        {
            var response = await _httpClient.GetAsync(
                $"postingPeriod/GetClosingPostingPeriodReport/{Uri.EscapeDataString(companyId)}");
            await EnsureSuccess(response);
            return await Read<List<ClosingPostingPeriodReport>>(response) ?? new List<ClosingPostingPeriodReport>();
        }

        public async Task<ClosurePostingPeriodBalance> GetClosureBalanceAsync(BasicReportParam reportParam)
        {
            var response = await _httpClient.PostAsJsonAsync("postingPeriod/GetClosureBalance", reportParam, _jsonOptions);
            await EnsureSuccess(response);
            return await Read<ClosurePostingPeriodBalance>(response) ?? new ClosurePostingPeriodBalance();
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
