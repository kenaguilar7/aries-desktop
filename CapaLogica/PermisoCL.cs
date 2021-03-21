using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class PermisoCL
    {
        readonly private PermisoDAO permisoDAO = new PermisoDAO();

        public Boolean InsertCompany(List<Compañia> compañias, Usuario asignacion, Usuario usuario)
        {
            return permisoDAO.InsertCompany(compañias, asignacion, usuario); 
        }
        public Boolean RemoveCompany(List<Compañia> compañias, Usuario asignacion, Usuario usuario) {
            return permisoDAO.RemoveCompany(compañias, asignacion, usuario); 
        }
        public List<Modulo> GetAllModules(Usuario usuario) {
            return permisoDAO.GetAllModules(usuario); 
        }
        public Boolean UpdatePermisos(List<Modulo> lst, Usuario actualizante, Usuario actualizador) {
            return permisoDAO.UpdatePermisos(lst, actualizante, actualizador); 
        }
    }
}
