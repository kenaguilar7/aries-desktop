using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;

namespace AriesContador.Core.Services
{
    public interface IFinancialReportService
    {
        Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default);
        Task<IEnumerable<BalanceComprobacionReport>> BalanceComprobacionReportAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task<ResultReportEstadoResultadoIntegral> EstadoResultadoIntegralAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task<ClosurePostingPeriodBalance> PreviousClosurePostingPeriodBalanceAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostingPeriodInfoReport>> PostingPeriodInfoAsync(string companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default);
        Task<DataTable> GetAccountMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default);
    }
}
