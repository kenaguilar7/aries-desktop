using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;

namespace AriesContador.Core.Services
{
    public interface  IFinancialReportService
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams);
        IEnumerable<BalanceComprobacionReport> BalanceComprobacionReport(BasicReportParam reportParam);
        ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(BasicReportParam reportParam); 
    }
}
