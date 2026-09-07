using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace AriesContador.Tests.AccountTests
{
    public class AccountTreeAndBalanceTests
    {
        [Fact]
        public void OrderByTree_is_preorder_title_then_children()
        {
            var titulo = Acc(1, "Activo", AccountType.Cuenta_Titulo, null);
            var mayor = Acc(2, "Circulante", AccountType.Cuenta_De_Mayor, 1);
            var aux = Acc(3, "Caja", AccountType.Cuenta_Auxiliar, 2);
            var otroTitulo = Acc(10, "Pasivo", AccountType.Cuenta_Titulo, null);

            var ordered = new[] { aux, otroTitulo, mayor, titulo }.OrderByTree().Select(a => a.Id).ToArray();

            Assert.Equal(new[] { 10, 1, 2, 3 }, ordered);
        }

        [Fact]
        public void BuildAccountsBalance_rolls_auxiliar_debit_up_to_parents()
        {
            var titulo = Acc(1, "Activo", AccountType.Cuenta_Titulo, null);
            var mayor = Acc(2, "Circulante", AccountType.Cuenta_De_Mayor, 1);
            var aux = Acc(3, "Caja", AccountType.Cuenta_Auxiliar, 2);
            aux.DebitBalance = 80;
            aux.CreditBalance = 5;

            new[] { titulo, mayor, aux }.BuildAccountsBalance();

            Assert.Equal(80, mayor.DebitBalance);
            Assert.Equal(5, mayor.CreditBalance);
            Assert.Equal(80, titulo.DebitBalance);
            Assert.Equal(5, titulo.CreditBalance);
            Assert.Equal(80, aux.DebitBalance);
        }

        [Fact]
        public void GetTotalPeridasYGanancias_is_ingreso_minus_costo_minus_egreso()
        {
            var accounts = new[]
            {
                Title(AccountTag.Ingreso, 1000),
                Title(AccountTag.CostoVenta, 300),
                Title(AccountTag.Egreso, 50),
                Title(AccountTag.Activo, 9999)
            };

            Assert.Equal(650m, accounts.GetTotalPeridasYGanancias());
        }

        private static Account Acc(int id, string name, AccountType type, int? father)
        {
            return new Account
            {
                Id = id,
                Name = name,
                AccountType = type,
                FatherAccount = father,
                DebOCred = DebOrCred.Debito
            };
        }

        private static Account Title(AccountTag tag, decimal credit)
        {
            return new Account
            {
                AccountType = AccountType.Cuenta_Titulo,
                AccountTag = tag,
                DebOCred = DebOrCred.Credito,
                CreditBalance = credit
            };
        }
    }
}
