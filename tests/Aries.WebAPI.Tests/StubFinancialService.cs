using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubFinancialService : IFinancialService
    {
        public JournalEntry LastCreatedEntry { get; private set; }

        public Task DeleteAccountAsync(Account account, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Account> FindAccountAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Account { Id = id, Name = "Caja" });
        public Task<Account> GetAccountBalanceAsync(Account account, IEnumerable<PostingPeriod> postingPeriods, CancellationToken cancellationToken = default) =>
            Task.FromResult(account);
        public IEnumerable<Account> GetDefaultAccounts() => Array.Empty<Account>();
        public Task CreateAccountAsync(Account account, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CreateAccountAsync(Account account, Account parent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAccountAsync(Account account, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<Account>> GetAccountsAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Account>>(Array.Empty<Account>());
        public Task<(bool CanProceed, string Message)> EvaluateParentForNewChildAsync(Account parent, CancellationToken cancellationToken = default) =>
            Task.FromResult((true, (string)null));
        public Task FillAccountsWithBalancesAsync(IList<Account> accounts, DateTime from, DateTime to, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
        public Task CreatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeletePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<PostingPeriod>> GetPostingPeriodsAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<PostingPeriod>>(Array.Empty<PostingPeriod>());
        public Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreatedAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new List<PostingPeriod>());
        public Task UpdatePostingPeriodAsync(PostingPeriod postingPeriod, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CreateJournalEntryConsecutiveAsync(int postingPeriodId, CancellationToken cancellationToken = default) =>
            Task.FromResult(3);
        public Task DeleteJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(int postingPeriodId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntry>>(Array.Empty<JournalEntry>());
        public Task<JournalEntry> GetJournalEntryByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(new JournalEntry { Id = id });
        public Task CreateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default)
        {
            journalEntry.Id = 42;
            LastCreatedEntry = journalEntry;
            return Task.CompletedTask;
        }
        public Task UpdateJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryDeletedReport>>(Array.Empty<JournalEntryDeletedReport>());
        public Task RestoreJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdatedJournalEntryPeriodAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<JournalEntryLine> GetJournalEntryLineByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(new JournalEntryLine { Id = id });
        public Task DeleteJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryLine>>(Array.Empty<JournalEntryLine>());
        public Task CreateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default)
        {
            journalEntryLine.Id = 9;
            return Task.CompletedTask;
        }
        public Task UpdateJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeletedAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryLineDeletedReport>>(Array.Empty<JournalEntryLineDeletedReport>());
        public Task RestoreJournalEntryLineAsync(JournalEntryLine journalEntryLine, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
