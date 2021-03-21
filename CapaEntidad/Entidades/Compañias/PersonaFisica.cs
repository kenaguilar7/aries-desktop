using CapaEntidad.Enumeradores;
using System;

namespace CapaEntidad.Entidades.Compañias
{
    public class PersonaFisica : Compañia
    {
        private String apellidoPaterno;
        private String apellidoMaterno;


        public PersonaFisica()
        {
        }

        public PersonaFisica(TipoID tipoID, string numeroId, string nombre, TipoMonedaCompañia TipoMoneda,  string direccion,
                                string[] telefono, string web, string correo, string observaciones, String apellidoPaterno, String apellidoMaterno, string codigo = "", Boolean activo = true) :
                                base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones, codigo, activo)
        {
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
        }
        public PersonaFisica(String apelledoPaterno, String apellidoMaterno)
        {
            this.apellidoPaterno = apelledoPaterno;
            this.MyApellidoMaterno = apellidoMaterno;
        }

        public String MyApellidoPaterno
        {
            get { return apellidoPaterno; }
            set { apellidoPaterno = value; }
        }

        public String MyApellidoMaterno
        {
            get { return apellidoMaterno; }
            set { apellidoMaterno = value; }
        }

       
    }
}
