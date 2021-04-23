using CapaDatos.Conexion;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDatos.Daos
{
    public class UsuarioDao
    {
        Manejador manejador = new Manejador();
        public Boolean Insert(Usuario t, Usuario user, out String mensaje)
        {
            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroUsuario, CRUDName.Insertar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            var sql = "INSERT INTO users(number_id,  name,  user_name , user_type , lastname_p , lastname_m,  phone_number , mail , notes,  password,  updated_by,  active) " +
                                 "VALUES(@number_id, @name, @user_name, @user_type, @lastname_p, @lastname_m, @phone_number, @mail, @notes, @password, @updated_by, @active);";
            List<Parametro> lst = new List<Parametro>();
            try
            {

                lst.Add(new Parametro("@number_id", t.MyCedula));
                lst.Add(new Parametro("@name", t.MyNombre));
                lst.Add(new Parametro("@user_name", t.UserName));
                lst.Add(new Parametro("@user_type", (int)t.TipoUsuario));
                lst.Add(new Parametro("@lastname_p", t.MyApellidoPaterno));
                lst.Add(new Parametro("@lastname_m", t.MyApellidoMaterno));
                lst.Add(new Parametro("@phone_number", t.MyTelefono));
                lst.Add(new Parametro("@mail", t.MyMail));
                lst.Add(new Parametro("@notes", t.MyNotas));
                lst.Add(new Parametro("@password", t.MyClave));
                lst.Add(new Parametro("@updated_by", user.UsuarioId));
                lst.Add(new Parametro("@active", Convert.ToInt16(t.MyActivo)));

                if (manejador.Ejecutar(sql, lst, CommandType.Text) == 0)
                {
                    mensaje = "No se guardaron datos";
                    return false;
                }
                else
                {
                    mensaje = "Datos guardados correctamente";
                    return true;
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }
        public Boolean Update(Usuario t, Usuario user, out String mensaje)
        {
            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroUsuario, CRUDName.Actualizar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            try
            {
                var sql = "UPDATE users SET number_id = @number_id,name=@name,user_name=@user_name,user_type=@user_type,lastname_p=@lastname_p,lastname_m=@lastname_m,phone_number=@phone_number," +
                      "mail=@mail,notes=@notes,password=@password,updated_by=@updated_by,active=@active WHERE user_id = @user_id;";
                List<Parametro> lst = new List<Parametro>();

                lst.Add(new Parametro("@user_id", t.UsuarioId));
                lst.Add(new Parametro("@number_id", t.MyCedula));
                lst.Add(new Parametro("@name", t.MyNombre));
                lst.Add(new Parametro("@user_name", t.UserName));
                lst.Add(new Parametro("@user_type", (int)t.TipoUsuario));
                lst.Add(new Parametro("@lastname_p", t.MyApellidoPaterno));
                lst.Add(new Parametro("@lastname_m", t.MyApellidoMaterno));
                lst.Add(new Parametro("@phone_number", t.MyTelefono));
                lst.Add(new Parametro("@mail", t.MyMail));
                lst.Add(new Parametro("@notes", t.MyNotas));
                lst.Add(new Parametro("@password", t.MyClave));
                lst.Add(new Parametro("@updated_by", user.UsuarioId));
                lst.Add(new Parametro("@active", Convert.ToInt16(t.MyActivo)));

                if (manejador.Ejecutar(sql, lst, CommandType.Text) == 0)
                {
                    mensaje = "No se actualizaron datos";
                    return false;
                }
                else
                {
                    mensaje = "Datos actualizados correctamente";
                    return true;
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }
        public List<Usuario> GetAll()
        {
            //if (!Guachi.Consultar(usuario, VentanaInfo.FormMaestroUsuario, CRUDName.Listar))
            //{
            //    mensaje = "Acceso denegado!!!";
            //    return false;
            //}

            List<Usuario> retorno = new List<Usuario>();

            var sql = "SELECT " +
                "user_id, " +            ///0
                "user_name, " +          ///1
                "user_type+0, " +        ///2
                "number_id, " +          ///3
                "name, " +               ///4
                "lastname_p, " +         ///5
                "lastname_m, " +         ///6
                "phone_number, " +       ///7
                "mail, " +               ///8
                "notes, " +              ///9
                "created_at, " +         ///10
                "updated_at, " +         ///11
                "(SELECT user_name FROM users us WHERE us.user_id = updated_by LIMIT 1)," +         ///12
                "active, " +              ///13
                "password " +             ///14
                "FROM users";

            var dt = manejador.Listado(sql, CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                Object[] vs = item.ItemArray;

                var user = new Usuario()
                {

                    Id = Convert.ToInt32(vs[0]),
                    UsuarioId = Convert.ToString(vs[0]), ///"user_id, " +            ///0   
                    UserName = Convert.ToString(vs[1]), ///"user_name, " +          ///1
                    TipoUsuario = (TipoUsuario) Convert.ToInt16(vs[2]), ///"user_type+0, " +        ///2
                    MyCedula = Convert.ToString(vs[3]), ///"number_id, " +          ///3
                    MyNombre = Convert.ToString(vs[4]), ///"name, " +               ///4
                    MyApellidoPaterno = Convert.ToString(vs[5]), ///"lastname_p, " +         ///5
                    MyApellidoMaterno = Convert.ToString(vs[6]), ///"lastname_m, " +         ///6
                    MyTelefono = Convert.ToString(vs[7]), ///"phone_number, " +       ///7
                    MyMail = Convert.ToString(vs[8]), ///"mail, " +               ///8
                    MyNotas = Convert.ToString(vs[9]), ///"notes, " +              ///9
                    /* myAdmin= Convert.ToBoolean(vs[10]),            */ ///"admin, " +              ///10
                    MyFechaCreacion = Convert.ToDateTime(vs[10]), ///"created_at, " +         ///11
                    MyFechaActualizacion = Convert.ToDateTime(
                       Convert.ToString(vs[11])), ///"updated_at, " +         ///12
                    MyUpdated = Convert.ToString(Convert.ToString(vs[12])), ///"updated_by ," +         ///13
                    MyActivo = Convert.ToBoolean(vs[13]), ///"active, "+              ///14
                    MyClave = Convert.ToString(vs[14]) ///"password "+             ///15
                };
                retorno.Add(user);
            };
            

            return retorno;
        }
        public DataTable Login(String user, String clave)
        {
            var sql = "SELECT " +
                "user_id, " +
                "user_name, " +
                "user_type+0, " +
                "number_id, " +
                "name, " +
                "lastname_p, " +
                "lastname_m, " +
                "phone_number, " +
                "mail, " +
                "notes, " +
                "created_at, " +
                "updated_at, " +
                "updated_by ," +
                "active, " +
                "password " +
                "FROM users WHERE user_name = @user_name AND password = @password AND active = 1";

            return manejador.Listado(sql,
                                        new List<Parametro>{ new Parametro("@user_name", user),
                                                             new Parametro("@password", clave)}, CommandType.Text);
        }
        /// <summary>
        /// Retorna true si el nombre existe ya en la base de datos
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Boolean VerificarNombre(String userName)
        {

            var retorno = false;
            try
            {
                var sql = "SELECT COUNT(*) FROM users WHERE user_name = @user_name";

                using (MySqlCommand cmd = new MySqlCommand(sql, manejador.GetConnection()))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter
                    {
                        SelectCommand = cmd
                    };

                    cmd.Parameters.AddWithValue("@user_name", userName);


                    retorno = Convert.ToBoolean(cmd.ExecuteScalar());
                }


                return retorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
