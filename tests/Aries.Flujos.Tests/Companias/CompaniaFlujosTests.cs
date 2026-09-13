using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace Aries.Flujos.Tests.Companias
{
    [Collection("flujos-db")]
    public class CrearCompaniaTests
    {
        private readonly FlujosDbFixture _db;

        public CrearCompaniaTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Crear_juridica_inserta_fila_y_plan_por_defecto()
        {
            var admin = _db.Services.Administration;
            var persona = FlujoBuilders.NuevaJuridica(
                "Cia Flujos Juridica",
                "juridica@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);

            await admin.CreateCompanyAsync(persona);

            Assert.Matches(new Regex("^C\\d{3}$"), persona.Code);

            var name = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT name FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal(persona.CompanyName, name);

            var numberId = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT number_id FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal(persona.NumberId, numberId);

            var typeId = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT type_id FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal((int)IdType.CEDULA_JURIDICA, typeId);

            var userId = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT user_id FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal(FlujosDbFixture.AdminUserId, userId);

            var accounts = (await _db.Services.UnitOfWork.AccountRepository.FindByCompanyIdAsync(persona.Code)).ToList();
            Assert.Equal(DefaultChartOfAccounts.AccountCount, accounts.Count);

            var loaded = await admin.FindByCodeAsync(persona.Code);
            Assert.Equal(persona.CompanyName, loaded.CompanyName);
            Assert.True(loaded.Active);
        }

        [DockerFact]
        public async Task Crear_fisica_inserta_fila()
        {
            var admin = _db.Services.Administration;
            var persona = FlujoBuilders.NuevaFisica(
                "Cia Flujos Fisica",
                "fisica@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);

            await admin.CreateCompanyAsync(persona);

            var typeId = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT type_id FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal((int)IdType.CEDULA_NACIONAL, typeId);

            var op1 = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT op1 FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal("Perez", op1);

            var loaded = await admin.FindByCodeAsync(persona.Code);
            var fisica = Assert.IsType<PersonaFisica>(loaded);
            Assert.Equal("Perez", fisica.MyApellidoPaterno);
            Assert.Equal("Mora", fisica.MyApellidoMaterno);
        }

        [DockerFact]
        public async Task Crear_asigna_consecutivo_y_queda_en_el_listado()
        {
            var admin = _db.Services.Administration;
            var before = await admin.GetCompanyConsecutiveAsync();
            var persona = FlujoBuilders.NuevaJuridica(
                "Cia Consecutivo",
                "consecutivo@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);

            await admin.CreateCompanyAsync(persona);

            Assert.Equal(before, persona.Code);
            var listed = await admin.GetAllCompaniesAsync();
            Assert.Contains(listed, c => c.Code == persona.Code && c.NumberId == persona.NumberId);
        }

        [DockerFact]
        public async Task Crear_rechaza_cedula_invalida_sin_insertar()
        {
            var admin = _db.Services.Administration;
            var persona = FlujoBuilders.NuevaJuridica(
                "Cia Invalida",
                "invalida@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);
            persona.NumberId = "1";

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => admin.CreateCompanyAsync(persona));
            Assert.Contains("cédula", ex.Message.ToLowerInvariant());

            var count = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM companies WHERE name = @name",
                ("@name", "Cia Invalida"));
            Assert.Equal(0L, count);
        }
    }

    [Collection("flujos-db")]
    public class ActualizarCompaniaTests
    {
        private readonly FlujosDbFixture _db;

        public ActualizarCompaniaTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Actualizar_persiste_nombre_y_correo()
        {
            var admin = _db.Services.Administration;
            var persona = FlujoBuilders.NuevaJuridica(
                "Cia Para Editar",
                "editar@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);
            await admin.CreateCompanyAsync(persona);

            var loaded = await admin.FindByCodeAsync(persona.Code);
            loaded.CompanyName = "Cia Editada";
            loaded.Mail = "editada@aries.test";
            loaded.CreatedBy = FlujosDbFixture.AdminUserId;
            await admin.UpdateCompanyAsync(loaded);

            var name = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT name FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal("Cia Editada", name);

            var mail = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT mail FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal("editada@aries.test", mail);
        }

        [DockerFact]
        public async Task Eliminar_borra_fila_y_cuentas()
        {
            var admin = _db.Services.Administration;
            var persona = FlujoBuilders.NuevaJuridica(
                "Cia Para Borrar",
                "borrar@aries.test",
                createdBy: FlujosDbFixture.AdminUserId);
            await admin.CreateCompanyAsync(persona);

            await admin.DeleteCompanyAsync(persona);

            var companies = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM companies WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal(0L, companies);

            var accounts = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM accounts WHERE company_id = @code",
                ("@code", persona.Code));
            Assert.Equal(0L, accounts);
        }
    }
}
