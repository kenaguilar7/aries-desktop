using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Services
{
    public interface  IFinancialReportService
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(JournalEntryReportParam jEParams);
    }
}
