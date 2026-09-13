using AriesContador.Core.Models.Accounts;

namespace Aries.Contabilidad.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsAsync(string companyId);
        Task<Account> FindAccountAsync(int accountId);
        Task<Account> GetAccountBalanceAsync(string companyId, int accountId, DateTime startMonth, DateTime endMonth);
    }
}
