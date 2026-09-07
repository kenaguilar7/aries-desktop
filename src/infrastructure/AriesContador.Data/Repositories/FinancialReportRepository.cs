using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;

namespace AriesContador.Data.Repositories
{
    public class FinancialReportRepository : IFinancialReportRepository
    {
        private readonly IConnectionString _connectionString;
        public FinancialReportRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<JournalEntryReport, BasicReportParam>("SP_JournalEntryReportByDateRange", jEParams, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Account>> EstadoResultadoIntegralAccountsAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<Account, BasicReportParam>("SP_EstadoResultadoIntegralReport", reportParam, cancellationToken)
                .ConfigureAwait(false);
            output.BuildAccountsBalance();
            return output.OrderByDescTree();
        }

        public async Task<IEnumerable<PostingPeriodInfo>> PostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<PostingPeriodInfo, dynamic>("SP_GetPostingPeriodReport", new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<ClosingPostingPeriodReport, dynamic>("SP_GetClosingPostingPeriodReport", new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
