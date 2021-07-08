using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;
using AriesContador.Core.Models.Utils; 

namespace AriesContador.Services
{
    public class FinancialReportService : IFinancialReportService
    {

        private readonly IUnitOfWork _unitOfWork;

        public FinancialReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams)
        {
            var output = _unitOfWork.FinancialReportRepository.JournalEntryReport(jEParams);
            return output; 
        }

        public IEnumerable<BalanceComprobacionReport> BalanceComprobacionReport(BasicReportParam reportParam)
        {
            var accounts = _unitOfWork.AccountRepository.AccountsWithBalanceByDateRange(reportParam);
            var report = new List<BalanceComprobacionReport>();

            foreach (var account in accounts)
            {
                var rLine = new BalanceComprobacionReport
                {
                    Account = account.Name,
                    AccountPath = account.PathDirection,
                    SaldoAnteriorDeb = (account.DebOCred == DebOCred.Debito) ? account.PriorBalance : 0,
                    SaldoAnteriorCred = (account.DebOCred == DebOCred.Credito) ? account.PriorBalance : 0,
                    SaldoMensualDeb = (account.DebOCred == DebOCred.Debito) ? account.MontlyBalance : 0,
                    SaldoMensualCred = (account.DebOCred == DebOCred.Credito) ? account.MontlyBalance : 0,
                    SaldoActualCuentaDeb = (account.DebOCred == DebOCred.Debito) ? account.CurrentBalance : 0,
                    SaldoActualCuentaCred = (account.DebOCred == DebOCred.Credito) ? account.CurrentBalance : 0,
                };

                report.Add(rLine);
            }

            return report; 
        }
    }
}
