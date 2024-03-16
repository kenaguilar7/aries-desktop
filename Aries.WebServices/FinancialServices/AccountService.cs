using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialServices
{
    public interface IAccountService
    {
        Task DeleteAccount(Account account);
        Task<Account> FindAccount(int id);
        Task<Account> GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods);
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

        public Task<Account> GetAccountBalance(Account account, IEnumerable<PostingPeriod> postingPeriods)
        {
            throw new System.NotImplementedException();
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
