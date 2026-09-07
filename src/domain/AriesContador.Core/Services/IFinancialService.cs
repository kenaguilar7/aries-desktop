using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Services
{
    public interface IFinancialService
    {
        #region Account
        Task DeleteAccountAsync(Account account, CancellationToken cancellationToken = default);
        Task<Account> FindAccountAsync(int id, CancellationToken cancellationToken = default);
        Task<Account> GetAccountBalanceAsync(Account account, IEnumerable<PostingPeriod> postingPeriods, CancellationToken cancellationToken = default);
        IEnumerable<Account> GetDefaultAccounts();
        Task CreateAccountAsync(Account account, CancellationToken cancellationToken = default);
        Task CreateAccountAsync(Account account, Account parent, CancellationToken cancellationToken = default);
        Task UpdateAccountAsync(Account account, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> GetAccountsAsync(string companyId, CancellationToken cancellationToken = default);
        Task<(bool CanProceed, string Message)> EvaluateParentForNewChildAsync(Account parent, CancellationToken cancellationToken = default);
        Task FillAccountsWithBalancesAsync(IList<Account> accounts, DateTime from, DateTime to, CancellationToken cancellationToken = default);
        #endregion

        #region Posting Periods
        Task CreatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default);
        Task DeletePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostingPeriod>> GetPostingPeriodsAsync(string companyId, CancellationToken cancellationToken = default);
        Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreatedAsync(string companyId, CancellationToken cancellationToken = default);
        Task UpdatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default);
        Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default);
        #endregion

        #region Journal Entry
        Task<int> CreateJournalEntryConsecutiveAsync(int postingPeriodId, CancellationToken cancellationToken = default);
        Task DeleteJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId, CancellationToken cancellationToken = default);
        Task<JournalEntry> GetJournalEntryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
        Task UpdateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task RestoreJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
        Task UpdatedJournalEntryPeriodAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
        #endregion

        #region JournalEntryLine
        Task<JournalEntryLine> GetJournalEntryLineByIdAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default);
        Task CreateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default);
        Task UpdateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default);
        Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task RestoreJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default);
        #endregion
    }
}
