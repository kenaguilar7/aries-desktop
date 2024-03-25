using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
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
            var accounts = _unitOfWork.AccountRepository.AccountsWithBalanceByDateRange(reportParam).GetAwaiter().GetResult();
            var report = new List<BalanceComprobacionReport>();

            foreach (var account in accounts)
            {
                var rLine = new BalanceComprobacionReport
                {
                    Account = account.Name,
                    AccountPath = account.PathDirection,
                    SaldoAnteriorDeb = (account.DebOrCred == DebOrCred.Debito) ? account.PriorBalance : 0,
                    SaldoAnteriorCred = (account.DebOrCred == DebOrCred.Credito) ? account.PriorBalance : 0,
                    SaldoMensualDeb = (account.DebOrCred == DebOrCred.Debito) ? account.MontlyBalance : 0,
                    SaldoMensualCred = (account.DebOrCred == DebOrCred.Credito) ? account.MontlyBalance : 0,
                    SaldoActualCuentaDeb = (account.DebOrCred == DebOrCred.Debito) ? account.CurrentBalance : 0,
                    SaldoActualCuentaCred = (account.DebOrCred == DebOrCred.Credito) ? account.CurrentBalance : 0,
                };

                report.Add(rLine);
            }

            return report;
        }

        public ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(BasicReportParam reportParam)
        {
            var report = new List<EstadoResultadoIntegralReport>();
            IEnumerable<Account> accountsReport = new List<Account>();
            accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);

            foreach (var account in accountsReport)
            {
                var auxAccountWithOutMoves = account.AccountType == AccountType.Cuenta_Auxiliar &&
                                                 account.Editable == true && account.CurrentBalance == 0;

                if (!auxAccountWithOutMoves)
                {
                    var rLine = new EstadoResultadoIntegralReport
                    {
                        AccountPath = account.PathDirection,
                        SaldoActual = account.CurrentBalance,
                        IsMainAccount = account.AccountType == AccountType.Cuenta_Titulo
                    };
                    report.Add(rLine);
                }
            }

            var resultAmount = accountsReport.GetTotalPeridasYGanancias();
            return new ResultReportEstadoResultadoIntegral() { Results = report, TotalPeridaGanancia = resultAmount };
        }

        public ClosurePostingPeriodBalance PreviousClosurePostingPeriodBalance(BasicReportParam reportParam)
        {
            var accountsReport = _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccounts(reportParam);
            return new ClosurePostingPeriodBalance() { Amount = accountsReport.GetTotalPeridasYGanancias() };
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

        public Task<DataTable> AccountMoving()
        {
            throw new NotImplementedException();
        }
        //public IEnumerable<Core.Models.Reports.BalanceComprobacionReport> BalanceComprobacionReport(Core.Models.JournalEntries.BasicReportParam reportParam)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Core.Models.Reports.ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Core.Models.Reports.ResultReportEstadoResultadoIntegral EstadoResultadoIntegral(Core.Models.JournalEntries.BasicReportParam reportParam)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Core.Models.JournalEntries.JournalEntryReport> JournalEntryReport(Core.Models.JournalEntries.BasicReportParam jEParams)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Core.Models.Reports.PostingPeriodInfoReport> PostingPeriodInfo(string companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public ClosurePostingPeriodBalance PreviousClosurePostingPeriodBalance(Core.Models.JournalEntries.BasicReportParam reportParam)
        //{
        //    throw new NotImplementedException();
        //}
    }
}


