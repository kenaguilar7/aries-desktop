using System;
using System.Collections.Generic;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubFinancialService : IFinancialService
    {
        public JournalEntry LastCreatedEntry { get; private set; }

        public void DeleteAccount(Account account) { }
        public Account FindAccount(int id) => new Account { Id = id, Name = "Caja" };
        public Account GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods) => account;
        public IEnumerable<Account> GetDefaultAccounts() => Array.Empty<Account>();
        public void CreateAccount(Account account) { }
        public void CreateAccount(Account account, Account parent) { }
        public void UpdateAccount(Account account) { }
        public IEnumerable<Account> GetAccounts(string companyId) => Array.Empty<Account>();
        public bool EvaluateParentForNewChild(Account parent, out string message)
        {
            message = null;
            return true;
        }
        public void FillAccountsWithBalances(IList<Account> accounts, DateTime from, DateTime to) { }
        public void CreatePostingPeriod(PostingPeriod postingPeriod) { }
        public void DeletePostingPeriod(PostingPeriod postingPeriod) { }
        public IEnumerable<PostingPeriod> GetPostingPeriods(string companyId) => Array.Empty<PostingPeriod>();
        public List<PostingPeriod> GetAvailablePostingPeriodsForBeCreated(string companyId) =>
            new List<PostingPeriod>();
        public void UpdatePostingPeriod(PostingPeriod postingPeriod) { }
        public void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod) { }
        public int CreateJournalEntryConsecutive(int postingPeriodId) => 3;
        public void DeleteJournalEntry(JournalEntry journalEntry) { }
        public IEnumerable<JournalEntry> GetJournalEntries(int postingPeriodId) => Array.Empty<JournalEntry>();
        public JournalEntry GetJournalEntryById(int id) => new JournalEntry { Id = id };
        public void CreateJournalEntry(JournalEntry journalEntry)
        {
            journalEntry.Id = 42;
            LastCreatedEntry = journalEntry;
        }
        public void UpdateJournalEntry(JournalEntry journalEntry) { }
        public IEnumerable<JournalEntryDeletedReport> GetAllJournalEntryDeleted(BasicReportParam reportParam) =>
            Array.Empty<JournalEntryDeletedReport>();
        public void RestoreJournalEntry(JournalEntry journalEntry) { }
        public void UpdatedJournalEntryPeriod(JournalEntry journalEntry) { }
        public JournalEntryLine GetJournalEntryLineById(int id) => new JournalEntryLine { Id = id };
        public void DeleteJournalEntryLine(JournalEntryLine journalEntryLine) { }
        public IEnumerable<JournalEntryLine> GetJournalEntryLineByJournalEntryId(int journalEntryId) =>
            Array.Empty<JournalEntryLine>();
        public void CreateJournalEntryLine(JournalEntryLine journalEntryLine) => journalEntryLine.Id = 9;
        public void UpdateJournalEntryLine(JournalEntryLine journalEntryLine) { }
        public IEnumerable<JournalEntryLineDeletedReport> GetAllJournalEntryLineDeleted(BasicReportParam reportParam) =>
            Array.Empty<JournalEntryLineDeletedReport>();
        public void RestoreJournalEntryLine(JournalEntryLine journalEntryLine) { }
    }
}
