using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using CapaLogica;
using CapaPresentacion.Conf;
using Microsoft.Extensions.DependencyInjection;
using Squirrel;

namespace CapaPresentacion
{
    /// <summary>
    /// usar como clase estatica para cargar datos generales 
    /// a futuro para 
    /// </summary>
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
            var httpBase = ConfigurationManager.ConnectionStrings["HttpBaseUrl"];
            if (httpBase == null || string.IsNullOrWhiteSpace(httpBase.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "Falta connectionString 'HttpBaseUrl' en CapaPresentacion.exe.config (copia de app.config).");
            }

            EnvironmentVariable.ApiUrl = httpBase.ConnectionString;
        }

        private static void LoadDatabaseConnectionString()
        {
            var db = ConfigurationManager.ConnectionStrings["DBconnectionString"]
                ?? ConfigurationManager.ConnectionStrings["DBconnectionstring"];
            if (db == null || string.IsNullOrWhiteSpace(db.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "Falta connectionString 'DBconnectionString' en CapaPresentacion.exe.config (copia de app.config).");
            }

            var server = ReadConnectionPart(db.ConnectionString, "Server")
                ?? ReadConnectionPart(db.ConnectionString, "Data Source")
                ?? ReadConnectionPart(db.ConnectionString, "Host");
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ConfigurationErrorsException(
                    "DBconnectionString no tiene Server=. El login HTTP no prueba RDS; Maestro de Cuentas y Asientos fallarán.");
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
                if (name.Equals(key, System.StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring(eq + 1).Trim();
            }
            return null;
        }

        private async Task CheckForUpdates()
        {
            try
            {
                var updateUrl = ConfigurationManager.ConnectionStrings["UpdateServerString"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(updateUrl))
                    return;

                using (var manager = new UpdateManager(updateUrl))
                {
                    await manager.UpdateApp();
                }
            }
            catch
            {
                // Squirrel/S3 no debe impedir el login.
            }
        }

        public static List<Modulo> Permisos = new List<Modulo>();



        public static bool GetPermiso(Ventana ventana, CRUDItem cRUDItem)
        {


            return false;
        }


        public static void SetModule(Usuario usuario, Modulo modulo)
        {
            var permisos = usuario.Modulos;

        }

        public static List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public static List<Company> Compañias { get; set; } = new List<Company>();
        //public static Usuario Usuario { get; set; }
        //public static User User { get; set;  }
        //public static Company Company { get; set; }
        //public static Company NewCompany { get; set; }

        public static ConnectionString ConnectionString = new ConnectionString();

        public static IServiceProvider Services { get; set; }

        public static string BaseUrl = ConfigurationManager.ConnectionStrings["HttpBaseUrl"]?.ConnectionString;



        public static Company Company { get; set; }
        //private static Company _newCompany; 
        //public static Company NewCompany 
        //{
        //    get { return _newCompany;  }
        //    set 
        //    {
        //        Company = new Company= value;
        //    }
        //}


        public static Usuario Usuario { get; set; }
        private static User user;
        public static User User
        {
            get { return user; }
            set 
            {
                Usuario = new Usuario()
                {
                    UsuarioId = value.Id.ToString(),
                    UserName = value.UserName,
                    TipoUsuario = (TipoUsuario)value.UserType,
                    MyNombre = value.Name, 
                    Id = value.Id
                };

                try
                {
                    Usuario.Modulos = new PermisoCL().GetAllModules(Usuario);
                }
                catch
                {
                    Usuario.Modulos = new List<Modulo>();
                }

                user = value; 
            }
        }

    }
}
