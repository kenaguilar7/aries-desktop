using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        IEnumerable<Account> GetDefaultAccounts();
        Task<IEnumerable<Account>> AccountsWithBalanceByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default);
        Task AddChildAsync(Account entity, CancellationToken cancellationToken = default);
        Task<bool> NameTakenAsync(int accountId, string companyId, string name, CancellationToken cancellationToken = default);
        Task<bool> HasOpenPeriodMovementsAsync(int accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> GetBalancesFromAccountInfoAsync(string companyId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task UpdateNameInfoAsync(Account entity, CancellationToken cancellationToken = default);
        Task<DataTable> GetMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default);
    }
}
