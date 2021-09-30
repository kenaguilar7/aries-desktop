using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models;
using AriesContador.Core.Services;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;

namespace AriesContador.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinancialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Account
        public IEnumerable<Account> GetAccounts(string companyId)
        {
            var output = _unitOfWork.AccountRepository.FindByCompanyId(companyId);
            return output;
        }

        public Account FindAccount(int id)
        {
            var account = _unitOfWork.AccountRepository.GetById(id);
            return account;
        }

        public IEnumerable<Account> GetDefaultAccounts()
        {
            var accounts = _unitOfWork.AccountRepository.GetDefaultAccounts();
            return accounts;
        }

        public void CreateAccount(Account account)
        {
            _unitOfWork.AccountRepository.Add(account);
        }

        public void UpdateAccount(Account account)
        {
            _unitOfWork.AccountRepository.Update(account);
        }

        public void UpdateAccountName(Account account)
        {
            _unitOfWork.AccountRepository.UpdatePartlyAccount(account);
        }

        public void DeleteAccount(Account account)
        {
            var accountTree = _unitOfWork.AccountRepository.FindByCompanyId(account.CompanyId).ToList();
            var accountVerify = accountTree.First(a => a.Id == account.Id); 


            if (accountVerify.AccountType == AccountType.Cuenta_Titulo ||
                accountVerify.AccountType == AccountType.Cuenta_De_Mayor)
            {
                throw new Exception($"No se pueden eliminar cuentas de tipo {accountVerify.AccountType.ToString().Replace('_',' ')}");
            }

            var accountHasMovements = _unitOfWork.AccountRepository.HasMovements(account.Id, account.CompanyId);
            if (accountHasMovements)
            {
                throw new Exception("Esta cuenta no se puede eliminar porque tiene movimientos asociados"); 
            }

            _unitOfWork.AccountRepository.Remove(account);
            //Account deleted successful, then if the father account doesn't have more child mark as auxiliar account
            accountTree.Remove(accountVerify); 
            var brothersAccounts = accountTree.Where(a => a.FatherAccount == accountVerify.FatherAccount);
            if (!brothersAccounts.Any())
            {
                //Update father account
                var fatherAccount = accountTree.Find(a => a.Id == accountVerify.FatherAccount);
                fatherAccount.AccountType = AccountType.Cuenta_Auxiliar; 
                _unitOfWork.AccountRepository.Update(fatherAccount);
            }
        }

        public IEnumerable<Account> GetAccountsBalance(BasicReportParam reportParam)
        {
            IEnumerable<Account> accountsReport = new List<Account>();
            var postingPeriods = _unitOfWork.PostingPeriodRepository.FindByCompanyId(reportParam.CompanyId).OrderBy(p => p.Date);

            var firstPeriod = postingPeriods.FirstOrDefault();
            var firstPeriodString = firstPeriod.Date.ParseToYearMonthStringFromDate();

            //don't create the previous balance. 
            if (firstPeriodString.Equals(reportParam.FirstDate))
            {
                accountsReport = _unitOfWork.AccountRepository.AccountsWithBalanceByDateRange(reportParam);
            }
            else
            {
                var stringToDate = reportParam.FirstDate.ParseToDateTimeWithFromMimFormat();

                var previusBalanceReportParam = new BasicReportParam()
                {
                    CompanyId = reportParam.CompanyId,
                    FirstDate = firstPeriodString,
                    EndDate = stringToDate.AddMonths(-1).ParseToYearMonthStringFromDate()
                };

                var accountForPreviewsBalance = _unitOfWork.AccountRepository.AccountsWithBalanceByDateRange(previusBalanceReportParam);
                accountsReport = _unitOfWork.AccountRepository.AccountsWithBalanceByDateRange(reportParam);

                accountsReport.FillPriorBalance(accountForPreviewsBalance);
            }

            return accountsReport;
        }

        public Account GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods)
        {
   
            throw new NotImplementedException(); 
        }

        //private IEnumerable<Account> BuildAccountBalance(IEnumerable<Account> accounts, IEnumerable<PostingPeriod> postingPeriods)
        //{
        //    foreach (var months in postingPeriods)
        //    {
        //        FillAccountWithJournalEntryLineByMonthId(accounts, months);
        //    }
        //    return accounts.BuildAccountsBalance();
        //}

        //private void FillAccountWithJournalEntryLineByMonthId(IEnumerable<Account> accounts, PostingPeriod months)
        //{
        //    var searhAccounts = accounts.Where(x => x.AccountType == AccountType.Cuenta_Auxiliar);
        //    foreach (var account in searhAccounts)
        //    {
        //        var jEnLines = _unitOfWork.JournalEntryLineRepository
        //                                    .FindByAccountIdAndPostingPeriodId(account.Id, months.Id);
        //        account.JournalEntryLines.AddRange(jEnLines);
        //    }
        //}


        #endregion

        #region Posting Periods
        public IEnumerable<PostingPeriod> GetPostingPeriods(string companyId)
        {
            var output = _unitOfWork.PostingPeriodRepository.FindByCompanyId(companyId);
            return output.OrderBy(x=>x.Date);
        }
        public void CreatePostingPeriod(PostingPeriod postingPeriod)
        {
            var postingPeriods = _unitOfWork.PostingPeriodRepository.FindByCompanyId(postingPeriod.CompanyId);

            var isRepeated = postingPeriods
                    .Any(x => x.Date.Year == postingPeriod.Date.Year &&
                     x.Date.Month == postingPeriod.Date.Month);

            if (isRepeated)
            {
                throw new Exception("Periodo contable con fechas repetidas");
            }
            else
            {
                _unitOfWork.PostingPeriodRepository.Add(postingPeriod);
            }

        }

        public void UpdatePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }
        public void ClosePostingPeriod(PostingPeriod postingPeriod)
        {
            _unitOfWork.PostingPeriodRepository.ClosePostingPeriod(postingPeriod);
        }
        public void DeletePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PostingPeriod> GetAvailablePostingPeriodsForBeCreated(string companyId)
        {
            var postingPeriods = this.GetPostingPeriods(companyId);

            if (postingPeriods.Count() > 0)
            {
                return CreateAvailablePostingPeriodsForBeCreate(postingPeriods);
            }
            else
            {
                return CreatePreEntityPostingPeriod(DateTime.Now);
            }
        }

        private IEnumerable<PostingPeriod> CreateAvailablePostingPeriodsForBeCreate(IEnumerable<PostingPeriod> postingPeriods)
        {

            var hasEntries = HasJournalEntries(postingPeriods);
            if (hasEntries)
            {
                var newerPeriod = postingPeriods.GetNewerAccountPeriod();
                var newerDate = newerPeriod.Date.AddMonths(1);
                return CreatePreEntityPostingPeriod(newerDate);
            }
            else
            {
                var olderPeriod = postingPeriods.GetOlderAccountPeriod();
                var olderDate = olderPeriod.Date.AddMonths(-1);

                var newerPeriod = postingPeriods.GetNewerAccountPeriod();
                var newerDate = newerPeriod.Date.AddMonths(1);

                return CreatePreEntityPostingPeriod(olderDate, newerDate);
            }
        }

        private bool HasJournalEntries(IEnumerable<PostingPeriod> postingPeriods)
        {

            foreach (var postingP in postingPeriods)
            {
                postingP.JournalEntries = _unitOfWork.JournalEntryRepository.FindByPostingPeriodId(postingP.Id).ToList();
            }

            var hasEntries = postingPeriods.Where(x => x.JournalEntries.Count > 0).Count();

            return hasEntries > 0;
        }

        private IEnumerable<PostingPeriod> CreatePreEntityPostingPeriod(DateTime fromDatePeriod)
        {
            return new List<PostingPeriod>() { CreatePostingPeriodEntity(fromDatePeriod) };
        }

        private IEnumerable<PostingPeriod> CreatePreEntityPostingPeriod(DateTime fromDatePeriod, DateTime toDatePeriod)
        {
            return new List<PostingPeriod>() { CreatePostingPeriodEntity(fromDatePeriod),
                                                  CreatePostingPeriodEntity(toDatePeriod) };
        }

        public PostingPeriod CreatePostingPeriodEntity(DateTime PeriodDate)
             => new PostingPeriod() { Date = PeriodDate };

        #endregion

        #region Journal Entry
        public IEnumerable<JournalEntry> GetJournalEntries(int postingPeriodId)
        {
            var output = _unitOfWork.JournalEntryRepository.FindByPostingPeriodId(postingPeriodId);
            return output;
        }
        public JournalEntry GetJournalEntryById(int id)
        {
            var output = _unitOfWork.JournalEntryRepository.GetById(id);
            //   output.JournalEntryLines = _unitOfWork.JournalEntryLineRepository.FindByJournalEntryId(id);
            return output;
        }

        public int CreateJournalEntryConsecutive(int postingPeriodId)
        {
            var newNumber = _unitOfWork.JournalEntryRepository.GetConsecutiveNumber(postingPeriodId);
            return newNumber;
        }

        public void CreateJournalEntry(JournalEntry journalEntry)
        {
            _unitOfWork.JournalEntryRepository.Add(journalEntry);
        }
        public void UpdateJournalEntry(JournalEntry journalEntry)
        {
            _unitOfWork.JournalEntryRepository.Update(journalEntry);
        }

        public IEnumerable<JournalEntryDeletedReport> GetAllJournalEntryDeleted(BasicReportParam reportParam)
        {
            return _unitOfWork.JournalEntryRepository.GetDeletedItemByDateRange(reportParam); 
        }

        public IEnumerable<JournalEntryLineDeletedReport> GetAllJournalEntryLineDeleted(BasicReportParam reportParam)
        {
            return  _unitOfWork.JournalEntryLineRepository.GetDeletedItemByDateRange(reportParam);
        }

        public void RestoreJournalEntry(JournalEntry journalEntry)
        {
            _unitOfWork.JournalEntryRepository.RestoreJournalEntry(journalEntry);
        }

        public void DeleteJournalEntry(JournalEntry journalEntry)
        {
            _unitOfWork.JournalEntryRepository.Remove(journalEntry);
        }

        #endregion

        #region Journal Entry Line
        public IEnumerable<JournalEntryLine> GetJournalEntryLineByJournalEntryId(int journalEntryId)
        {
            var output = _unitOfWork.JournalEntryLineRepository
                                        .FindByJournalEntryId(journalEntryId);

            return output;
        }

        public JournalEntryLine GetJournalEntryLineById(int id)
        {
            var output = _unitOfWork.JournalEntryLineRepository.GetById(id);
            return output;
        }
        public void CreateJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            _unitOfWork.JournalEntryLineRepository.Add(journalEntryLine);
        }
        public void UpdateJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            _unitOfWork.JournalEntryLineRepository.Update(journalEntryLine);
        }

        public void RestoreJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            _unitOfWork.JournalEntryLineRepository.RestoreJournalEntryLine(journalEntryLine);
        }

        public void DeleteJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            _unitOfWork.JournalEntryLineRepository.Remove(journalEntryLine);
        }

        #endregion

    }
}













