using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private const string CloneChartFailedMessage = "No se pudo clonar el maestro de cuentas";

        private readonly IConnectionString _connectionString;
        public CompanyRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Company entity, CancellationToken cancellationToken = default)
        {
            var accounts = (entity.Account ?? Enumerable.Empty<Account>()).ToList();
            entity.Account = null;
            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                try
                {
                    await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                    await dataAccess.SaveDataInTransactionAsync("SP_InsertCompany", ToInsertCommandParams(entity), cancellationToken)
                        .ConfigureAwait(false);

                    if (CopiesFromExistingCompany(entity.CopyFrom))
                    {
                        await dataAccess.SaveDataInTransactionAsync("SP_CopyChartOfAccounts", new
                        {
                            FromCompany = entity.CopyFrom,
                            ToCompany = entity.Code,
                            UpdatedBy = entity.CreatedBy
                        }, cancellationToken).ConfigureAwait(false);
                    }
                    else if (accounts.Count > 0)
                    {
                        await InsertChartFromMemoryAsync(dataAccess, entity, accounts, cancellationToken).ConfigureAwait(false);
                    }

                    await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                    if (IsChartCloneFailure(ex))
                        throw new InvalidOperationException(CloneChartFailedMessage, ex);
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var lst1 = await dataAccess.ExecuteQueryAsync<Company>(Query.Query.AdministrationQuery.JuridicPerson, cancellationToken)
                .ConfigureAwait(false);
            var lst2 = await dataAccess.ExecuteQueryAsync<Company>(Query.Query.AdministrationQuery.FisicPerson, cancellationToken)
                .ConfigureAwait(false);
            lst1.AddRange(lst2);
            return lst1;
        }

        public async Task<string> LatestCodeAsync(CancellationToken cancellationToken = default)
        {
            const string query = "SELECT c.company_id as Code FROM companies c ORDER BY c.company_id DESC LIMIT 1";
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.ExecuteQueryAsync<Company>(query, cancellationToken).ConfigureAwait(false);
            if (output == null || output.Count == 0 || string.IsNullOrEmpty(output.First().Code))
                return "C000";
            return output.First().Code;
        }

        public async Task<IEnumerable<string>> GetCodesAllowedForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Company, object>(
                Query.Query.AdministrationQuery.CompanyCodesAllowedForUser,
                new { UserId = userId },
                cancellationToken).ConfigureAwait(false);
            return rows.Select(x => x.Code);
        }

        public async Task RemoveAsync(Company entity, CancellationToken cancellationToken = default)
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

UPDATE accounts SET father_account = NULL WHERE company_id = @Code;
DELETE FROM accounts WHERE company_id = @Code;
  
delete from companies where company_id = @Code
";
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteSingleAsync(query, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Company entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_UpdateCompany", ToUpdateParams(entity), cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task InsertChartFromMemoryAsync(
            MySqlDataAccess dataAccess,
            Company entity,
            IReadOnlyList<Account> accounts,
            CancellationToken cancellationToken)
        {
            await dataAccess.ExecuteTextInTransactionAsync(@"
DROP TEMPORARY TABLE IF EXISTS tmp_aries_chart;
CREATE TEMPORARY TABLE tmp_aries_chart (
  seq INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  old_id INT NOT NULL,
  father_old INT NULL,
  name VARCHAR(50) NOT NULL,
  account_tag INT NOT NULL,
  account_guide INT NOT NULL,
  editable TINYINT(1) NOT NULL,
  memo VARCHAR(50) NULL,
  updated_by INT NULL
) ENGINE=MEMORY;", null, cancellationToken).ConfigureAwait(false);

            var sql = new StringBuilder();
            sql.Append("INSERT INTO tmp_aries_chart (old_id, father_old, name, account_tag, account_guide, editable, memo, updated_by) VALUES ");
            var args = new DynamicParameters();
            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts[i];
                if (string.IsNullOrWhiteSpace(account.Name))
                    throw new InvalidOperationException("La cuenta no tiene nombre");

                if (i > 0)
                    sql.Append(',');
                sql.Append("(@old").Append(i).Append(", @father").Append(i)
                    .Append(", @name").Append(i).Append(", @tag").Append(i)
                    .Append(", @guide").Append(i).Append(", @edit").Append(i)
                    .Append(", @memo").Append(i).Append(", @user").Append(i).Append(')');

                args.Add("old" + i, account.Id);
                args.Add("father" + i, account.FatherAccount);
                args.Add("name" + i, account.Name);
                args.Add("tag" + i, (int)account.AccountTag);
                args.Add("guide" + i, (int)account.AccountType);
                args.Add("edit" + i, account.Editable);
                args.Add("memo" + i, account.Memo);
                args.Add("user" + i, account.UpdatedBy != 0 ? account.UpdatedBy : entity.CreatedBy);
            }

            await dataAccess.ExecuteTextInTransactionAsync(sql.ToString(), args, cancellationToken).ConfigureAwait(false);
            await dataAccess.SaveDataInTransactionAsync("SP_InsertChartFromTemp", new { ToCompany = entity.Code }, cancellationToken)
                .ConfigureAwait(false);
        }

        private static bool CopiesFromExistingCompany(string copyFrom)
        {
            return !string.IsNullOrWhiteSpace(copyFrom) && copyFrom != "POR DEFECTO";
        }

        private static bool IsChartCloneFailure(Exception ex)
        {
            while (ex != null)
            {
                if (ex.Message != null &&
                    ex.Message.IndexOf(CloneChartFailedMessage, StringComparison.Ordinal) >= 0)
                    return true;
                ex = ex.InnerException;
            }
            return false;
        }

        private static DynamicParameters ToInsertCommandParams(Company entity)
        {
            var parameters = new DynamicParameters(ToInsertParams(entity));
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
