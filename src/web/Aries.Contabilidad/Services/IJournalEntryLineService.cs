using AriesContador.Core.Models.JournalEntries;

namespace Aries.Contabilidad.Services
{
    public interface IJournalEntryLineService
    {
        Task<int> CreateJournalEntryLineAsync(JournalEntryLine journalEntryLine);
        Task UpdateJournalEntryLineAsync(JournalEntryLine journalEntryLine);
        Task DeleteJournalEntryLineAsync(JournalEntryLine journalEntryLine);
        Task<List<JournalEntryLine>> GetJournalEntryLinesAsync(int journalEntryId);
        Task RestoreJournalEntryLineAsync(JournalEntryLine journalEntryLine);
        Task<List<JournalEntryLineDeletedReport>> GetDeletedJournalEntryLinesAsync(BasicReportParam reportParam);
    }
}
