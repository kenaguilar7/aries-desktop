using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Ventanas
{
    public class Modulo : IPermiso
    {
        public int Codigo { get; set; }
        public String NombreInterno { get; set; }
        public String NombreExterno { get; set; }
        public TipoUsuario TipoUsario { get; set; }
        public bool TienePermiso { get; set; }
        public List<Ventana> LstVentanas { get; set; } = new List<Ventana>();

        public Modulo(int codigo, string nombreInterno, string nombreExterno, 
                     TipoUsuario tipoUsario, bool activo, List<Ventana> lstVentanas)
        {
            Codigo = codigo;
            NombreInterno = nombreInterno;
            NombreExterno = nombreExterno;
            TipoUsario = tipoUsario;
            TienePermiso = activo;
            LstVentanas = lstVentanas;
        }

        public Modulo()
        {
        }
        public override string ToString()
        {
            return NombreExterno; 
        }
    }
}
