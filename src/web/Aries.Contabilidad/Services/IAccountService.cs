using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Reports;

namespace Aries.Contabilidad.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsAsync(string companyId);
        Task<Account> FindAccountAsync(int accountId);
        Task<Account> GetAccountBalanceAsync(string companyId, int accountId, DateTime startMonth, DateTime endMonth);
        Task<EvaluateParentResult> EvaluateParentAsync(Account parent);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Account account);
        Task<List<AccountMovementRow>> GetAccountMovementsAsync(int accountId);
    }
}
