using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialServices
{
    public class AccountingReportService : IFinancialReportService
    {
        public Task<DataTable> AccountMoving()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BalanceComprobacionReport> BalanceComprobacionReport(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId)
        {
            throw new NotImplementedException();
        }

        public ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PostingPeriodInfoReport> PostingPeriodInfo(string companyId)
        {
            throw new NotImplementedException();
        }

        public ClosurePostingPeriodBalance PreviousClosurePostingPeriodBalance(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }
    }
}
