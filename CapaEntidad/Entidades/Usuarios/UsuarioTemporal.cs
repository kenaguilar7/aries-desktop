using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Usuarios
{
    public class UsuarioTemporal
    {

        public String Nombre { get; set; }
        public String Apellido { get; set; }
        public String CorreoElectronico { get; set; }
        public String CCopy { get; set; }
        public String Asunto { get; set; }
        public String Titulo { get; set; }
        public String Cuerpo { get; set; }
        public DateTime UltimoEnvio { get; set; }
        public EstadoEnvio EstadoEnvio { get; set; }

        public UsuarioTemporal(string nombre, string apellido, string correoElectronico, String ccopy, string asunto, string cuerpo, DateTime ultimoEnvio, EstadoEnvio estadoEnvio)
        {
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correoElectronico;
            CCopy = ccopy;
            Asunto = asunto;
            Cuerpo = cuerpo;
            UltimoEnvio = ultimoEnvio;
            EstadoEnvio = estadoEnvio;
        }

        public UsuarioTemporal(string nombre, string apellido, string correoElectronico, String ccopy, string asunto, String titulo, string cuerpo)
        {
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correoElectronico;
            Titulo = titulo; 
            CCopy = ccopy;
            Asunto = asunto;
            Cuerpo = cuerpo;
        }
    }
}
