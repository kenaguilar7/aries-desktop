using AriesContador.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Account GetById(int id); 
        IEnumerable<Account> FindByCompanyId(string companyId);
        IEnumerable<Account> GetDefaultAccounts();
    }
}
