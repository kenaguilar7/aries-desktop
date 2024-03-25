using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task AddAsync(Company company)
        {
            var accounts = company.Account;
            company.Account = null;
            //using (MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync   (_connectionString))
            //{
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            try
            {
                dataAccess.StartTransaction();
                dataAccess.SaveDataInTransaction<Company>("SP_InsertCompany", company);

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
            catch (Exception ex)
            {
                dataAccess.RollBackTransaction();
                throw ex;
            }
        }

        public void Add(Company entity)
        {
            var accounts = entity.Account;
            entity.Account = null;
            //using (MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync   (_connectionString))
            //{
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString); 
                try
                {
                    dataAccess.StartTransaction();
                    dataAccess.SaveDataInTransaction<Company>("SP_InsertCompany", entity);

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
                catch (Exception ex)
                {
                    dataAccess.RollBackTransaction();
                    throw ex;
                }

            //}

        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            var dataAccess = new MySqlDataAccessAsync(_connectionString);

            var lst1 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.JuridicPerson);
            var lst2 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.FisicPerson);
            lst1.AddRange(lst2);
            return await Task.FromResult(lst1); 
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
            dataAccess.SaveData<Company>("SP_UpdateCompany", entity);
        }
    }
}
