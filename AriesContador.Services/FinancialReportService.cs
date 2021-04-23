using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Services
{
    public class FinancialReportService : IFinancialReportService
    {

        private readonly IUnitOfWork _unitOfWork;

        public FinancialReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<JournalEntryReport> JournalEntryReport(JournalEntryReportParam jEParams)
        {
            var output = _unitOfWork.FinancialReportRepository.JournalEntryReport(jEParams);
            return output; 
        }
    }
}
