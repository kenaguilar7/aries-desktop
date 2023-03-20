using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaPresentacion.Conf;
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
            CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            
            using (var manager = new UpdateManager(ConfigurationManager.ConnectionStrings["UpdateServerString"].ConnectionString))
            {
                await manager.UpdateApp();
            }
            EnvironmentVariable.ApiUrl = ConfigurationManager.ConnectionStrings["HttpBaseUrl"].ConnectionString;
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
        public static List<Compañia> Compañias { get; set; } = new List<Compañia>();
        public static Usuario Usuario { get; set; }
        public static User User { get; set;  }
        public static Compañia Company { get; set; }
        public static Company NewCompany { get; set; }

        public static ConnectionString ConnectionString = new ConnectionString();
        public static string BaseUrl = ConfigurationManager.ConnectionStrings["HttpBaseUrl"].ConnectionString; 
        
    }
}
