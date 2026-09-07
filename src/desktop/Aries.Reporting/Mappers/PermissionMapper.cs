using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Permissions;
using Aries.Reporting.Entidades.Seguridad;
using Aries.Reporting.Entidades.Ventanas;
using Aries.Reporting.Enumeradores;

namespace Aries.Reporting.Mappers
{
    public static class PermissionMapper
    {
        public static List<Modulo> ToModulos(IEnumerable<ModulePermission> modules)
        {
            if (modules == null)
                return new List<Modulo>();

            return modules.Select(ToModulo).ToList();
        }

        public static Modulo ToModulo(ModulePermission module)
        {
            if (module == null)
                return null;

            var modulo = new Modulo
            {
                Codigo = module.Id,
                NombreInterno = module.InternalName,
                NombreExterno = module.ExternalName,
                TipoUsario = (TipoUsuario)module.UserType,
                TienePermiso = module.HasAccess
            };

            if (module.Windows == null)
                return modulo;

            foreach (var window in module.Windows)
            {
                modulo.LstVentanas.Add(new Ventana(
                    ventanaInfo: (VentanaInfo)window.Id,
                    code: window.Id,
                    nombreInterno: window.InternalName,
                    nombreExterno: window.ExternalName,
                    comentarios: window.Comments,
                    activa: window.Active,
                    tienePermiso: window.HasAccess,
                    cRUDInsert: new CRUDItem(CRUDName.Insertar, window.CanInsert),
                    cRUDUpdate: new CRUDItem(CRUDName.Actualizar, window.CanUpdate),
                    cRUDDeleted: new CRUDItem(CRUDName.Eliminar, window.CanRemove),
                    cRUDLIst: new CRUDItem(CRUDName.Listar, window.CanList)));
            }

            return modulo;
        }

        public static List<ModulePermission> ToModulePermissions(IEnumerable<Modulo> modulos)
        {
            if (modulos == null)
                return new List<ModulePermission>();

            return modulos.Select(ToModulePermission).ToList();
        }

        public static ModulePermission ToModulePermission(Modulo modulo)
        {
            if (modulo == null)
                return null;

            var module = new ModulePermission
            {
                Id = modulo.Codigo,
                InternalName = modulo.NombreInterno,
                ExternalName = modulo.NombreExterno,
                UserType = (int)modulo.TipoUsario,
                HasAccess = modulo.TienePermiso
            };

            if (modulo.LstVentanas == null)
                return module;

            foreach (var ventana in modulo.LstVentanas)
            {
                module.Windows.Add(new WindowPermission
                {
                    Id = ventana.Code,
                    InternalName = ventana.NombreInterno,
                    ExternalName = ventana.NombreExterno,
                    Comments = ventana.Comentarios,
                    Active = ventana.Activa,
                    HasAccess = ventana.TienePermiso,
                    CanInsert = ventana.CRUDInsert != null && ventana.CRUDInsert.TienePermiso,
                    CanUpdate = ventana.CRUDUpdate != null && ventana.CRUDUpdate.TienePermiso,
                    CanRemove = ventana.CRUDDeleted != null && ventana.CRUDDeleted.TienePermiso,
                    CanList = ventana.CRUDLIst != null && ventana.CRUDLIst.TienePermiso
                });
            }

            return module;
        }
    }
}
