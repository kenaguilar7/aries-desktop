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
    }
}
