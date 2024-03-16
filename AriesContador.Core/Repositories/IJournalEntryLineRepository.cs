using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryLineRepository : IRepository<JournalEntryLine>
    {
        Task<int> AddAsyncWithReturnId(JournalEntryLine entity);
        JournalEntryLine GetById(int id);
        IEnumerable<JournalEntryLine> FindByJournalEntryId(int journalEntryId);
        IEnumerable<JournalEntryLine> FindByAccountIdAndPostingPeriodId(int accountId, int postingPeriodId);
        IEnumerable<JournalEntryLineDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam);
        void RestoreJournalEntryLine(JournalEntryLine entryLine);
        Task<IEnumerable<JournalEntryLine>> FindByJournalEntryIdAsync(int journalEntryId);
        Task UpdateAsync(JournalEntryLine entity); 
    }
}
