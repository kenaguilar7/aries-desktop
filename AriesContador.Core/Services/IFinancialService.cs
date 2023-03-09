using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;

namespace AriesContador.Core.Services
{
    public interface IFinancialService
    {
        #region Account
        void DeleteAccount(Account account);
        Account FindAccount(int id);
        Account GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods);
        IEnumerable<Account> GetDefaultAccounts();
        void CreateAccount(Account account);
        void UpdateAccount(Account account);
        IEnumerable<Account> GetAccounts(string companyId);
        #endregion

        #region Posting Periods
        void CreatePostingPeriod(PostingPeriod postingPeriod);
        void DeletePostingPeriod(PostingPeriod postingPeriod);
        IEnumerable<PostingPeriod> GetPostingPeriods(string companyId);
        List<PostingPeriod> GetAvailablePostingPeriodsForBeCreated(string companyId); 
        void UpdatePostingPeriod(PostingPeriod postingPeriod);
        void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod);

        #endregion

        #region Journal Entry
        int CreateJournalEntryConsecutive(int postingPeriodId); 
        void DeleteJournalEntry(JournalEntry journalEntry);
        IEnumerable<JournalEntry> GetJournalEntries(int postingPeriodId);
        JournalEntry GetJournalEntryById(int id);
        void CreateJournalEntry(JournalEntry journalEntry);
        void UpdateJournalEntry(JournalEntry journalEntry);
        IEnumerable<JournalEntryDeletedReport> GetAllJournalEntryDeleted(BasicReportParam reportParam);
        void RestoreJournalEntry(JournalEntry journalEntry);
        void UpdatedJournalEntryPeriod(JournalEntry journalEntry); 

        #endregion

        #region JournalEntryLine
        JournalEntryLine GetJournalEntryLineById(int id);
        void DeleteJournalEntryLine(JournalEntryLine journalEntryLine);
        IEnumerable<JournalEntryLine> GetJournalEntryLineByJournalEntryId(int journalEntryId);
        void CreateJournalEntryLine(JournalEntryLine journalEntryLine);
        void UpdateJournalEntryLine(JournalEntryLine journalEntryLine);
        IEnumerable<JournalEntryLineDeletedReport> GetAllJournalEntryLineDeleted(BasicReportParam reportParam);
        void RestoreJournalEntryLine(JournalEntryLine journalEntryLine); 

        #endregion
    }
}
