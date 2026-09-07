using System;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using AriesContador.Data;
using AriesContador.Data.Repositories;
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
                await repo.AddAsync(new Company
                {
                    Code = code,
                    IdType = IdType.CEDULA_JURIDICA,
                    NumberId = numberId,
                    CompanyName = "Test NewCompanyId",
                    MoneyType = CurrencyTypeCompany.Dolares_y_Colones,
                    CreatedBy = 1,
                    Active = true
                });

                var found = await repo.GetAllAsync();
                Assert.Contains(found, c => c.Code == code);
            }
            finally
            {
                await repo.RemoveAsync(new Company { Code = code });
            }
        }

        private sealed class TestConnectionString : IConnectionString
        {
            public string MySQLDefault => MySqlTestConnection.ConnectionString;
        }
    }
}
