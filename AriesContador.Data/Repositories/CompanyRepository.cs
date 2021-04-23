using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            }

        }

        public IEnumerable<Company> GetAll()
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<Company>("SP_GetCompanies");
            return output;
        }

        public string GetConsecutive()
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = (dataAccess.LoadData<string>("SP_GetLastCompanyCode")).First();

            var newCode = int.Parse(output.Substring(1, 3));
            newCode++;

            var code = "C" + (newCode).ToString("000");

            return code;

        }

        public void Remove(Company entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData<Company>("SP_DesactivateCompany", entity);
        }

        public void Update(Company entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData<Company>("SP_UpdateCompany", entity);
        }
    }
}
