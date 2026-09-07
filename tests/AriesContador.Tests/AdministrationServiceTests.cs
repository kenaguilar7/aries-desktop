using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Models.Utils;
using AriesContador.Services;
using AriesContador.Services.Security;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests
{
    public class AdministrationServiceTests
    {
        [Fact]
        public async Task Login_rejects_wrong_password()
        {
            var uow = new FakeUnitOfWork();
            uow.Users.Add(new User { UserName = "kenneth", Password = "96321", Active = true, Name = "K" });
            var svc = new AdministrationService(uow);

            var token = await svc.LoginAsync(new Login { UserId = "kenneth", Password = "nope" });

            Assert.Null(token.User);
            Assert.True(string.IsNullOrEmpty(token.Token));
        }

        [Fact]
        public async Task Login_returns_local_token_for_active_user()
        {
            var uow = new FakeUnitOfWork();
            uow.Users.Add(new User { UserName = "kenneth", Password = "96321", Active = true, Name = "K" });
            var svc = new AdministrationService(uow);

            var token = await svc.LoginAsync(new Login { UserId = "kenneth", Password = "96321" });

            Assert.Equal("local", token.Token);
            Assert.Equal("kenneth", token.User.UserName);
            Assert.Null(token.User.Password);
            Assert.Equal(0, uow.Users.GetAllCalls);
            Assert.True(PasswordHasher.LooksHashed(uow.Users.Items[0].Password));
        }

        [Fact]
        public async Task Login_verifies_existing_hash_and_does_not_rehash()
        {
            var hashed = PasswordHasher.Hash("96321");
            var uow = new FakeUnitOfWork();
            uow.Users.Add(new User { UserName = "kenneth", Password = hashed, Active = true, Name = "K" });
            var svc = new AdministrationService(uow);

            var token = await svc.LoginAsync(new Login { UserId = "kenneth", Password = "96321" });

            Assert.Equal("local", token.Token);
            Assert.Equal(hashed, uow.Users.Items[0].Password);
        }

        [Fact]
        public async Task Login_rejects_inactive_user()
        {
            var uow = new FakeUnitOfWork();
            uow.Users.Add(new User { UserName = "kenneth", Password = "96321", Active = false, Name = "K" });
            var svc = new AdministrationService(uow);

            var token = await svc.LoginAsync(new Login { UserId = "kenneth", Password = "96321" });

            Assert.Null(token.User);
        }

        [Fact]
        public async Task CreateUser_hashes_password()
        {
            var uow = new FakeUnitOfWork();
            var svc = new AdministrationService(uow);

            await svc.CreateUserAsync(new User { UserName = "nuevo", Name = "N", Password = "plain" });

            Assert.True(PasswordHasher.LooksHashed(uow.Users.Items[0].Password));
            Assert.True(PasswordHasher.Verify("plain", uow.Users.Items[0].Password));
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateCompany_copies_full_chart_not_id_filter()
        {
            var uow = new FakeUnitOfWork();
            for (var i = 1; i <= 80; i++)
            {
                uow.Accounts.Items.Add(new Account
                {
                    Id = i,
                    Name = $"Cuenta {i}",
                    CompanyId = "C001",
                    FatherAccount = i == 1 ? 0 : 1
                });
            }

            var svc = new AdministrationService(uow);
            var company = new Company
            {
                NumberId = "3-101-123456",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "Copia completa",
                Mail = "a@b.com",
                CopyFrom = "C001"
            };

            await svc.CreateCompanyAsync(company);

            var saved = Assert.Single(uow.Companies.Added);
            Assert.Equal(80, saved.Account.Count());
            Assert.All(saved.Account, a => Assert.Equal(saved.Code, a.CompanyId));
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateCompany_por_defecto_loads_builtin_chart()
        {
            var uow = new FakeUnitOfWork();
            var svc = new AdministrationService(uow);
            var company = new Company
            {
                NumberId = "3-101-123456",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "Nueva",
                Mail = "a@b.com",
                CopyFrom = "POR DEFECTO"
            };

            await svc.CreateCompanyAsync(company);

            var saved = Assert.Single(uow.Companies.Added);
            var accounts = saved.Account.ToList();
            Assert.Equal(DefaultChartOfAccounts.AccountCount, accounts.Count);
            Assert.All(accounts, a => Assert.Equal(saved.Code, a.CompanyId));
            Assert.Equal("ACTIVO CORRIENTE", accounts[6].Name);
            Assert.Equal(1, accounts[6].FatherAccount);
            Assert.Equal("INGRESO", accounts[54].Name);
            Assert.Equal(4, accounts[54].FatherAccount);
        }

        [Fact]
        public async Task CreateUser_rejects_blank_name_or_username()
        {
            var svc = new AdministrationService(new FakeUnitOfWork());

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(() =>
                svc.CreateUserAsync(new User { Name = "  ", UserName = "admin" }));

            Assert.Contains("blanco", ex.Message);
        }

        [Fact]
        public async Task CreateUser_rejects_duplicate_username()
        {
            var uow = new FakeUnitOfWork();
            uow.Users.Add(new User { UserName = "admin", Name = "A", Password = "x", Active = true });
            var svc = new AdministrationService(uow);

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(() =>
                svc.CreateUserAsync(new User { UserName = "admin", Name = "Otro", Password = "y" }));

            Assert.Contains("registrado", ex.Message);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateCompany_rejects_invalid_cedula()
        {
            var svc = new AdministrationService(new FakeUnitOfWork());
            var company = new Company
            {
                NumberId = "1",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "Demo",
                Mail = "a@b.com"
            };

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(() => svc.CreateCompanyAsync(company));
            Assert.Contains("cédula", ex.Message.ToLowerInvariant());
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateCompany_rejects_blank_name()
        {
            var svc = new AdministrationService(new FakeUnitOfWork());
            var company = new Company
            {
                NumberId = "3-101-123456",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "  ",
                Mail = "a@b.com"
            };

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(() => svc.CreateCompanyAsync(company));
            Assert.Contains("Nombre", ex.Message);
        }
    }
}
