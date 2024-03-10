using AriesContador.Core.Models;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Services
{
    public interface IHttpFinancialService
    {
        Task ClosePostingPeriod(PostingPeriodEndClosing postingPeriod);
        Task CreateAccount(Account account);
        Task<int> CreateJournalEntry(JournalEntry journalEntry);
        Task<int> CreateJournalEntryConsecutive(int postingPeriodId);
        Task<int> CreateJournalEntryLine(JournalEntryLine journalEntryLine);
        Task CreatePostingPeriod(PostingPeriod postingPeriod);
        Task DeleteAccount(Account account);
        Task DeleteJournalEntry(JournalEntry journalEntry);
        Task DeleteJournalEntryLine(JournalEntryLine journalEntryLine);
        Task DeletePostingPeriod(PostingPeriod postingPeriod);
        Task<Account> FindAccount(int id);
        Task<Account> GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods);
        Task<IEnumerable<Account>> GetAccounts(string companyId);
        Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeleted(BasicReportParam reportParam);
        Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeleted(BasicReportParam reportParam);
        Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreated(string companyId);
        Task<IEnumerable<Account>> GetDefaultAccounts();
        Task<IEnumerable<JournalEntry>> GetJournalEntries(int postingPeriodId);
        Task<JournalEntry> GetJournalEntryById(int id);
        Task<JournalEntryLine> GetJournalEntryLineById(int id);
        Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryId(int journalEntryId);
        Task<IEnumerable<PostingPeriod>> GetPostingPeriods(string companyId);
        Task RestoreJournalEntry(JournalEntry journalEntry);
        Task RestoreJournalEntryLine(JournalEntryLine journalEntryLine);
        Task UpdateAccount(Account account);
        Task UpdatedJournalEntryPeriod(JournalEntry journalEntry);
        Task UpdateJournalEntry(JournalEntry journalEntry);
        Task UpdateJournalEntryLine(JournalEntryLine journalEntryLine);
        Task UpdatePostingPeriod(PostingPeriod postingPeriod);
    }

    public class HttpFinancialService : IHttpFinancialService
    {
        private readonly IHttpClientService _httpClientService;
        private string _baseUrl;

        public HttpFinancialService(IHttpClientService httpClientService)
        {

            _baseUrl = "https://localhost:44320/";
            this._httpClientService = httpClientService;
        }

        public Task ClosePostingPeriod(PostingPeriodEndClosing postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task CreateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateJournalEntry(JournalEntry journalEntry)
        {
            try
            {

                return await _httpClientService
                    .PostAsync<int,JournalEntry>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"CreateJournalEntry"), journalEntry);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<int> CreateJournalEntryConsecutive(int postingPeriodId)
        {
            try
            {

                var response = await _httpClientService
                    .GetAsync<int>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"JournalEntry/GetConsecutiveNumber/{postingPeriodId}"));
                return response;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> CreateJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            try
            {
                return await _httpClientService
                    .PostAsync<int, JournalEntryLine>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"JournalEntryLine/CreateJournalEntryLine"), journalEntryLine);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public Task CreatePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public Task DeleteJournalEntry(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task DeleteJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task<Account> FindAccount(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetAccounts(string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeleted(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeleted(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreated(string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetDefaultAccounts()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntries(int postingPeriodId)
        {
            try
            {

                var response = await _httpClientService
                    .GetAsync<List<JournalEntry>>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"JournalEntry/GetJournalEntries/{postingPeriodId}"));
                return response;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public Task<JournalEntry> GetJournalEntryById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<JournalEntryLine> GetJournalEntryLineById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryId(int journalEntryId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PostingPeriod>> GetPostingPeriods(string companyId)
        {
            try
            {

                var response = await _httpClientService
                    .GetAsync<List<PostingPeriod>>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"PostingPeriod/GetPostingPeriods/{companyId}"));
                return response;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public Task RestoreJournalEntry(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task RestoreJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public Task UpdatedJournalEntryPeriod(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJournalEntry(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }
    }
}
