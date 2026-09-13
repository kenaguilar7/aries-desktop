using System;
using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Accounts;
using Xunit;

namespace Aries.Flujos.Tests.Cuentas
{
    [Collection("flujos-db")]
    public class CuentaFlujosTests
    {
        private readonly FlujosDbFixture _db;

        public CuentaFlujosTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Crear_cuenta_hija_inserta_auxiliar_bajo_bancos()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;

            var loaded = await financial.FindAccountAsync(ctx.BacColones.Id);
            Assert.Equal(HopeMuestra.CuentaBacColones, loaded.Name);
            Assert.Equal(AccountType.Cuenta_Auxiliar, loaded.AccountType);
            Assert.Equal(ctx.Company.Code, loaded.CompanyId);

            var nameId = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT IFNULL((SELECT account_name_id FROM accounts_names WHERE name = @name LIMIT 1), 0)",
                ("@name", HopeMuestra.CuentaBacColones));
            Assert.True(nameId > 0);

            var fatherId = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT IFNULL(father_account, 0) FROM accounts WHERE account_id = @id",
                ("@id", ctx.BacColones.Id));
            Assert.Equal(ctx.Bancos.Id, fatherId);

            var active = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM accounts WHERE account_id = @id",
                ("@id", ctx.BacColones.Id));
            Assert.Equal(1, active);

            var bancos = await financial.FindAccountAsync(ctx.Bancos.Id);
            Assert.Equal(AccountType.Cuenta_De_Mayor, bancos.AccountType);
        }

        [DockerFact]
        public async Task Editar_cuenta_persiste_nombre_y_detalle()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;

            ctx.BacColones.Name = "BAC 912612520 COLONES HOPE";
            ctx.BacColones.Memo = "banco colones HOPE";
            ctx.BacColones.UpdatedBy = FlujosDbFixture.AdminUserId;
            await financial.UpdateAccountAsync(ctx.BacColones);

            var name = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                @"SELECT n.name FROM accounts a
                  JOIN accounts_names n ON n.account_name_id = a.account_name_id
                  WHERE a.account_id = @id",
                ("@id", ctx.BacColones.Id));
            Assert.Equal("BAC 912612520 COLONES HOPE", name);

            var memo = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT detail FROM accounts WHERE account_id = @id",
                ("@id", ctx.BacColones.Id));
            Assert.Equal("banco colones HOPE", memo);
        }

        [DockerFact]
        public async Task Eliminar_cuenta_sin_movimientos_la_desactiva()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;

            var temporal = new Account
            {
                Name = "CUENTA TEMP HOPE " + FlujoBuilders.UniqueToken(),
                CompanyId = ctx.Company.Code,
                FatherAccount = ctx.Bancos.Id,
                Memo = "borrar",
                Editable = true,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.EnsureAccountNameAsync(temporal.Name);
            await financial.CreateAccountAsync(temporal, ctx.Bancos);
            temporal = (await financial.GetAccountsAsync(ctx.Company.Code))
                .First(a => string.Equals((a.Name ?? string.Empty).Trim(), temporal.Name, StringComparison.Ordinal));

            await financial.DeleteAccountAsync(temporal);

            var active = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM accounts WHERE account_id = @id",
                ("@id", temporal.Id));
            Assert.Equal(0, active);

            var listed = await financial.GetAccountsAsync(ctx.Company.Code);
            Assert.DoesNotContain(listed, a => a.Id == temporal.Id);
        }

        [DockerFact]
        public async Task Eliminar_cuenta_de_sistema_es_rechazado()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => _db.Services.Financial.DeleteAccountAsync(ctx.Bancos));
            Assert.Contains("sistema", ex.Message.ToLowerInvariant());
        }
    }
}
