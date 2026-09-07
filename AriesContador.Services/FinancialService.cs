using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Services;
using AriesContador.Core.Models.JournalEntries;

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
            if (companyId == "POR DEFECTO")
                return _unitOfWork.AccountRepository.GetDefaultAccounts();

            var output = _unitOfWork.AccountRepository.FindByCompanyId(companyId);
            return AccountRules.OrderByTree(output);
        }

        public Account FindAccount(int id)
        {
            return _unitOfWork.AccountRepository.GetById(id).GetAwaiter().GetResult();
        }

        public IEnumerable<Account> GetDefaultAccounts()
        {
            var accounts = _unitOfWork.AccountRepository.GetDefaultAccounts();
            return accounts;
        }

        public void CreateAccount(Account account)
        {
            Account parent = null;
            if (account != null && account.FatherAccount.HasValue && account.FatherAccount.Value != 0
                && !string.IsNullOrEmpty(account.CompanyId))
            {
                parent = _unitOfWork.AccountRepository.FindByCompanyId(account.CompanyId)
                    .FirstOrDefault(x => x.Id == account.FatherAccount.Value);
            }

            CreateAccount(account, parent);
        }

        public void CreateAccount(Account account, Account parent)
        {
            if (account == null)
                throw new InvalidOperationException(AccountRules.BlankNameMessage);

            if (!AccountRules.ValidateName(account.Name, out var nameMessage))
                throw new InvalidOperationException(nameMessage);

            if (_unitOfWork.AccountRepository.NameTaken(account.Id, account.CompanyId, account.Name))
                throw new InvalidOperationException(AccountRules.NameCannotBeUsedMessage);

            account.AccountType = AccountType.Cuenta_Auxiliar;
            var parentWasAuxiliar = parent != null && parent.AccountType == AccountType.Cuenta_Auxiliar;
            if (parent != null)
            {
                account.FatherAccount = parent.Id;
                account.AccountTag = parent.AccountTag;
                if (string.IsNullOrEmpty(account.CompanyId))
                    account.CompanyId = parent.CompanyId;
                AccountRules.InheritBalancesIfParentIsAuxiliar(account, parent);
            }

            _unitOfWork.AccountRepository.AddChild(account);

            if (parentWasAuxiliar)
                parent.AccountType = AccountType.Cuenta_De_Mayor;
        }

        public void UpdateAccount(Account account)
        {
            if (account == null)
                throw new InvalidOperationException(AccountRules.BlankNameMessage);

            if (!AccountRules.ValidateName(account.Name, out var nameMessage))
                throw new InvalidOperationException(nameMessage);

            if (_unitOfWork.AccountRepository.NameTaken(account.Id, account.CompanyId, account.Name))
                throw new InvalidOperationException(AccountRules.NameTakenMessage);

            _unitOfWork.AccountRepository.UpdateNameInfo(account);
        }

        public void DeleteAccount(Account account)
        {
            if (!AccountRules.CanDelete(account, out var message))
                throw new InvalidOperationException(message);

            if (_unitOfWork.AccountRepository.HasOpenPeriodMovements(account.Id))
                throw new InvalidOperationException(AccountRules.DeleteWithMovementsMessage);

            _unitOfWork.AccountRepository.Remove(account).GetAwaiter().GetResult();
        }

        public bool EvaluateParentForNewChild(Account parent, out string message)
        {
            message = "";
            if (parent == null)
                return true;

            var periods = GetPostingPeriods(parent.CompanyId).ToList();
            if (periods.Count == 0)
                return true;

            var dummy = CloneAccountBalances(parent);
            FillAccountsWithBalances(new List<Account> { dummy }, periods[0].Date, periods[periods.Count - 1].Date);

            if (dummy.AccountType == AccountType.Cuenta_Auxiliar && AccountRules.HasMovement(dummy))
            {
                message = AccountRules.ParentHasMovementsWarning(dummy);
                return false;
            }

            return true;
        }

        public void FillAccountsWithBalances(IList<Account> accounts, DateTime from, DateTime to)
        {
            if (accounts == null || accounts.Count == 0)
                return;

            foreach (var account in accounts)
            {
                account.DebitBalance = 0m;
                account.CreditBalance = 0m;
                account.DebitBalanceForeign = 0m;
                account.CreditBalanceForeign = 0m;
            }

            var companyId = accounts.Select(a => a.CompanyId).FirstOrDefault(id => !string.IsNullOrEmpty(id));
            if (string.IsNullOrEmpty(companyId))
                return;

            var rows = _unitOfWork.AccountRepository.GetBalancesFromAccountInfo(companyId, from, to);
            var byId = rows.ToDictionary(x => x.Id);
            foreach (var account in accounts)
            {
                if (!byId.TryGetValue(account.Id, out var row))
                    continue;
                account.DebitBalance = row.DebitBalance;
                account.CreditBalance = row.CreditBalance;
                account.DebitBalanceForeign = row.DebitBalanceForeign;
                account.CreditBalanceForeign = row.CreditBalanceForeign;
            }

            AccountRules.ApplyRollUp(accounts);
        }

        public IEnumerable<Account> GetAccountBalance(string companyId, IEnumerable<PostingPeriod> postingPeriods)
        {
            throw new NotImplementedException();
        }

        public Account GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods)
        {
            throw new NotImplementedException();
        }

        private static Account CloneAccountBalances(Account source)
        {
            return new Account
            {
                Id = source.Id,
                Name = source.Name,
                CompanyId = source.CompanyId,
                FatherAccount = source.FatherAccount,
                AccountType = source.AccountType,
                AccountTag = source.AccountTag,
                Editable = source.Editable,
                PriorBalance = source.PriorBalance,
                PriorBalanceForeign = source.PriorBalanceForeign,
                DebitBalance = source.DebitBalance,
                CreditBalance = source.CreditBalance,
                DebitBalanceForeign = source.DebitBalanceForeign,
                CreditBalanceForeign = source.CreditBalanceForeign
            };
        }

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

            if (postingPeriods.PeriodExist(postingPeriod))
                throw new Exception("Periodo contable con fechas repetidas");

            _unitOfWork.PostingPeriodRepository.Add(postingPeriod);
        }

        public void UpdatePostingPeriod(PostingPeriod postingPeriod)
        {
            _unitOfWork.PostingPeriodRepository.Update(postingPeriod);
        }
        public void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod)
        {
            _unitOfWork.PostingPeriodRepository.ClosePostingPeriod(postingPeriod);
        }
        public void DeletePostingPeriod(PostingPeriod postingPeriod)
        {
            _unitOfWork.PostingPeriodRepository.Remove(postingPeriod).GetAwaiter().GetResult();
        }

        public List<PostingPeriod> GetAvailablePostingPeriodsForBeCreated(string companyId)
        {
            var postingPeriods = GetPostingPeriods(companyId);
            var output = new List<PostingPeriod>();

            if (postingPeriods.Any())
            {
                var exitMovements = HasJournalEntries(postingPeriods);
                var pPeriods = new PostingPeriodCreator(postingPeriods.ToList(), exitMovements)
                    .GetAvailablePostingPeriodForBeCreated(); 

                output.AddRange(pPeriods);
            }
            else
            {
                var pPeriod = new PostingPeriodCreator().CreatePostingPeriodForNewCompany(); 
                output.Add(pPeriod);
            }

            return output; 
        }

        private bool HasJournalEntries(IEnumerable<PostingPeriod> postingPeriods)
        {

            foreach (var postingP in postingPeriods)
            {
                postingP.JournalEntries = _unitOfWork.JournalEntryRepository.FindByPostingPeriodId(postingP.Id).ToList();
            }

            var hasEntries = postingPeriods.Count(x => x.JournalEntries.Count > 0);

            return hasEntries > 0;
        }

        //private IEnumerable<PostingPeriod> CreatePreEntityPostingPeriod(DateTime fromDatePeriod)
        //{
        //    return new List<PostingPeriod>() { CreatePostingPeriodEntity(fromDatePeriod) };
        //}

        //private IEnumerable<PostingPeriod> CreatePreEntityPostingPeriod(DateTime fromDatePeriod, DateTime toDatePeriod)
        //{
        //    return new List<PostingPeriod>() { CreatePostingPeriodEntity(fromDatePeriod),
        //                                          CreatePostingPeriodEntity(toDatePeriod) };
        //}

        //public PostingPeriod CreatePostingPeriodEntity(DateTime PeriodDate)
        //     => new PostingPeriod() { Date = PeriodDate };

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
            _unitOfWork.JournalEntryRepository.Remove(journalEntry).GetAwaiter().GetResult();
        }

        public void UpdatedJournalEntryPeriod(JournalEntry journalEntry)
        {
            var newJournalEntryNumber = _unitOfWork.JournalEntryRepository.GetConsecutiveNumber(journalEntry.PostingPeriodId);
            journalEntry.Number = newJournalEntryNumber;
            _unitOfWork.JournalEntryRepository.Update(journalEntry);
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
            _unitOfWork.JournalEntryLineRepository.Remove(journalEntryLine).GetAwaiter().GetResult();
        }

        #endregion

    }
}













