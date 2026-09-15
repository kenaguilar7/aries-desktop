using System.Collections.Generic;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Enumeradores;
using Aries.Reporting.Mappers;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class AccountDeleteWorkflowTests
    {
        [Fact]
        public void TryPrepare_maps_editable_auxiliar_for_delete()
        {
            var cuenta = Auxiliar("Caja temporal", editable: true);

            var ok = AccountDeleteWorkflow.TryPrepare(cuenta, updatedBy: 7, out var account, out var error);

            Assert.True(ok, error);
            Assert.Equal(9, account.Id);
            Assert.Equal("C001", account.CompanyId);
            Assert.True(account.Editable);
            Assert.Equal(AccountType.Cuenta_Auxiliar, account.AccountType);
            Assert.Equal(7, account.UpdatedBy);
        }

        [Fact]
        public void TryPrepare_rejects_system_and_title_accounts()
        {
            Assert.False(AccountDeleteWorkflow.TryPrepare(
                Auxiliar("Sistema", editable: false), 1, out _, out var systemError));
            Assert.Contains("sistema", systemError.ToLowerInvariant());

            var titulo = Auxiliar("Activo", editable: true);
            titulo.Indicador = IndicadorCuenta.Cuenta_Titulo;
            Assert.False(AccountDeleteWorkflow.CanDelete(titulo, out var titleError));
            Assert.Contains("No pueden ser eliminadas", titleError);
        }

        [Fact]
        public void TryPrepare_requires_selected_account_with_id()
        {
            Assert.False(AccountDeleteWorkflow.TryPrepare(null, 1, out _, out var missing));
            Assert.Equal(AccountDeleteWorkflow.SelectAccountMessage, missing);

            var sinId = Auxiliar("Sin id", editable: true);
            sinId.Id = 0;
            Assert.False(AccountDeleteWorkflow.TryPrepare(sinId, 1, out _, out var noId));
            Assert.Equal(AccountDeleteWorkflow.SelectAccountMessage, noId);
        }

        [Fact]
        public void RemoveFromList_drops_by_id_not_reference()
        {
            var original = Auxiliar("Temporal", editable: true);
            var sameId = Auxiliar("Temporal copia", editable: true);
            var sibling = Auxiliar("Otra", editable: true);
            sibling.Id = 10;
            var list = new List<Cuenta> { original, sibling };

            AccountDeleteWorkflow.RemoveFromList(list, sameId.Id);

            Assert.DoesNotContain(list, c => c.Id == 9);
            Assert.Contains(list, c => c.Id == 10);
        }

        [Fact]
        public void ToAccount_round_trips_editable_auxiliar()
        {
            var company = new Company { Code = "C001" };
            var account = new Account
            {
                Id = 9,
                Name = "Temporal",
                Editable = true,
                AccountType = AccountType.Cuenta_Auxiliar,
                AccountTag = AccountTag.Activo,
                CompanyId = company.Code,
                FatherAccount = 2
            };

            var mapped = CuentaMapper.ToAccount(CuentaMapper.ToCuenta(account, company));

            Assert.Equal(9, mapped.Id);
            Assert.True(mapped.Editable);
            Assert.Equal(AccountType.Cuenta_Auxiliar, mapped.AccountType);
            Assert.Equal("C001", mapped.CompanyId);
            Assert.True(AccountRules.CanDelete(mapped, out var error), error);
        }

        private static Cuenta Auxiliar(string nombre, bool editable)
        {
            return new Cuenta
            {
                Id = 9,
                Nombre = nombre,
                Padre = 2,
                Editable = editable,
                Indicador = IndicadorCuenta.Cuenta_Auxiliar,
                MyCompania = new Company { Code = "C001" },
                TipoCuenta = Cuenta.GenerarTipoCuenta((int)AccountTag.Activo)
            };
        }
    }
}
