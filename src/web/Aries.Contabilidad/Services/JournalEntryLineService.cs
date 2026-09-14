using System.Net.Http.Json;
using AriesContador.Core.Models.JournalEntries;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class JournalEntryLineService : BaseHttpService, IJournalEntryLineService
    {
        private readonly ILocalStorageService _localStorageService;

        public JournalEntryLineService(
            IHttpClientFactory httpClientFactory,
            ILogger<JournalEntryLineService> logger,
            ILocalStorageService localStorageService)
            : base(httpClientFactory, logger)
        {
            _localStorageService = localStorageService;
        }

        public async Task<int> CreateJournalEntryLineAsync(JournalEntryLine journalEntryLine)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            if (journalEntryLine.CreatedBy == 0)
                journalEntryLine.CreatedBy = user.Id;
            journalEntryLine.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntryLine/CreateJournalEntryLine", journalEntryLine, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>(_jsonOptions);
        }

        public async Task UpdateJournalEntryLineAsync(JournalEntryLine journalEntryLine)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntryLine.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntryLine/UpdateJournalEntryLine", journalEntryLine, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteJournalEntryLineAsync(JournalEntryLine journalEntryLine)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntryLine.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntryLine/DeleteJournalEntryLine", journalEntryLine, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<JournalEntryLine>> GetJournalEntryLinesAsync(int journalEntryId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<JournalEntryLine>>(
                $"journalEntryLine/FindJournalEntryLine/{journalEntryId}", _jsonOptions);
            return response ?? new List<JournalEntryLine>();
        }

        public async Task RestoreJournalEntryLineAsync(JournalEntryLine journalEntryLine)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntryLine.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntryLine/RestoreJournalEntryLine", journalEntryLine, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<JournalEntryLineDeletedReport>> GetDeletedJournalEntryLinesAsync(BasicReportParam reportParam)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "journalEntryLine/GetDeletedJournalEntryLines", reportParam, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<JournalEntryLineDeletedReport>>(_jsonOptions);
            return result ?? new List<JournalEntryLineDeletedReport>();
        }
    }
}
