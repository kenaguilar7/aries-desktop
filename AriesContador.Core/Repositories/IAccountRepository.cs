using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetById(int id); 
        IEnumerable<Account> FindByCompanyId(string companyId);
        IEnumerable<Account> GetDefaultAccounts();
        IEnumerable<Account> AccountsWithBalanceByDateRange(BasicReportParam reportParam);
        void AddChild(Account entity);
        bool NameTaken(int accountId, string companyId, string name);
        bool HasOpenPeriodMovements(int accountId);
        IEnumerable<Account> GetBalancesFromAccountInfo(string companyId, DateTime from, DateTime to);
        void UpdateNameInfo(Account entity);
        DataTable GetMovementReport(int accountId, bool auxiliar);
    }
}
