using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
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

        public Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.FinancialReportRepository.JournalEntryReportAsync(jEParams, cancellationToken);
        }

        public async Task<IEnumerable<BalanceComprobacionReport>> BalanceComprobacionReportAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var accounts = await _unitOfWork.AccountRepository.AccountsWithBalanceByDateRangeAsync(reportParam, cancellationToken)
                .ConfigureAwait(false);
            var report = new List<BalanceComprobacionReport>();

            foreach (var account in accounts)
            {
                var rLine = new BalanceComprobacionReport
                {
                    Account = account.Name,
                    AccountPath = account.PathDirection,
                    SaldoAnteriorDeb = (account.DebOCred == DebOrCred.Debito) ? account.PriorBalance : 0,
                    SaldoAnteriorCred = (account.DebOCred == DebOrCred.Credito) ? account.PriorBalance : 0,
                    SaldoMensualDeb = (account.DebOCred == DebOrCred.Debito) ? account.MontlyBalance : 0,
                    SaldoMensualCred = (account.DebOCred == DebOrCred.Credito) ? account.MontlyBalance : 0,
                    SaldoActualCuentaDeb = (account.DebOCred == DebOrCred.Debito) ? account.CurrentBalance : 0,
                    SaldoActualCuentaCred = (account.DebOCred == DebOrCred.Credito) ? account.CurrentBalance : 0,
                };

                report.Add(rLine);
            }

            return report;
        }

        public async Task<ResultReportEstadoResultadoIntegral> EstadoResultadoIntegralAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var report = new List<EstadoResultadoIntegralReport>();
            var accountsReport = await _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccountsAsync(reportParam, cancellationToken)
                .ConfigureAwait(false);

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

        public async Task<ClosurePostingPeriodBalance> PreviousClosurePostingPeriodBalanceAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var accountsReport = await _unitOfWork.FinancialReportRepository.EstadoResultadoIntegralAccountsAsync(reportParam, cancellationToken)
                .ConfigureAwait(false);
            return new ClosurePostingPeriodBalance() { Amount = accountsReport.GetTotalPeridasYGanancias() };
        }

        public async Task<IEnumerable<PostingPeriodInfoReport>> PostingPeriodInfoAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var postingPeriods = await _unitOfWork.FinancialReportRepository.PostingPeriodReportAsync(companyId, cancellationToken)
                .ConfigureAwait(false);
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

        public Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.FinancialReportRepository.ClosingPostingPeriodReportAsync(companyId, cancellationToken);
        }

        public async Task<DataTable> GetAccountMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default)
        {
            var table = await _unitOfWork.AccountRepository.GetMovementReportAsync(accountId, auxiliar, cancellationToken)
                .ConfigureAwait(false);
            decimal lastSaldoActual = 0m;
            foreach (DataRow item in table.Rows)
            {
                var debito = string.IsNullOrWhiteSpace(Convert.ToString(item["Debito"])) ? 0m : Convert.ToDecimal(item["Debito"]);
                var credito = string.IsNullOrWhiteSpace(Convert.ToString(item["Credito"])) ? 0m : Convert.ToDecimal(item["Credito"]);
                var tag = Convert.ToInt32(Convert.ToDecimal(item["Saldo Actual"]));
                lastSaldoActual = RunningBalance(tag, lastSaldoActual, debito, credito);
                item["Saldo Actual"] = string.Format("{0:n}", lastSaldoActual);
            }
            return table;
        }

        private static decimal RunningBalance(int accountTag, decimal saldo, decimal debito, decimal credito)
        {
            var debitNature = accountTag == 1 || accountTag == 5 || accountTag == 6;
            return debitNature ? saldo + debito - credito : saldo + credito - debito;
        }
    }
}
