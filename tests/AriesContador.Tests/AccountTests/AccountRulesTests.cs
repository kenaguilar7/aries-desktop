using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using Xunit;

namespace AriesContador.Tests.AccountTests
{
    public class AccountRulesTests
    {
        [Fact]
        public void OrderByTree_is_preorder_title_then_children()
        {
            var titulo = new Account { Name = "Activo", Id = 1, FatherAccount = 0, AccountType = AccountType.Cuenta_Titulo };
            var mayor = new Account { Name = "Circulante", Id = 2, FatherAccount = 1, AccountType = AccountType.Cuenta_De_Mayor };
            var aux = new Account { Name = "Caja", Id = 3, FatherAccount = 2, AccountType = AccountType.Cuenta_Auxiliar };
            var pasivo = new Account { Name = "Pasivo", Id = 10, FatherAccount = 0, AccountType = AccountType.Cuenta_Titulo };

            var ordered = AccountRules.OrderByTree(new[] { aux, pasivo, mayor, titulo })
                .Select(c => c.Id)
                .ToArray();

            Assert.Equal(new[] { 10, 1, 2, 3 }, ordered);
        }

        [Fact]
        public void ApplyRollUp_adds_auxiliar_debits_credits_to_parents_not_prior()
        {
            var titulo = new Account { Id = 1, AccountType = AccountType.Cuenta_Titulo, PriorBalance = 9 };
            var mayor = new Account { Id = 2, FatherAccount = 1, AccountType = AccountType.Cuenta_De_Mayor };
            var aux = new Account
            {
                Id = 3,
                FatherAccount = 2,
                AccountType = AccountType.Cuenta_Auxiliar,
                DebitBalance = 80,
                CreditBalance = 5,
                DebitBalanceForeign = 2,
                CreditBalanceForeign = 1
            };

            AccountRules.ApplyRollUp(new List<Account> { titulo, mayor, aux });

            Assert.Equal(80, mayor.DebitBalance);
            Assert.Equal(5, mayor.CreditBalance);
            Assert.Equal(80, titulo.DebitBalance);
            Assert.Equal(2, titulo.DebitBalanceForeign);
            Assert.Equal(80, aux.DebitBalance);
            Assert.Equal(9, titulo.PriorBalance);
        }

        [Fact]
        public void RemoveAccountsWithoutBalances_keeps_titles_without_movement()
        {
            var titulo = new Account { Id = 1, AccountType = AccountType.Cuenta_Titulo };
            var mayorVacio = new Account { Id = 2, FatherAccount = 1, AccountType = AccountType.Cuenta_De_Mayor };
            var auxConSaldo = new Account
            {
                Id = 3,
                FatherAccount = 2,
                AccountType = AccountType.Cuenta_Auxiliar,
                DebitBalance = 10
            };

            var filtered = AccountRules.RemoveAccountsWithoutBalances(new[] { titulo, mayorVacio, auxConSaldo });

            Assert.Contains(filtered, c => c.Id == 1);
            Assert.Contains(filtered, c => c.Id == 3);
            Assert.DoesNotContain(filtered, c => c.Id == 2);
        }

        [Fact]
        public void CanDelete_rejects_system_accounts()
        {
            var account = new Account
            {
                Name = "Sistema",
                AccountType = AccountType.Cuenta_Auxiliar,
                Editable = false
            };

            var ok = AccountRules.CanDelete(account, out var mensaje);

            Assert.False(ok);
            Assert.Contains("sistema", mensaje.ToLowerInvariant());
        }

        [Fact]
        public void CanDelete_rejects_non_auxiliar()
        {
            var account = new Account
            {
                Name = "Titulo",
                AccountType = AccountType.Cuenta_Titulo,
                Editable = true
            };

            var ok = AccountRules.CanDelete(account, out var mensaje);

            Assert.False(ok);
            Assert.Contains("No pueden ser eliminadas", mensaje);
        }

        [Fact]
        public void InheritBalancesIfParentIsAuxiliar_copies_balances()
        {
            var padre = new Account
            {
                Id = 2,
                AccountType = AccountType.Cuenta_Auxiliar,
                PriorBalance = 40,
                DebitBalance = 10,
                CreditBalance = 3,
                DebitBalanceForeign = 1
            };
            var nueva = new Account { Id = 3, AccountType = AccountType.Cuenta_Auxiliar };

            AccountRules.InheritBalancesIfParentIsAuxiliar(nueva, padre);

            Assert.Equal(40, nueva.PriorBalance);
            Assert.Equal(10, nueva.DebitBalance);
            Assert.Equal(3, nueva.CreditBalance);
            Assert.Equal(1, nueva.DebitBalanceForeign);
        }

        [Fact]
        public void InheritBalancesIfParentIsAuxiliar_does_nothing_when_parent_is_mayor()
        {
            var padre = new Account { AccountType = AccountType.Cuenta_De_Mayor, DebitBalance = 99 };
            var nueva = new Account { AccountType = AccountType.Cuenta_Auxiliar };

            AccountRules.InheritBalancesIfParentIsAuxiliar(nueva, padre);

            Assert.Equal(0, nueva.DebitBalance);
        }

        [Fact]
        public void ValidateName_rejects_blank()
        {
            var ok = AccountRules.ValidateName("   ", out var mensaje);

            Assert.False(ok);
            Assert.Equal(AccountRules.BlankNameMessage, mensaje);
        }

        [Fact]
        public void HasMovement_ignores_foreign_balances()
        {
            var onlyDollars = new Account { DebitBalanceForeign = 50 };
            Assert.False(AccountRules.HasMovement(onlyDollars));

            var colones = new Account { PriorBalance = 1 };
            Assert.True(AccountRules.HasMovement(colones));
        }
    }
}
