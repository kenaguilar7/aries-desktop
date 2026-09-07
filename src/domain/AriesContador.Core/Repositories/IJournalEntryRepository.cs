using AriesContador.Core.Models.JournalEntries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IJournalEntryRepository : IRepository<JournalEntry>
    {
        Task<JournalEntry> GetByIdAsync(int jEntryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int postPeriodId, CancellationToken cancellationToken = default);
        Task<int> GetConsecutiveNumberAsync(int postingPeriodId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task RestoreJournalEntryAsync(JournalEntry entryLine, CancellationToken cancellationToken = default);
    }
}
