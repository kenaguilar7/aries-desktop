using AriesContador.Core.Models.JournalEntries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryLineRepository : IRepository<JournalEntryLine>
    {
        Task<int> AddAsyncWithReturnId(JournalEntryLine entity, CancellationToken cancellationToken = default);
        Task<JournalEntryLine> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryLine>> FindByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryLine>> FindByAccountIdAndPostingPeriodIdAsync(int accountId, int postingPeriodId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryLineDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task RestoreJournalEntryLineAsync(JournalEntryLine entryLine, CancellationToken cancellationToken = default);
    }
}
