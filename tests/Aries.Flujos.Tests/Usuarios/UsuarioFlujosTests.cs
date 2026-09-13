using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Users;
using AriesContador.Services.Security;
using Xunit;

namespace Aries.Flujos.Tests.Usuarios
{
    [Collection("flujos-db")]
    public class CrearUsuarioTests
    {
        private readonly FlujosDbFixture _db;

        public CrearUsuarioTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Crear_inserta_fila_y_hashea_password()
        {
            var admin = _db.Services.Administration;
            var user = FlujoBuilders.NuevoUsuario(FlujoBuilders.UniqueUserName(), "Clave.123");

            await admin.CreateUserAsync(user);
            Assert.True(user.Id > 0);

            var storedName = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT user_name FROM users WHERE user_id = @id",
                ("@id", user.Id));
            Assert.Equal(user.UserName, storedName);

            var storedPassword = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT password FROM users WHERE user_id = @id",
                ("@id", user.Id));
            Assert.True(PasswordHasher.LooksHashed(storedPassword));
            Assert.True(PasswordHasher.Verify("Clave.123", storedPassword));
            Assert.NotEqual("Clave.123", storedPassword);

            var listed = (await admin.GetAllUsersAsync()).Single(u => u.Id == user.Id);
            Assert.Null(listed.Password);
            Assert.Equal(UserType.Usuario, listed.UserType);
        }

        [DockerFact]
        public async Task Crear_rechaza_username_duplicado_sin_segunda_fila()
        {
            var admin = _db.Services.Administration;
            var name = FlujoBuilders.UniqueUserName("dup");
            await admin.CreateUserAsync(FlujoBuilders.NuevoUsuario(name, "Clave.123"));

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => admin.CreateUserAsync(FlujoBuilders.NuevoUsuario(name, "Otra.123")));
            Assert.Contains("registrado", ex.Message);

            var count = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM users WHERE user_name = @name",
                ("@name", name));
            Assert.Equal(1L, count);
        }

        [DockerFact]
        public async Task Login_acepta_clave_correcta_y_rechaza_la_incorrecta()
        {
            var admin = _db.Services.Administration;
            var name = FlujoBuilders.UniqueUserName("log");
            await admin.CreateUserAsync(FlujoBuilders.NuevoUsuario(name, "Clave.123"));

            var ok = await admin.LoginAsync(new Login { UserId = name, Password = "Clave.123" });
            Assert.Equal("local", ok.Token);
            Assert.Equal(name, ok.User.UserName);
            Assert.Null(ok.User.Password);

            var bad = await admin.LoginAsync(new Login { UserId = name, Password = "nope" });
            Assert.Null(bad.User);
            Assert.True(string.IsNullOrEmpty(bad.Token));
        }

        [DockerFact]
        public async Task UserNameTaken_detecta_el_insert()
        {
            var admin = _db.Services.Administration;
            var name = FlujoBuilders.UniqueUserName("tk");
            Assert.False(await admin.UserNameTakenAsync(name));

            await admin.CreateUserAsync(FlujoBuilders.NuevoUsuario(name, "Clave.123"));
            Assert.True(await admin.UserNameTakenAsync(name));
        }
    }

    [Collection("flujos-db")]
    public class ActualizarUsuarioTests
    {
        private readonly FlujosDbFixture _db;

        public ActualizarUsuarioTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Actualizar_cambia_nombre_y_conserva_password_si_viene_vacio()
        {
            var admin = _db.Services.Administration;
            var created = FlujoBuilders.NuevoUsuario(FlujoBuilders.UniqueUserName("up"), "Clave.123");
            await admin.CreateUserAsync(created);

            var before = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT password FROM users WHERE user_id = @id",
                ("@id", created.Id));

            created.Name = "Nombre Editado";
            created.Password = null;
            created.UpdatedBy = FlujosDbFixture.AdminUserId;
            await admin.UpdateUserAsync(created);

            var name = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT name FROM users WHERE user_id = @id",
                ("@id", created.Id));
            Assert.Equal("Nombre Editado", name);

            var after = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT password FROM users WHERE user_id = @id",
                ("@id", created.Id));
            Assert.Equal(before, after);
        }

        [DockerFact]
        public async Task Inactivar_pone_active_en_cero_y_bloquea_login()
        {
            var admin = _db.Services.Administration;
            var created = FlujoBuilders.NuevoUsuario(FlujoBuilders.UniqueUserName("in"), "Clave.123");
            await admin.CreateUserAsync(created);

            created.UpdatedBy = FlujosDbFixture.AdminUserId;
            await admin.InactivateUserAsync(created);

            var active = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM users WHERE user_id = @id",
                ("@id", created.Id));
            Assert.Equal(0, active);

            var token = await admin.LoginAsync(new Login { UserId = created.UserName, Password = "Clave.123" });
            Assert.Null(token.User);
        }
    }
}
