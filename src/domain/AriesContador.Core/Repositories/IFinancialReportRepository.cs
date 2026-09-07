using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;

namespace AriesContador.Core.Repositories
{
    public interface IFinancialReportRepository
    {
        Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> EstadoResultadoIntegralAccountsAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostingPeriodInfo>> PostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default);
    }
}
