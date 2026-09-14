using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubFinancialReportService : IFinancialReportService
    {
        public Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryReport>>(Array.Empty<JournalEntryReport>());

        public Task<IEnumerable<BalanceComprobacionReport>> BalanceComprobacionReportAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<BalanceComprobacionReport>>(Array.Empty<BalanceComprobacionReport>());

        public Task<ResultReportEstadoResultadoIntegral> EstadoResultadoIntegralAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResultReportEstadoResultadoIntegral());

        public Task<ClosurePostingPeriodBalance> PreviousClosurePostingPeriodBalanceAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ClosurePostingPeriodBalance());

        public Task<IEnumerable<PostingPeriodInfoReport>> PostingPeriodInfoAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<PostingPeriodInfoReport>>(Array.Empty<PostingPeriodInfoReport>());

        public Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<ClosingPostingPeriodReport>>(Array.Empty<ClosingPostingPeriodReport>());

        public Task<DataTable> GetAccountMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default) =>
            Task.FromResult(new DataTable());
    }
}
