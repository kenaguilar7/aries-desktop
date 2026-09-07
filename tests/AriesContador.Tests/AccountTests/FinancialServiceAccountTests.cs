using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.AccountTests
{
    public class FinancialServiceAccountTests
    {
        [Fact]
        public async Task GetAccounts_returns_preorder_tree()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.Items.AddRange(new[]
            {
                Aux("Caja", 3, 2),
                Title("Pasivo", 10),
                Mayor("Circulante", 2, 1),
                Title("Activo", 1)
            });
            var svc = new FinancialService(uow);

            var ordered = (await svc.GetAccountsAsync("C001")).Select(a => a.Id).ToArray();

            Assert.Equal(new[] { 10, 1, 2, 3 }, ordered);
        }

        [Fact]
        public async Task CreateAccount_rejects_blank_name()
        {
            var svc = new FinancialService(new FakeUnitOfWork());

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CreateAccountAsync(new Account { Name = "  ", CompanyId = "C001" }, parent: null));

            Assert.Equal(AccountRules.BlankNameMessage, ex.Message);
        }

        [Fact]
        public async Task CreateAccount_rejects_name_taken_on_activo()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.Items.Add(new Account
            {
                Id = 1,
                Name = "Caja",
                CompanyId = "C001",
                AccountTag = AccountTag.Activo
            });
            var svc = new FinancialService(uow);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CreateAccountAsync(new Account { Name = "Caja", CompanyId = "C001" }, parent: null));

            Assert.Equal(AccountRules.NameCannotBeUsedMessage, ex.Message);
        }

        [Fact]
        public async Task CreateAccount_under_auxiliar_inherits_and_promotes_father()
        {
            var uow = new FakeUnitOfWork();
            var padre = new Account
            {
                Id = 2,
                Name = "Padre aux",
                CompanyId = "C001",
                AccountType = AccountType.Cuenta_Auxiliar,
                AccountTag = AccountTag.Activo,
                PriorBalance = 40,
                DebitBalance = 10
            };
            uow.Accounts.Items.Add(padre);
            var svc = new FinancialService(uow);
            var nueva = new Account { Name = "Hija", CompanyId = "C001", UpdatedBy = 7 };

            await svc.CreateAccountAsync(nueva, padre);

            Assert.Equal(3, nueva.Id);
            Assert.Equal(40, nueva.PriorBalance);
            Assert.Equal(10, nueva.DebitBalance);
            Assert.Equal(AccountType.Cuenta_Auxiliar, nueva.AccountType);
            Assert.Equal(AccountTag.Activo, nueva.AccountTag);
            Assert.Equal(AccountType.Cuenta_De_Mayor, padre.AccountType);
            Assert.Contains(2, uow.Accounts.PromotedFatherIds);
        }

        [Fact]
        public async Task CreateAccount_under_mayor_does_not_copy_balances()
        {
            var uow = new FakeUnitOfWork();
            var padre = new Account
            {
                Id = 2,
                CompanyId = "C001",
                AccountType = AccountType.Cuenta_De_Mayor,
                AccountTag = AccountTag.Pasivo,
                DebitBalance = 99
            };
            uow.Accounts.Items.Add(padre);
            var svc = new FinancialService(uow);
            var nueva = new Account { Name = "Hija", CompanyId = "C001" };

            await svc.CreateAccountAsync(nueva, padre);

            Assert.Equal(0, nueva.DebitBalance);
            Assert.Empty(uow.Accounts.PromotedFatherIds);
            Assert.Equal(AccountType.Cuenta_De_Mayor, padre.AccountType);
        }

        [Fact]
        public async Task DeleteAccount_rejects_system_and_non_auxiliar()
        {
            var svc = new FinancialService(new FakeUnitOfWork());

            var systemEx = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.DeleteAccountAsync(new Account { AccountType = AccountType.Cuenta_Auxiliar, Editable = false }));
            Assert.Contains("sistema", systemEx.Message.ToLowerInvariant());

            var titleEx = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.DeleteAccountAsync(new Account { AccountType = AccountType.Cuenta_Titulo, Editable = true }));
            Assert.Contains("No pueden ser eliminadas", titleEx.Message);
        }

        [Fact]
        public async Task DeleteAccount_rejects_open_period_movements()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.HasOpenPeriodMovementsResult = true;
            var svc = new FinancialService(uow);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.DeleteAccountAsync(new Account { Id = 3, AccountType = AccountType.Cuenta_Auxiliar, Editable = true }));

            Assert.Equal(AccountRules.DeleteWithMovementsMessage, ex.Message);
            Assert.Empty(uow.Accounts.Removed);
        }

        [Fact]
        public async Task DeleteAccount_soft_deletes_auxiliar()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var account = new Account { Id = 3, AccountType = AccountType.Cuenta_Auxiliar, Editable = true };

            await svc.DeleteAccountAsync(account);

            Assert.Same(account, uow.Accounts.Removed.Single());
        }

        [Fact]
        public async Task UpdateAccount_rejects_duplicate_name()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.Items.Add(new Account
            {
                Id = 1,
                Name = "Caja",
                CompanyId = "C001",
                AccountTag = AccountTag.Activo
            });
            var svc = new FinancialService(uow);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.UpdateAccountAsync(new Account { Id = 2, Name = "Caja", CompanyId = "C001" }));

            Assert.Equal(AccountRules.NameTakenMessage, ex.Message);
        }

        [Fact]
        public async Task EvaluateParentForNewChild_warns_when_auxiliar_has_movement()
        {
            var uow = new FakeUnitOfWork();
            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                CompanyId = "C001",
                Date = new DateTime(2024, 1, 1)
            });
            var padre = new Account
            {
                Id = 3,
                CompanyId = "C001",
                AccountType = AccountType.Cuenta_Auxiliar,
                PriorBalance = 25
            };
            var svc = new FinancialService(uow);

            var result = await svc.EvaluateParentForNewChildAsync(padre);

            Assert.False(result.CanProceed);
            Assert.Contains("posee movimientos", result.Message);
        }

        [Fact]
        public async Task FillAccountsWithBalances_rolls_up_from_account_info_rows()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.BalanceRows.Add(new Account
            {
                Id = 3,
                DebitBalance = 80,
                CreditBalance = 5
            });
            var titulo = new Account { Id = 1, CompanyId = "C001", AccountType = AccountType.Cuenta_Titulo };
            var mayor = new Account { Id = 2, FatherAccount = 1, CompanyId = "C001", AccountType = AccountType.Cuenta_De_Mayor };
            var aux = new Account { Id = 3, FatherAccount = 2, CompanyId = "C001", AccountType = AccountType.Cuenta_Auxiliar };
            var svc = new FinancialService(uow);

            await svc.FillAccountsWithBalancesAsync(new List<Account> { titulo, mayor, aux }, new DateTime(2024, 1, 1), new DateTime(2024, 2, 1));

            Assert.Equal(80, aux.DebitBalance);
            Assert.Equal(80, mayor.DebitBalance);
            Assert.Equal(80, titulo.DebitBalance);
            Assert.Equal(5, titulo.CreditBalance);
        }

        private static Account Title(string name, int id) => new Account
        {
            Name = name,
            Id = id,
            CompanyId = "C001",
            AccountType = AccountType.Cuenta_Titulo
        };

        private static Account Mayor(string name, int id, int father) => new Account
        {
            Name = name,
            Id = id,
            FatherAccount = father,
            CompanyId = "C001",
            AccountType = AccountType.Cuenta_De_Mayor
        };

        private static Account Aux(string name, int id, int father) => new Account
        {
            Name = name,
            Id = id,
            FatherAccount = father,
            CompanyId = "C001",
            AccountType = AccountType.Cuenta_Auxiliar
        };
    }
}
