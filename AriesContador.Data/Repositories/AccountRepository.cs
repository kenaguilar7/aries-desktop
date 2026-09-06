using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConnectionString _connectionString;
        public AccountRepository(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Add(Account entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = dataAccess.SaveData<Account, int>("SP_InsertAccount", entity);
        }

        public void AddChild(Account entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = dataAccess.SaveData<object, int>("SP_InsertChildAccount", ToChildInsertParams(entity));
        }

        public IEnumerable<Account> FindByCompanyId(string companyId)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<Account, dynamic>("SP_GetAccountsByCompanyId", new { CompanyId = companyId });
            return output;
        }

        public async Task<Account> GetById(int id)
        {
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            var output = await dataAccess.LoadData<Account, dynamic>("SP_GetAccountById", new { accountId = id });
            return output.FirstOrDefault();
        }

        public async Task Remove(Account entity)
        {
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            await dataAccess.SaveData<Account>("SP_DesactivateAccount", entity);
        }

        public void Update(Account entity)
        {
            UpdateNameInfo(entity);
        }

        public void UpdateNameInfo(Account entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData("SP_UpdateAccountNameInfo", new
            {
                entity.Id,
                entity.Name,
                entity.Memo,
                entity.CompanyId,
                entity.UpdatedBy
            });
        }

        public bool NameTaken(int accountId, string companyId, string name)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var rows = dataAccess.LoadData<FlagRow, dynamic>("SP_AccountNameTaken", new
            {
                AccountId = accountId,
                CompanyId = companyId,
                Name = name
            });
            return rows.FirstOrDefault()?.Taken == 1;
        }

        public bool HasOpenPeriodMovements(int accountId)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var rows = dataAccess.LoadData<FlagRow, dynamic>("SP_AccountHasOpenPeriodMovements", new
            {
                AccountId = accountId
            });
            return rows.FirstOrDefault()?.HasMovements == 1;
        }

        public IEnumerable<Account> GetBalancesFromAccountInfo(string companyId, DateTime from, DateTime to)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            return dataAccess.LoadData<Account, dynamic>("SP_GetAccountBalancesFromAccountInfo", new
            {
                CompanyId = companyId,
                FromPeriod = AccountRules.ToYearMonthKey(from),
                ToPeriod = AccountRules.ToYearMonthKey(to)
            });
        }

        public IEnumerable<Account> GetDefaultAccounts()
        {
            var jsonString = System.IO.File.ReadAllText("defaultaccounts.json");

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(List<Account>));
                List<Account> accounts = (List<Account>)deserializer.ReadObject(ms);
                return accounts;
            }
        }

        public IEnumerable<Account> AccountsWithBalanceByDateRange(BasicReportParam reportParam)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<Account, BasicReportParam>("SP_AuxiliaryAccountsWithBalanceByDateRange", reportParam);
            output.BuildAccountsBalance();
            return output.OrderByTree();
        }

        public Task AddAsync(Account entity)
        {
            throw new NotImplementedException();
        }

        private static object ToChildInsertParams(Account entity)
        {
            return new
            {
                entity.Name,
                entity.PriorBalance,
                entity.PriorBalanceForeign,
                FatherAccount = entity.FatherAccount ?? 0,
                entity.CompanyId,
                AccountType = (int)entity.AccountType,
                AccountTag = (int)entity.AccountTag,
                entity.Memo,
                Editable = entity.EditableMySql,
                entity.UpdatedBy
            };
        }

        private class FlagRow
        {
            public int Taken { get; set; }
            public int HasMovements { get; set; }
        }
    }
}
