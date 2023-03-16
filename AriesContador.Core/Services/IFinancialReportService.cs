using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;

namespace AriesContador.Core.Services
{
    public interface  IFinancialReportService
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams);
        IEnumerable<BalanceComprobacionReport> BalanceComprobacionReport(BasicReportParam reportParam);
        ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(BasicReportParam reportParam);
        ClosurePostingPeriodBalance PreviousClosurePostingPeriodBalance(BasicReportParam reportParam);
        IEnumerable<PostingPeriodInfoReport> PostingPeriodInfo(string companyId);
        IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId);

        Task<DataTable> AccountMoving(); 
    }
}
