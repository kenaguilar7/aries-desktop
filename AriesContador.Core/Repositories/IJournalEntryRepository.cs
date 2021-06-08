using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryRepository : IRepository<JournalEntry>
    {
        JournalEntry GetById(int jEntryId); 
        IEnumerable<JournalEntry> FindByPostingPeriodId(int postPeriodId);
        int GetConsecutiveNumber(int postingPeriodId);
        IEnumerable<JournalEntryDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam);
        void RestoreJournalEntry(JournalEntry entryLine);
    }
}
