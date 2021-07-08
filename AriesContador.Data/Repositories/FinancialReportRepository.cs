using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using CapaEntidad.Entidades.JournalEntries;
using System.Linq;

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
    }
}
