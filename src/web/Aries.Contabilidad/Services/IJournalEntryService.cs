using AriesContador.Core.Models.JournalEntries;

namespace Aries.Contabilidad.Services
{
    public interface IJournalEntryService
    {
        Task<List<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId);
        Task<JournalEntry?> GetJournalEntryByIdAsync(int id);
        Task<int> GetConsecutiveNumberAsync(int postingPeriodId);
        Task<int> CreateJournalEntryAsync(JournalEntry journalEntry);
        Task UpdateJournalEntryAsync(JournalEntry journalEntry);
        Task DeleteJournalEntryAsync(JournalEntry journalEntry);
        Task UpdatePeriodAsync(JournalEntry journalEntry);
        Task RestoreJournalEntryAsync(JournalEntry journalEntry);
        Task<List<JournalEntryDeletedReport>> GetDeletedJournalEntriesAsync(BasicReportParam reportParam);
        Task<List<JournalEntryReport>> GetJournalEntryReportAsync(BasicReportParam reportParam);
    }
}
