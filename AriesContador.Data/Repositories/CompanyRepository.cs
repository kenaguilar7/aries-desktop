using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IConnectionString _connectionString;
        public CompanyRepository(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Add(Company entity)
        {
            var accounts = entity.Account;
            entity.Account = null;
            using (MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString))
            {
                try
                {
                    dataAccess.StartTransaction();
                    dataAccess.SaveDataInTransaction("SP_InsertCompany", ToInsertParams(entity));

                    foreach (var account in accounts)
                    {
                        var oldId = account.Id;
                        var newID = dataAccess.SaveDataInTransaction<Account, int>("SP_InsertAccount", account);
                        account.Id = newID;

                        var childAccounts = (from acn in accounts
                                             where acn.FatherAccount == oldId
                                             select acn).ToList();

                        childAccounts.ForEach(x => x.FatherAccount = newID);
                    }
                }
                catch (Exception)
                {
                    dataAccess.RollBackTransaction();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            var dataAccess = new MySqlDataAccessAsync(_connectionString);

            var lst1 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.JuridicPerson);
            var lst2 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.FisicPerson);
            lst1.AddRange(lst2);
            return lst1;
        }

        public async Task<string> LatestCode()
        {
            string query = "SELECT c.company_id as Code FROM companies c ORDER BY c.company_id DESC LIMIT 1";
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            var output = await dataAccess.ExecuteQuery<Company>(query);
            return output.First().Code;
        }

        public async Task Remove(Company entity)
        {
            var query = @"
delete T2 from accounting_months T0 JOIN  
accounting_entries T1 ON T1.accounting_months_id = T0.accounting_months_id
JOIN transactions_accounting T2 ON T1.accounting_entry_id = T2.accounting_entry_id
where T0.company_id = @Code;

delete T1 from accounting_months T0 JOIN  
accounting_entries T1 ON T1.accounting_months_id = T0.accounting_months_id
where T0.company_id = @Code; 

delete T0 from posting_period_end_closing T0 where T0.company_id = @Code; 

delete T0 from accounting_months T0 where T0.company_id = @Code; 
  
delete from companies where company_id = @Code
";
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            await dataAccess.ExecuteSingle(query, entity);
        }

        public void Update(Company entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData("SP_UpdateCompany", ToUpdateParams(entity));
        }

        private static object ToInsertParams(Company entity)
        {
            return new
            {
                entity.Code,
                TypeId = (int)entity.IdType,
                entity.NumberId,
                entity.CompanyName,
                MoneyType = (int)entity.MoneyType,
                entity.Op1,
                entity.Op2,
                entity.Address,
                Website = entity.WebSite,
                entity.Mail,
                entity.PhoneNumber1,
                entity.PhoneNumber2,
                entity.Notes,
                UserId = entity.CreatedBy,
                IsActive = entity.Active
            };
        }

        private static object ToUpdateParams(Company entity)
        {
            return new
            {
                entity.Code,
                entity.CompanyName,
                MoneyType = (int)entity.MoneyType,
                entity.Op1,
                entity.Op2,
                entity.Address,
                Website = entity.WebSite,
                entity.Mail,
                entity.PhoneNumber1,
                entity.PhoneNumber2,
                entity.Notes,
                UserId = entity.CreatedBy,
                IsActive = entity.Active
            };
        }
    }
}
