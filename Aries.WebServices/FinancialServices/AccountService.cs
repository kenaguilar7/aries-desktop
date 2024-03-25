using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace Aries.WebServices.FinancialServices
{
    public interface IAccountService
    {
        Task DeleteAccount(Account account);
        Task<Account> FindAccount(int id);
        Task<IEnumerable<Account>> GetAccountsBalance(string companyId, PostingPeriod startLook, PostingPeriod endLook); 
        Task<IEnumerable<Account>> GetDefaultAccounts();
        Task CreateAccount(Account account);
        Task UpdateAccount(Account account);
        Task<IEnumerable<Account>> GetAccounts(string companyId);
    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task CreateAccount(Account account)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAccount(Account account)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Account> FindAccount(int id)
            => await _accountRepository.GetById(id); 

        public async Task<IEnumerable<Account>> GetAccountsBalance(string companyId, PostingPeriod startLook, PostingPeriod endLook)
        {
            return await _accountRepository.AccountsWithBalanceByDateRange(new BasicReportParam()
            {
                CompanyId = companyId,
                FirstDate = $"{startLook.Year}{String.Format("{0, 0:D2}", startLook.Month)}",
                EndDate = $"{endLook.Year}{String.Format("{0, 0:D2}", endLook.Month)}" 
            }); 
        }

        public Task<IEnumerable<Account>> GetAccounts(string companyId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetDefaultAccounts()
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAccount(Account account)
        {
            throw new System.NotImplementedException();
        }
    }
}
