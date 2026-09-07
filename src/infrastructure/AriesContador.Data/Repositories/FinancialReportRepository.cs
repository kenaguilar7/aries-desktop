using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Reports;

namespace AriesContador.Data.Repositories
{
    public class FinancialReportRepository : IFinancialReportRepository
    {
        private readonly IConnectionString _connectionString;
        public FinancialReportRepository(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }


        public IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            IEnumerable<JournalEntryReport> output = dataAccess.LoadData<JournalEntryReport, BasicReportParam>("SP_JournalEntryReportByDateRange", jEParams);
            return output;
        }

        public IEnumerable<Account> EstadoResultadoIntegralAccounts(BasicReportParam reportParam)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<Account, BasicReportParam>("SP_EstadoResultadoIntegralReport", reportParam);
            output.BuildAccountsBalance();
            return output.OrderByDescTree();
            //return output;
        }

        public IEnumerable<PostingPeriodInfo> PostingPeriodReport(string companyId)
        {
            var dataAcces = new MySqlDataAccess(_connectionString);
            var output = dataAcces.LoadData<PostingPeriodInfo, dynamic>("SP_GetPostingPeriodReport", new {CompanyId = companyId});
            return output; 
        }

        public IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId)
        {
            var dataAcces = new MySqlDataAccess(_connectionString);
            var output = dataAcces.LoadData<ClosingPostingPeriodReport, dynamic>("SP_GetClosingPostingPeriodReport", new { CompanyId = companyId });
            return output;
        }
    }
}
