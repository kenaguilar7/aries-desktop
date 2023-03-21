using AriesContador.Core.Models.Companies;
using CapaDatos.Daos;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class PermisoCL
    {
        readonly private PermisoDAO permisoDAO = new PermisoDAO();

        public Boolean InsertCompany(List<Company> compañias, Usuario asignacion, Usuario usuario)
        {
            return permisoDAO.InsertCompany(compañias, asignacion, usuario); 
        }
        public Boolean RemoveCompany(List<Company> compañias, Usuario asignacion, Usuario usuario) {
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
