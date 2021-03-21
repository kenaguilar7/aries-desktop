using CapaDatos.Conexion;
using CapaEntidad.Entidades.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Daos
{
    public class CorreoDao
    {


        Manejador manejador = new Manejador();
        public DataTable Get()
        {

            var sql = "select nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado,ultimo_envio from usuarios_correo order by mailusuario_id desc";
            return manejador.Listado(sql, CommandType.Text);
        }
        public Boolean Insert(UsuarioTemporal usuarioTemporal)
        {

            var sql = "insert into usuarios_correo (nombre,apellido,correo_electronico,correo_copia,asunto,titulo,mensaje,estado) VALUES(@nombre, @apellido, @correo_electronico,@correo_copia, @asunto,@titulo, @mensaje,@estado)";
            var parametros = new List<Parametro> {
                new Parametro("@nombre",usuarioTemporal.Nombre ),
                new Parametro("@apellido",usuarioTemporal.Apellido),
                new Parametro("@correo_electronico",usuarioTemporal.CorreoElectronico ),
                new Parametro("@correo_copia",usuarioTemporal.CCopy ),
                new Parametro("@asunto", usuarioTemporal.Asunto),
                new Parametro("@titulo", usuarioTemporal.Titulo),
                new Parametro("@mensaje",usuarioTemporal.Cuerpo),
                new Parametro("@estado",Convert.ToInt16( usuarioTemporal.EstadoEnvio))

            };
            try
            {
                var resl = manejador.Ejecutar(sql, parametros, CommandType.Text);

                return (resl == 0) ? false : true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
