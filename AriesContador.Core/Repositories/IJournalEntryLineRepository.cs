using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryLineRepository : IRepository<JournalEntryLine>
    {
        JournalEntryLine GetById(int id);
        IEnumerable<JournalEntryLine> FindByJournalEntryId(int journalEntryId);
        IEnumerable<JournalEntryLine> FindByAccountIdAndPostingPeriodId(int accountId, int postingPeriodId);
        IEnumerable<JournalEntryLineDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam);
        void RestoreJournalEntryLine(JournalEntryLine entryLine);
    }
}
