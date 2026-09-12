using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System;
using System.Windows.Forms;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Data.Migrations;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.Usuarios;
using Aries.Reporting.Entidades.Ventanas;
using Aries.Reporting.Mappers;
using Aries.Desktop.Conf;
using Microsoft.Extensions.DependencyInjection;

namespace Aries.Desktop
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            LoadHttpBaseUrl();
            LoadDatabaseConnectionString();
        }

        private static void LoadHttpBaseUrl()
        {
            var fromEnv = Environment.GetEnvironmentVariable("ARIES_HTTP_BASE_URL");
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                EnvironmentVariable.ApiUrl = fromEnv;
                return;
            }

            var httpBase = AppSettingReader.Read("HttpBaseUrl", "HttpBaseUrl");
            if (!string.IsNullOrWhiteSpace(httpBase))
                EnvironmentVariable.ApiUrl = httpBase;
        }

        private static void LoadDatabaseConnectionString()
        {
            var cs = ConnectionString.MySQLDefault;
            var server = MySqlConnectionInfo.Server(cs);
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ConfigurationErrorsException(
                    "DBconnectionString no tiene Server=. Maestro de Cuentas y Asientos fallarán.");
            }
        }

        public static List<Modulo> Permisos = new List<Modulo>();

        public static List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public static List<Company> Compañias { get; set; } = new List<Company>();

        public static ConnectionString ConnectionString = new ConnectionString();

        public static string EnvironmentName =>
            AppSettingReader.Read("EnvironmentName") ?? "Local";

        public static bool IsLocalEnvironment =>
            string.Equals(EnvironmentName, "Local", StringComparison.OrdinalIgnoreCase);

        public static bool IsBeta =>
            AppSettingReader.ReadFlag("IsBeta", "IsBeta", defaultValue: false);

        public static string UpdateUrl =>
            Environment.GetEnvironmentVariable("ARIES_UPDATE_URL")
            ?? AppSettingReader.Read("UpdateUrl", "UpdateServerString");

        public static string MySqlDatabase =>
            MySqlConnectionInfo.ReadPart(ConnectionString.MySQLDefault, "Database") ?? "(sin Database)";

        public static string MySqlServer =>
            MySqlConnectionInfo.Server(ConnectionString.MySQLDefault) ?? "(sin Server)";

        public static bool CanApplySchemaMigrations =>
            SchemaMigrationGate.CanApply(User?.UserType);

        public static IServiceProvider Services { get; set; }

        public static Company Company { get; set; }

        public static Usuario Usuario { get; set; }

        private static User user;
        public static User User
        {
            get { return user; }
            set
            {
                Usuario = UserMapper.ToUsuario(value);
                if (Usuario != null)
                    Usuario.Modulos = new List<Modulo>();
                user = value;
            }
        }

        public static async Task ClearSessionAsync()
        {
            Company = null;
            Cuentas = new List<Cuenta>();
            Compañias = new List<Company>();
            Permisos = new List<Modulo>();
            EnvironmentVariable.ApiToken = new WebToken();
            await SetUserAsync(null).ConfigureAwait(true);
        }

        public static async Task SetUserAsync(User value)
        {
            Usuario = UserMapper.ToUsuario(value);
            try
            {
                if (value != null && Services != null)
                {
                    var permissions = Services.GetRequiredService<IPermissionService>();
                    Usuario.Modulos = PermissionMapper.ToModulos(await permissions.GetModulesAsync(value.Id));
                }
                else if (Usuario != null)
                {
                    Usuario.Modulos = new List<Modulo>();
                }
            }
            catch (Exception ex)
            {
                if (Usuario != null)
                    Usuario.Modulos = new List<Modulo>();
                StartupLog.Write("Permisos: " + ex);
                MessageBox.Show(
                    "No se pudieron cargar los permisos del usuario.\n\n" + ex.GetBaseException().Message,
                    "Aries Contador",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            user = value;
        }
    }
}
