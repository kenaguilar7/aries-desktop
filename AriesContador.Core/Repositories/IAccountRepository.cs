using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
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
    }
}
