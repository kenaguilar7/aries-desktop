using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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
            var accounts = entity.Account ?? Enumerable.Empty<Account>();
            entity.Account = null;
            using (MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString))
            {
                try
                {
                    dataAccess.StartTransaction();
                    dataAccess.SaveDataInTransaction("SP_InsertCompany", ToInsertCommandParams(entity));

                    foreach (var account in accounts)
                    {
                        var oldId = account.Id;
                        account.Name = ResolveAccountNameId(dataAccess, account.Name);
                        var newID = dataAccess.SaveDataInTransaction<Account, int>("SP_InsertAccount", account);
                        account.Id = newID;

                        var childAccounts = (from acn in accounts
                                             where acn.FatherAccount == oldId
                                             select acn).ToList();

                        childAccounts.ForEach(x => x.FatherAccount = newID);
                    }

                    dataAccess.CommitTransaction();
                }
                catch (Exception)
                {
                    dataAccess.RollBackTransaction();
                    throw;
                }
            }
        }

        public Task AddAsync(Company entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            var dataAccess = new MySqlDataAccessAsync(_connectionString);

            var lst1 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.JuridicPerson);
            var lst2 = await dataAccess.ExecuteQuery<Company>(Query.Query.AdministrationQuery.FisicPerson);
            lst1.AddRange(lst2);
            return lst1;
        }

        public IEnumerable<Company> GetAllBlocking()
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var lst1 = dataAccess.ExecuteQuery<Company, object>(Query.Query.AdministrationQuery.JuridicPerson, new { });
            var lst2 = dataAccess.ExecuteQuery<Company, object>(Query.Query.AdministrationQuery.FisicPerson, new { });
            lst1.AddRange(lst2);
            return lst1;
        }

        public async Task<string> LatestCode()
        {
            string query = "SELECT c.company_id as Code FROM companies c ORDER BY c.company_id DESC LIMIT 1";
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            var output = await dataAccess.ExecuteQuery<Company>(query);
            if (output == null || output.Count == 0 || string.IsNullOrEmpty(output.First().Code))
                return "C000";
            return output.First().Code;
        }

        public async Task<IEnumerable<string>> GetCodesAllowedForUser(int userId)
        {
            var dataAccess = new MySqlDataAccessAsync(_connectionString);
            var rows = await dataAccess.ExecuteQuery<Company, object>(
                Query.Query.AdministrationQuery.CompanyCodesAllowedForUser,
                new { UserId = userId });
            return rows.Select(x => x.Code);
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

        private static string ResolveAccountNameId(MySqlDataAccess dataAccess, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("La cuenta no tiene nombre");

            var nameId = dataAccess.SaveDataInTransaction<object, int>(
                "SP_GetOrCreateAccountName",
                new { AccountName = name });
            return nameId.ToString();
        }

        private static DynamicParameters ToInsertCommandParams(Company entity)
        {
            var parameters = new DynamicParameters(ToInsertParams(entity));
            // MySqlConnector copies OUT values after CALL; without this it throws
            // "Parameter 'NewCompanyId' not found in the collection".
            parameters.Add("@NewCompanyId", dbType: DbType.String, size: 5, direction: ParameterDirection.Output);
            return parameters;
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
