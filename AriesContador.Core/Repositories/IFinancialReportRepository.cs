using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;

namespace AriesContador.Core.Repositories
{
    public interface IFinancialReportRepository
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams);
        IEnumerable<Account> EstadoResultadoIntegralAccounts(BasicReportParam reportParam);
        IEnumerable<PostingPeriodInfo> PostingPeriodReport(string companyId);
        IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId); 
    }
}
