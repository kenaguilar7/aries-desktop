using System.Linq;
using AriesContador.Core.Models.Accounts;
using Xunit;

namespace AriesContador.Tests.AccountTests
{
    public class DefaultChartOfAccountsTests
    {
        [Fact]
        public void Create_matches_legacy_generar_cuentas_default()
        {
            var accounts = DefaultChartOfAccounts.Create();

            Assert.Equal(57, accounts.Count);
            Assert.Equal(Enumerable.Range(1, 57), accounts.Select(a => a.Id));

            Assert.All(accounts.Take(6), a =>
            {
                Assert.Null(a.FatherAccount);
                Assert.Equal(AccountType.Cuenta_Titulo, a.AccountType);
            });

            Assert.Equal(AccountTag.Activo, accounts[0].AccountTag);
            Assert.Equal(AccountTag.Egreso, accounts[5].AccountTag);

            Assert.Equal("ACTIVO CORRIENTE", accounts[6].Name);
            Assert.Equal(1, accounts[6].FatherAccount);
            Assert.Equal(AccountType.Cuenta_De_Mayor, accounts[6].AccountType);

            Assert.Equal("CAJA", accounts[18].Name);
            Assert.Equal(7, accounts[18].FatherAccount);
            Assert.Equal(AccountType.Cuenta_Auxiliar, accounts[18].AccountType);

            Assert.Equal("SUELDOS Y SALARIOS", accounts[29].Name);
            Assert.Equal(16, accounts[29].FatherAccount);

            Assert.Equal("INGRESO", accounts[54].Name);
            Assert.Equal(4, accounts[54].FatherAccount);
            Assert.Equal(AccountTag.Ingreso, accounts[54].AccountTag);
            Assert.Equal(AccountType.Cuenta_De_Mayor, accounts[54].AccountType);

            Assert.Equal("COSTO VENTA", accounts[55].Name);
            Assert.Equal(5, accounts[55].FatherAccount);
            Assert.Equal("EGRESO", accounts[56].Name);
            Assert.Equal(6, accounts[56].FatherAccount);
        }
    }
}
