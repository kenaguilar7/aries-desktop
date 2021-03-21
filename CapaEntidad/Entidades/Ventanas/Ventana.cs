using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Ventanas
{
    public class Ventana : IPermiso
    {
        public VentanaInfo VentanaInfo { get; set; }
        public int Code { get; set; }
        public String NombreInterno { get; set; }
        public String NombreExterno { get; set; }
        public String Comentarios { get; set; }
        public bool Activa { get; set; }
        public bool TienePermiso { get; set; }
        public CRUDItem CRUDInsert { get; set; }
        public CRUDItem CRUDUpdate { get; set; }
        public CRUDItem CRUDDeleted { get; set; }
        public CRUDItem CRUDLIst { get; set; }
        public Ventana(int code, string nombreInterno,
                       string nombreExterno, string comentarios,
                       bool activa, Boolean tienePermiso)
        {
            Code = code;
            NombreInterno = nombreInterno;
            NombreExterno = nombreExterno;
            Comentarios = comentarios;
            Activa = activa;
            TienePermiso = tienePermiso;
        }

        public Ventana(int code, string nombreInterno, string nombreExterno, 
            string comentarios, bool activa, bool tienePermiso, 
            CRUDItem cRUDInsert, CRUDItem cRUDUpdate, CRUDItem cRUDDeleted, CRUDItem cRUDLIst, VentanaInfo ventanaInfo) 
            : this(code, nombreInterno, nombreExterno, comentarios, activa, tienePermiso)
        {
            CRUDInsert = cRUDInsert;
            CRUDUpdate = cRUDUpdate;
            CRUDDeleted = cRUDDeleted;
            CRUDLIst = cRUDLIst;
            VentanaInfo = ventanaInfo;
        }

        public Ventana()
        {
        }
        public CRUDItem FindCRUD(CRUDName cRUDItem)
        {

            switch (cRUDItem)
            {
                case CRUDName.Insertar: return CRUDInsert;

                case CRUDName.Actualizar: return CRUDUpdate;

                case CRUDName.Eliminar: return CRUDDeleted;

                case CRUDName.Listar: return CRUDLIst;

                default: return null;

            }
        }
    }
}
