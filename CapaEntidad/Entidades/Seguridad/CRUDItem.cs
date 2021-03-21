using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Seguridad
{
    public class CRUDItem : IPermiso
    {
        public CRUDName Nombre { get; set; }
        public bool TienePermiso { get; set; }

        public CRUDItem(CRUDName nombre, bool tienePermiso)
        {
            Nombre = nombre;
            TienePermiso = tienePermiso;
        }

        public CRUDItem()
        {
        }
    }
}
