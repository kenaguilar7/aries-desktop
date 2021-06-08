using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Repositories
{
    public interface IFinancialReportRepository
    {
        IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams);
    }
}
