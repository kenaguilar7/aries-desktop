using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Models.Accounts;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Repositories
{
    public interface IFinancialReportRepository
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams);
        IEnumerable<Account> EstadoResultadoIntegralAccounts(BasicReportParam reportParam); 
    }
}
