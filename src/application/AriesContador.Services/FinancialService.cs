using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Account>> GetAccountsAsync(string companyId, CancellationToken cancellationToken = default)
        {
            if (companyId == "POR DEFECTO")
                return _unitOfWork.AccountRepository.GetDefaultAccounts();

            var output = await _unitOfWork.AccountRepository.FindByCompanyIdAsync(companyId, cancellationToken).ConfigureAwait(false);
            return AccountRules.OrderByTree(output);
        }

        public Task<Account> FindAccountAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.AccountRepository.GetByIdAsync(id, cancellationToken);
        }

        public IEnumerable<Account> GetDefaultAccounts()
        {
            return _unitOfWork.AccountRepository.GetDefaultAccounts();
        }

        public async Task CreateAccountAsync(Account account, CancellationToken cancellationToken = default)
        {
            Account parent = null;
            if (account != null && account.FatherAccount.HasValue && account.FatherAccount.Value != 0
                && !string.IsNullOrEmpty(account.CompanyId))
            {
                var accounts = await _unitOfWork.AccountRepository.FindByCompanyIdAsync(account.CompanyId, cancellationToken)
                    .ConfigureAwait(false);
                parent = accounts.FirstOrDefault(x => x.Id == account.FatherAccount.Value);
            }

            await CreateAccountAsync(account, parent, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateAccountAsync(Account account, Account parent, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new InvalidOperationException(AccountRules.BlankNameMessage);

            if (!AccountRules.ValidateName(account.Name, out var nameMessage))
                throw new InvalidOperationException(nameMessage);

            if (await _unitOfWork.AccountRepository.NameTakenAsync(account.Id, account.CompanyId, account.Name, cancellationToken).ConfigureAwait(false))
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

            await _unitOfWork.AccountRepository.AddChildAsync(account, cancellationToken).ConfigureAwait(false);

            if (parentWasAuxiliar)
                parent.AccountType = AccountType.Cuenta_De_Mayor;
        }

        public async Task UpdateAccountAsync(Account account, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new InvalidOperationException(AccountRules.BlankNameMessage);

            if (!AccountRules.ValidateName(account.Name, out var nameMessage))
                throw new InvalidOperationException(nameMessage);

            if (await _unitOfWork.AccountRepository.NameTakenAsync(account.Id, account.CompanyId, account.Name, cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException(AccountRules.NameTakenMessage);

            await _unitOfWork.AccountRepository.UpdateNameInfoAsync(account, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAccountAsync(Account account, CancellationToken cancellationToken = default)
        {
            if (!AccountRules.CanDelete(account, out var message))
                throw new InvalidOperationException(message);

            if (await _unitOfWork.AccountRepository.HasOpenPeriodMovementsAsync(account.Id, cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException(AccountRules.DeleteWithMovementsMessage);

            await _unitOfWork.AccountRepository.RemoveAsync(account, cancellationToken).ConfigureAwait(false);
        }

        public async Task<(bool CanProceed, string Message)> EvaluateParentForNewChildAsync(Account parent, CancellationToken cancellationToken = default)
        {
            if (parent == null)
                return (true, "");

            var periods = (await GetPostingPeriodsAsync(parent.CompanyId, cancellationToken).ConfigureAwait(false)).ToList();
            if (periods.Count == 0)
                return (true, "");

            var dummy = CloneAccountBalances(parent);
            await FillAccountsWithBalancesAsync(new List<Account> { dummy }, periods[0].Date, periods[periods.Count - 1].Date, cancellationToken)
                .ConfigureAwait(false);

            if (dummy.AccountType == AccountType.Cuenta_Auxiliar && AccountRules.HasMovement(dummy))
                return (false, AccountRules.ParentHasMovementsWarning(dummy));

            return (true, "");
        }

        public async Task FillAccountsWithBalancesAsync(IList<Account> accounts, DateTime from, DateTime to, CancellationToken cancellationToken = default)
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

            var rows = await _unitOfWork.AccountRepository.GetBalancesFromAccountInfoAsync(companyId, from, to, cancellationToken)
                .ConfigureAwait(false);
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

        public Task<Account> GetAccountBalanceAsync(Account account, IEnumerable<PostingPeriod> postingPeriods, CancellationToken cancellationToken = default)
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
        public async Task<IEnumerable<PostingPeriod>> GetPostingPeriodsAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var output = await _unitOfWork.PostingPeriodRepository.FindByCompanyIdAsync(companyId, cancellationToken).ConfigureAwait(false);
            return output.OrderBy(x => x.Date);
        }

        public async Task CreatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default)
        {
            var postingPeriods = await _unitOfWork.PostingPeriodRepository.FindByCompanyIdAsync(postingPeriod.CompanyId, cancellationToken)
                .ConfigureAwait(false);

            if (postingPeriods.PeriodExist(postingPeriod))
                throw new Exception("Periodo contable con fechas repetidas");

            await _unitOfWork.PostingPeriodRepository.AddAsync(postingPeriod, cancellationToken).ConfigureAwait(false);
        }

        public Task UpdatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PostingPeriodRepository.UpdateAsync(postingPeriod, cancellationToken);
        }

        public Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PostingPeriodRepository.ClosePostingPeriodAsync(postingPeriod, cancellationToken);
        }

        public Task DeletePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PostingPeriodRepository.RemoveAsync(postingPeriod, cancellationToken);
        }

        public async Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreatedAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var postingPeriods = await GetPostingPeriodsAsync(companyId, cancellationToken).ConfigureAwait(false);
            var output = new List<PostingPeriod>();

            if (postingPeriods.Any())
            {
                var exitMovements = await HasJournalEntriesAsync(postingPeriods, cancellationToken).ConfigureAwait(false);
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

        private async Task<bool> HasJournalEntriesAsync(IEnumerable<PostingPeriod> postingPeriods, CancellationToken cancellationToken)
        {
            foreach (var postingP in postingPeriods)
            {
                postingP.JournalEntries = (await _unitOfWork.JournalEntryRepository.FindByPostingPeriodIdAsync(postingP.Id, cancellationToken)
                    .ConfigureAwait(false)).ToList();
            }

            return postingPeriods.Count(x => x.JournalEntries.Count > 0) > 0;
        }

        #endregion

        #region Journal Entry
        public Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.FindByPostingPeriodIdAsync(postingPeriodId, cancellationToken);
        }

        public Task<JournalEntry> GetJournalEntryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<int> CreateJournalEntryConsecutiveAsync(int postingPeriodId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.GetConsecutiveNumberAsync(postingPeriodId, cancellationToken);
        }

        public Task CreateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.AddAsync(journalEntry, cancellationToken);
        }

        public Task UpdateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.UpdateAsync(journalEntry, cancellationToken);
        }

        public Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.GetDeletedItemByDateRangeAsync(reportParam, cancellationToken);
        }

        public Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.GetDeletedItemByDateRangeAsync(reportParam, cancellationToken);
        }

        public Task RestoreJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.RestoreJournalEntryAsync(journalEntry, cancellationToken);
        }

        public Task DeleteJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryRepository.RemoveAsync(journalEntry, cancellationToken);
        }

        public async Task UpdatedJournalEntryPeriodAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            var newJournalEntryNumber = await _unitOfWork.JournalEntryRepository.GetConsecutiveNumberAsync(journalEntry.PostingPeriodId, cancellationToken)
                .ConfigureAwait(false);
            journalEntry.Number = newJournalEntryNumber;
            await _unitOfWork.JournalEntryRepository.UpdateAsync(journalEntry, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Journal Entry Line
        public Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.FindByJournalEntryIdAsync(journalEntryId, cancellationToken);
        }

        public Task<JournalEntryLine> GetJournalEntryLineByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task CreateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.AddAsync(journalEntryLine, cancellationToken);
        }

        public Task UpdateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.UpdateAsync(journalEntryLine, cancellationToken);
        }

        public Task RestoreJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.RestoreJournalEntryLineAsync(journalEntryLine, cancellationToken);
        }

        public Task DeleteJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.JournalEntryLineRepository.RemoveAsync(journalEntryLine, cancellationToken);
        }

        #endregion
    }
}
