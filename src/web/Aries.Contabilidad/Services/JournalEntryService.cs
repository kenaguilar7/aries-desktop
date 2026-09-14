using System.Net.Http.Json;
using AriesContador.Core.Models.JournalEntries;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class JournalEntryService : BaseHttpService, IJournalEntryService
    {
        private readonly ILocalStorageService _localStorageService;

        public JournalEntryService(
            IHttpClientFactory httpClientFactory,
            ILogger<JournalEntryService> logger,
            ILocalStorageService localStorageService)
            : base(httpClientFactory, logger)
        {
            _localStorageService = localStorageService;
        }

        public async Task<List<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<JournalEntry>>(
                $"journalEntry/GetJournalEntries/{postingPeriodId}", _jsonOptions);
            return response ?? new List<JournalEntry>();
        }

        public async Task<JournalEntry?> GetJournalEntryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"journalEntry/GetJournalEntryById/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<JournalEntry>(_jsonOptions);
        }

        public async Task<int> GetConsecutiveNumberAsync(int postingPeriodId)
        {
            return await _httpClient.GetFromJsonAsync<int>(
                $"journalEntry/GetConsecutiveNumber/{postingPeriodId}", _jsonOptions);
        }

        public async Task<int> CreateJournalEntryAsync(JournalEntry journalEntry)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            if (journalEntry.CreatedBy == 0)
                journalEntry.CreatedBy = user.Id;
            journalEntry.UpdatedBy = user.Id;
            journalEntry.ApplyStatusFromBalance();

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/CreateJournalEntry", journalEntry, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>(_jsonOptions);
        }

        public async Task UpdateJournalEntryAsync(JournalEntry journalEntry)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntry.UpdatedBy = user.Id;
            journalEntry.ApplyStatusFromBalance();

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/UpdateJournalEntry", journalEntry, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteJournalEntryAsync(JournalEntry journalEntry)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntry.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/DeleteJournalEntry", journalEntry, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdatePeriodAsync(JournalEntry journalEntry)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntry.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/UpdatedJournalEntryPeriod", journalEntry, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreJournalEntryAsync(JournalEntry journalEntry)
        {
            var user = await _localStorageService.GetCurrentUserSesion();
            journalEntry.UpdatedBy = user.Id;

            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/RestoreJournalEntry", journalEntry, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<JournalEntryDeletedReport>> GetDeletedJournalEntriesAsync(BasicReportParam reportParam)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/GetDeletedJournalEntries", reportParam, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<JournalEntryDeletedReport>>(_jsonOptions);
            return result ?? new List<JournalEntryDeletedReport>();
        }

        public async Task<List<JournalEntryReport>> GetJournalEntryReportAsync(BasicReportParam reportParam)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "journalEntry/GetJournalEntryReport", reportParam, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<JournalEntryReport>>(_jsonOptions);
            return result ?? new List<JournalEntryReport>();
        }
    }
}
