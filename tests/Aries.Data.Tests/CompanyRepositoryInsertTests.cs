using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using AriesContador.Data;
using AriesContador.Data.Repositories;
using MySql.Data.MySqlClient;
using Xunit;

namespace Aries.Data.Tests
{
    [Collection("mysql-schema")]
    public class CompanyRepositoryInsertTests
    {
        [MySqlFact]
        public async System.Threading.Tasks.Task Add_sends_NewCompanyId_out_parameter()
        {
            var code = "ZT01";
            var numberId = "ZT-" + DateTime.UtcNow.ToString("HHmmssfff");
            var repo = new CompanyRepository(new TestConnectionString());

            try
            {
                await repo.AddAsync(NewCompany(code, numberId));

                var found = await repo.GetAllAsync();
                Assert.Contains(found, c => c.Code == code);
            }
            finally
            {
                await repo.RemoveAsync(new Company { Code = code });
            }
        }

        [MySqlFact]
        public async System.Threading.Tasks.Task Add_por_defecto_inserts_57_accounts_with_tree()
        {
            var code = "ZT11";
            var repo = new CompanyRepository(new TestConnectionString());
            var accountsRepo = new AccountRepository(new TestConnectionString());

            try
            {
                await repo.AddAsync(NewCompany(code, UniqueNumberId("ZT11"), accounts: DefaultChart()));

                var accounts = (await accountsRepo.FindByCompanyIdAsync(code)).ToList();
                Assert.Equal(DefaultChartOfAccounts.AccountCount, accounts.Count);

                var activo = Assert.Single(accounts, a => a.Name == "ACTIVO");
                var corriente = Assert.Single(accounts, a => a.Name == "ACTIVO CORRIENTE");
                Assert.Equal(activo.Id, corriente.FatherAccount);

                var ingresoTitulo = Assert.Single(accounts, a =>
                    a.Name == "INGRESO" && a.AccountType == AccountType.Cuenta_Titulo);
                var ingresoMayor = Assert.Single(accounts, a =>
                    a.Name == "INGRESO" && a.AccountType == AccountType.Cuenta_De_Mayor);
                Assert.Equal(ingresoTitulo.Id, ingresoMayor.FatherAccount);
            }
            finally
            {
                await repo.RemoveAsync(new Company { Code = code });
            }
        }

        [MySqlFact]
        public async System.Threading.Tasks.Task Add_copying_company_preserves_tree_and_enums()
        {
            var sourceCode = "ZT12";
            var destCode = "ZT13";
            var repo = new CompanyRepository(new TestConnectionString());
            var accountsRepo = new AccountRepository(new TestConnectionString());

            try
            {
                await repo.AddAsync(NewCompany(sourceCode, UniqueNumberId("ZT12"), accounts: DefaultChart()));
                await repo.AddAsync(NewCompany(destCode, UniqueNumberId("ZT13"), copyFrom: sourceCode));

                var source = (await accountsRepo.FindByCompanyIdAsync(sourceCode)).ToList();
                var dest = (await accountsRepo.FindByCompanyIdAsync(destCode)).ToList();
                Assert.Equal(source.Count, dest.Count);

                var sourceRows = await LoadChartRowsAsync(sourceCode);
                var destRows = await LoadChartRowsAsync(destCode);
                Assert.Equal(sourceRows, destRows);

                var ingresoTitulo = Assert.Single(dest, a =>
                    a.Name == "INGRESO" && a.AccountType == AccountType.Cuenta_Titulo);
                var ingresoMayor = Assert.Single(dest, a =>
                    a.Name == "INGRESO" && a.AccountType == AccountType.Cuenta_De_Mayor);
                Assert.Equal(ingresoTitulo.Id, ingresoMayor.FatherAccount);
            }
            finally
            {
                await repo.RemoveAsync(new Company { Code = destCode });
                await repo.RemoveAsync(new Company { Code = sourceCode });
            }
        }

        [MySqlFact]
        public async System.Threading.Tasks.Task Add_copying_empty_company_rolls_back()
        {
            var emptyCode = "ZT14";
            var destCode = "ZT15";
            var repo = new CompanyRepository(new TestConnectionString());

            try
            {
                await repo.AddAsync(NewCompany(emptyCode, UniqueNumberId("ZT14")));

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    repo.AddAsync(NewCompany(destCode, UniqueNumberId("ZT15"), copyFrom: emptyCode)));

                Assert.Contains("clonar el maestro de cuentas", ex.Message);

                var found = await repo.GetAllAsync();
                Assert.DoesNotContain(found, c => c.Code == destCode);
                Assert.Contains(found, c => c.Code == emptyCode);
            }
            finally
            {
                await repo.RemoveAsync(new Company { Code = destCode });
                await repo.RemoveAsync(new Company { Code = emptyCode });
            }
        }

        private static Company NewCompany(
            string code,
            string numberId,
            string copyFrom = null,
            IEnumerable<Account> accounts = null)
        {
            return new Company
            {
                Code = code,
                IdType = IdType.CEDULA_JURIDICA,
                NumberId = numberId,
                CompanyName = "Test " + code,
                MoneyType = CurrencyTypeCompany.Dolares_y_Colones,
                CreatedBy = 1,
                Active = true,
                CopyFrom = copyFrom,
                Account = accounts
            };
        }

        private static List<Account> DefaultChart()
        {
            var accounts = DefaultChartOfAccounts.Create().ToList();
            foreach (var account in accounts)
                account.UpdatedBy = 1;
            return accounts;
        }

        private static string UniqueNumberId(string prefix)
        {
            return prefix + "-" + DateTime.UtcNow.ToString("HHmmssfff");
        }

        private static async System.Threading.Tasks.Task<List<ChartRow>> LoadChartRowsAsync(string companyId)
        {
            var rows = new List<ChartRow>();
            using (var connection = new MySqlConnection(MySqlTestConnection.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
SELECT
  n.name AS Name,
  a.account_type + 0 AS AccountType,
  a.account_guide + 0 AS AccountGuide,
  pn.name AS FatherName
FROM accounts a
JOIN accounts_names n ON n.account_name_id = a.account_name_id
LEFT JOIN accounts p ON p.account_id = a.father_account
LEFT JOIN accounts_names pn ON pn.account_name_id = p.account_name_id
WHERE a.company_id = @code AND a.active = 1
ORDER BY n.name, a.account_guide + 0, IFNULL(pn.name, '')";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@code";
                    parameter.Value = companyId;
                    command.Parameters.Add(parameter);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new ChartRow
                            {
                                Name = Convert.ToString(reader.GetValue(0)),
                                AccountType = Convert.ToInt32(reader.GetValue(1)),
                                AccountGuide = Convert.ToInt32(reader.GetValue(2)),
                                FatherName = reader.IsDBNull(3) ? null : Convert.ToString(reader.GetValue(3))
                            });
                        }
                    }
                }
            }

            return rows;
        }

        private sealed class ChartRow
        {
            public string Name { get; set; }
            public int AccountType { get; set; }
            public int AccountGuide { get; set; }
            public string FatherName { get; set; }

            public override bool Equals(object obj)
            {
                return obj is ChartRow other
                    && Name == other.Name
                    && AccountType == other.AccountType
                    && AccountGuide == other.AccountGuide
                    && FatherName == other.FatherName;
            }

            public override int GetHashCode()
            {
                return (Name, AccountType, AccountGuide, FatherName).GetHashCode();
            }
        }

        private sealed class TestConnectionString : IConnectionString
        {
            public string MySQLDefault => MySqlTestConnection.ConnectionString;
        }
    }
}
