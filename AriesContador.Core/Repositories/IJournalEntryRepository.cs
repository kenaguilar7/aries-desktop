using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryRepository : IRepository<JournalEntry>
    {
        JournalEntry GetById(int jEntryId); 
        IEnumerable<JournalEntry> FindByPostingPeriodId(int postPeriodId);
        Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int pstPeriodId); 
        int GetConsecutiveNumber(int postingPeriodId);
        Task<int> GetConsecutiveNumberAsync(int postingPeriodId); 
        IEnumerable<JournalEntryDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam);
        void RestoreJournalEntry(JournalEntry entryLine);
        Task<int> AddAsyncReturningId(JournalEntry journalEntry);
        Task UpdateAsync(JournalEntry entity); 
    }
}
