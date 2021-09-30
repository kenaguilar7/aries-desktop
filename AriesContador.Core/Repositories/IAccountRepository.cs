using AriesContador.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Account GetById(int id); 
        IEnumerable<Account> FindByCompanyId(string companyId);
        IEnumerable<Account> GetDefaultAccounts();
        void UpdatePartlyAccount(Account account); 
        IEnumerable<Account> AccountsWithBalanceByDateRange(BasicReportParam reportParam);
        bool HasMovements(int accountId, string companyId); 
    }
}
