using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.Usuarios;
using Aries.Reporting.Entidades.Ventanas;
using Aries.Reporting.Mappers;
using Aries.Desktop.Conf;
using Microsoft.Extensions.DependencyInjection;
using Squirrel;

namespace Aries.Desktop
{
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            LoadHttpBaseUrl();
            LoadDatabaseConnectionString();
            CheckForUpdates();
        }

        private static void LoadHttpBaseUrl()
        {
            var fromEnv = Environment.GetEnvironmentVariable("ARIES_HTTP_BASE_URL");
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                EnvironmentVariable.ApiUrl = fromEnv;
                return;
            }

            var httpBase = ConfigurationManager.ConnectionStrings["HttpBaseUrl"];
            if (httpBase != null && !string.IsNullOrWhiteSpace(httpBase.ConnectionString))
                EnvironmentVariable.ApiUrl = httpBase.ConnectionString;
        }

        private static void LoadDatabaseConnectionString()
        {
            var db = ConfigurationManager.ConnectionStrings["DBconnectionString"]
                ?? ConfigurationManager.ConnectionStrings["DBconnectionstring"];
            var fromEnv = Environment.GetEnvironmentVariable("ARIES_MYSQL_CONNECTION")
                          ?? Environment.GetEnvironmentVariable("ConnectionStrings__MySQLDefault");
            var cs = !string.IsNullOrWhiteSpace(fromEnv) ? fromEnv : db?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new ConfigurationErrorsException(
                    "Falta connectionString 'DBconnectionString' o ARIES_MYSQL_CONNECTION.");
            }

            var server = ReadConnectionPart(cs, "Server")
                ?? ReadConnectionPart(cs, "Data Source")
                ?? ReadConnectionPart(cs, "Host");
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ConfigurationErrorsException(
                    "DBconnectionString no tiene Server=. Maestro de Cuentas y Asientos fallarán.");
            }
        }

        private static string ReadConnectionPart(string connectionString, string key)
        {
            foreach (var part in connectionString.Split(';'))
            {
                var trimmed = part.Trim();
                var eq = trimmed.IndexOf('=');
                if (eq <= 0)
                    continue;
                var name = trimmed.Substring(0, eq).Trim();
                if (name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring(eq + 1).Trim();
            }
            return null;
        }

        private async Task CheckForUpdates()
        {
            try
            {
                var updateUrl = Environment.GetEnvironmentVariable("ARIES_UPDATE_URL")
                    ?? ConfigurationManager.ConnectionStrings["UpdateServerString"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(updateUrl))
                    return;

                using (var manager = new UpdateManager(updateUrl))
                {
                    await manager.UpdateApp();
                }
            }
            catch
            {
                // Squirrel no debe impedir el login.
            }
        }

        public static List<Modulo> Permisos = new List<Modulo>();

        public static List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public static List<Company> Compañias { get; set; } = new List<Company>();

        public static ConnectionString ConnectionString = new ConnectionString();

        public static string EnvironmentName =>
            ConfigurationManager.AppSettings["EnvironmentName"] ?? "Local";

        public static bool IsLocalEnvironment =>
            string.Equals(EnvironmentName, "Local", StringComparison.OrdinalIgnoreCase);

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
                try
                {
                    if (value != null && Services != null)
                    {
                        var permissions = Services.GetRequiredService<IPermissionService>();
                        Usuario.Modulos = PermissionMapper.ToModulos(permissions.GetModules(value.Id));
                    }
                    else if (Usuario != null)
                    {
                        Usuario.Modulos = new List<Modulo>();
                    }
                }
                catch
                {
                    if (Usuario != null)
                        Usuario.Modulos = new List<Modulo>();
                }

                user = value;
            }
        }
    }
}
