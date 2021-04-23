using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Usuarios
{
    public class Usuario : BaseModel
    {
        // public String UsuarioId { get; set;  }
        
        public String UsuarioId { get; set; }///Cambiar a decimal
        public String UserName { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public String MyCedula { get; set; }
        public String MyNombre { get; set; }
        public String MyApellidoPaterno { get; set; }
        public String MyApellidoMaterno { get; set; }
        public String MyTelefono { get; set; }
        public String MyMail { get; set; }
        public String MyNotas { get; set; }
        public String MyClave { get; set; }
        public String MyUpdated { get; set; }
        public Boolean MyActivo { get; set; }
        //public Boolean MyAdmin { get; set; }
        public DateTime MyFechaCreacion { get; set; }
        public DateTime MyFechaActualizacion { get; set; }
        public List<Modulo> Modulos { get; set; } = new List<Modulo>(); 

        public Usuario(string myCedula, string myNombre, String username, TipoUsuario tipoUsuario, string myApellidoPaterno, string myApellidoMaterno,
                       string myTelefono, string myMail, string myNotas, string myClave, bool myActivo,
                       DateTime myFechaCreacion, DateTime myFechaActualizacion, string myID = "null", String myUpdated = "")
        {
            UsuarioId = myID;
            MyCedula = myCedula ?? throw new ArgumentNullException("CEDULA ");
            MyNombre = myNombre ?? throw new ArgumentNullException("NOMBRE");
            UserName = username;
            TipoUsuario = tipoUsuario;
            MyApellidoPaterno = myApellidoPaterno;
            MyApellidoMaterno = myApellidoMaterno;
            MyTelefono = myTelefono;
            MyMail = myMail;
            MyNotas = myNotas;
            MyClave = myClave ?? throw new ArgumentNullException("CLAVE");
            MyActivo = myActivo;
            MyFechaCreacion = myFechaCreacion;
            MyFechaActualizacion = myFechaActualizacion;
            //MyAdmin = myAdmin;
            MyUpdated = myUpdated;
        }
        public Usuario(string myCedula, string myNombre, String username, TipoUsuario tipoUsuario, string myApellidoPaterno, string myApellidoMaterno,
                   string myTelefono, string myMail, string myNotas, string myClave, bool myActivo, string myID = "null",  String myUpdated = "")
        {
            UsuarioId = myID;
            MyCedula = myCedula ?? throw new ArgumentNullException("CEDULA ");
            MyNombre = myNombre ?? throw new ArgumentNullException("NOMBRE");
            UserName = username;
            TipoUsuario = tipoUsuario;
            MyApellidoPaterno = myApellidoPaterno;
            MyApellidoMaterno = myApellidoMaterno;
            MyTelefono = myTelefono;
            MyMail = myMail;
            MyNotas = myNotas;
            MyClave = myClave ?? throw new ArgumentNullException("CLAVE");
            MyActivo = myActivo;
            //MyAdmin = myAdmin;
            MyUpdated = myUpdated;
        }
        public Usuario() { }
        public override string ToString()
        {
            return $"{MyNombre} {MyApellidoPaterno} {MyApellidoMaterno}"; 
        }

        //public CRUD GetCRUD(Ventana ventana) {

        //    var r = Modulos.Find(x => x.Ventana.Code == ventana.Code);
        //    return r.CRUD;

        //}
    }
}
