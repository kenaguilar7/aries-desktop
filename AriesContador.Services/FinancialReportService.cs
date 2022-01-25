using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AriesContador.Core;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;
using AriesContador.Core.Models.Utils;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Enumeradores;

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

        public ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(BasicReportParam reportParam)
        {
            var report = new List<EstadoResultadoIntegralReport>();
            IEnumerable<Account> accountsReport = new List<Account>();
            //var postingPeriods = _unitOfWork.PostingPeriodRepository.FindByCompanyId(reportParam.CompanyId).OrderBy(p=>p.Date);

            //var firstPeriod = postingPeriods.FirstOrDefault();
            //var firstPeriodString = firstPeriod.Date.ParseToYearMonthStringFromDate();

            //don't create the previous balance. 
            //if (firstPeriodString.Equals(reportParam.FirstDate))
            //{
            //    accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);
            //}
            //else
            //{
            //    //var stringToDate = reportParam.FirstDate.ParseToDateTimeWithFromMimFormat(); 

            //    //var previusBalanceReportParam = new BasicReportParam()
            //    //{
            //    //    CompanyId = reportParam.CompanyId, 
            //    //    FirstDate = firstPeriodString, 
            //    //    EndDate = stringToDate.AddMonths(-1).ParseToYearMonthStringFromDate()
            //    //}; 

            //    //var accountForPreviewsBalance = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(previusBalanceReportParam);
            //    accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);

            //    //accountsReport.FillPriorBalance(accountForPreviewsBalance);

            //}


            accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);

            foreach (var account in accountsReport)
            {
                var auxAccountWithOutMoves = account.AccountType == AccountType.Cuenta_Auxiliar &&
                                                 account.Editable == true && account.CurrentBalance == 0; 

                if (!auxAccountWithOutMoves)
                {
                    var rLine = new EstadoResultadoIntegralReport
                    {
                        Account = (account.AccountType == AccountType.Cuenta_Titulo)?$"TOTAL {account.Name}":account.Name,
                        AccountPath = account.PathDirection,
                        SaldoActual = account.CurrentBalance,
                        IsMainAccount = account.AccountType == AccountType.Cuenta_Titulo
                    };
                    report.Add(rLine);
                }
            }

            var resultAmount = accountsReport.GetTotalPeridasYGanancias();
            return new ResultReportEstadoResultadoIntegral() {Results = report, TotalPeridaGanancia = resultAmount}; 
        }

        public ClosurePostingPeriodBalance PreviousClosurePostingPeriodBalance(BasicReportParam reportParam)
        {
            var accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);
            return new ClosurePostingPeriodBalance(){Amount = accountsReport.GetTotalPeridasYGanancias()}; 
        }

        public IEnumerable<PostingPeriodInfoReport> PostingPeriodInfo(string companyId)
        {
            var postingPeriods = _unitOfWork.FinancialReportRepository.PostingPeriodReport(companyId);
            var returnList = new List<PostingPeriodInfoReport>();

            foreach (var postingPeriod in postingPeriods)
            {
                var item = new PostingPeriodInfoReport
                {
                    AccountPeriodName = postingPeriod.PostingPeriodDateString,
                    Status = postingPeriod.Status, 
                    CreatedDate = postingPeriod.CreatedDateString,
                    ClosedDate = postingPeriod.ClosedDateString, 
                    UserName = postingPeriod.UserName
                }; 
                returnList.Add(item);
            }
            return returnList; 
        }

        public IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId)
        {
            return _unitOfWork.FinancialReportRepository.ClosingPostingPeriodReport(companyId); 
        }
    }
}


