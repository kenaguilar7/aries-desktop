using AriesContador.Core.Models.JournalEntries;

namespace Aries.Contabilidad.Services
{
    public interface IJournalEntryService
    {
        Task<List<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId);
        Task<int> GetConsecutiveNumberAsync(int postingPeriodId);
        Task<int> CreateJournalEntryAsync(JournalEntry journalEntry);
        Task UpdateJournalEntryAsync(JournalEntry journalEntry);
        Task DeleteJournalEntryAsync(JournalEntry journalEntry);
    }
}
