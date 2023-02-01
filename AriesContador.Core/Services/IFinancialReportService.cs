using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Models.PostingPeriods;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;

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
    }
}
