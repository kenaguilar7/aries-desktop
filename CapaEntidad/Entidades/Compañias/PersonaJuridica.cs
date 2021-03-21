using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Compañias
{
    public class PersonaJuridica : Compañia
    {
        private String representanteLegal;
        private String IDRepresentante;
        public PersonaJuridica()
        {
        }

        public PersonaJuridica(TipoID tipoID, string numeroId, string nombre, TipoMonedaCompañia TipoMoneda, string direccion,
                                  string[] telefono, string web, string correo, string observaciones,
                                  String representanteLegal, String IDRepresentante, string codigo = "", Boolean activo = true) :
                                  base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones,codigo, activo)
        {
            this.representanteLegal = representanteLegal;
            this.IDRepresentante = IDRepresentante;
        }

        public String MyRepresentanteLegal
        {
            get { return representanteLegal; }
            set { representanteLegal = value; }
        }

        public String MyIDRepresentanteLegal
        {
            get { return IDRepresentante; }
            set { IDRepresentante = value; }
        }

    }
}
