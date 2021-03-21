using CapaDatos.Conexion;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDatos.Daos
{
    public class PermisoDAO
    {
        private readonly Manejador manejador = new Manejador();

        public Boolean InsertCompany(List<Compañia> compañias, Usuario asignacion, Usuario usuario)
        {
            int contador = compañias.Count;
            foreach (var compani in compañias)
            {
                ///agregar el ignore
                var sql = "INSERT INTO companies_permission (user_id,company_id,updated_by) VALUES (@user_id,@company_id,@updated_by) " +
                    "ON DUPLICATE KEY UPDATE updated_by = @updated_by , updated_at = NOW(), active = 1";

                var parametros = new List<Parametro> {
                    new Parametro("@user_id", asignacion.UsuarioId),
                    new Parametro("@company_id", compani.Codigo),
                    new Parametro("@updated_by", usuario.UsuarioId)
                };

                contador = (manejador.Ejecutar(sql, parametros, CommandType.Text) == 1) ? contador - 1 : contador;

            }
            return (contador == 0) ? true : false;
        }
        public bool UpdatePermisos(List<Modulo> lst, Usuario actualizante, Usuario actualizador)
        {
            ///Actualizar por partes
            ///Primero la ventana
            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    foreach (var item in lst)
                    {
                        foreach (var ventana in item.LstVentanas)
                        {

                            var sql = "INSERT INTO windows_permission(user_id, module_id, window_id, u_insert, u_update, u_remove,u_list, updated_by) " +
                                       "VALUES(@user_id, @module_id, @window_id, @u_insert, @u_update, @u_remove, @u_list, @updated_by) " +
                                       "ON DUPLICATE KEY UPDATE " +
                                       "u_insert = @u_insert , " +
                                       "u_update = @u_update ," +
                                       "u_remove = @u_remove, " +
                                       "u_list = @u_list, " +
                                       "updated_by = @updated_by, " +
                                       "updated_at = NOW() ";

                            using (MySqlCommand cmd = new MySqlCommand(sql, tr.Connection, tr))
                            {
                                //primero se actualiza el mes y se pone como cerrado. 
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@user_id", actualizante.UsuarioId);
                                cmd.Parameters.AddWithValue("@module_id", item.Codigo);
                                cmd.Parameters.AddWithValue("@window_id", ventana.Code);
                                cmd.Parameters.AddWithValue("@u_insert", ventana.CRUDInsert.TienePermiso);
                                cmd.Parameters.AddWithValue("@u_update", ventana.CRUDUpdate.TienePermiso);
                                cmd.Parameters.AddWithValue("@u_remove", ventana.CRUDDeleted.TienePermiso);
                                cmd.Parameters.AddWithValue("@u_list", ventana.CRUDLIst.TienePermiso);
                                cmd.Parameters.AddWithValue("@updated_by", actualizador.UsuarioId);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return true;
        }
        public bool RemoveCompany(List<Compañia> compañias, Usuario asignacion, Usuario usuario)
        {
            int contador = compañias.Count;
            foreach (var compani in compañias)
            {
                ///agregar el ignore
                var sql = "UPDATE companies_permission SET active = 0, updated_by = @updated_by  WHERE user_id = @user_id AND company_id = @company_id; ";

                var parametros = new List<Parametro> {
                    new Parametro("@user_id", asignacion.UsuarioId),
                    new Parametro("@company_id", compani.Codigo),
                    new Parametro("@updated_by", usuario.UsuarioId)
                };

                contador = (manejador.Ejecutar(sql, parametros, CommandType.Text) == 1) ? contador-- : contador;

            }
            return (contador == 0) ? true : false;
        }
        public List<Modulo> GetAllModules(Usuario usuario)
        {
            var retorno = new List<Modulo>();

            ///Traer todos los modulos 
            var sql = "SELECT " +
                      "T0.module_id," +
                      "T0.internal_name, " +
                      "T0.external_name, " +
                      "T0.users + 0, " +
                      "IF(((SELECT COUNT(*) FROM windows_permission T4 WHERE T4.module_id = T0.module_id AND T4.user_id = @user_id AND (T4.u_insert = 1 OR T4.u_update = 1 OR T4.u_remove = 1 OR T4.u_list = 1)) > 0) " +
                      "OR((SELECT T3.user_type + 0 FROM users T3 WHERE T3.user_id = @user_id LIMIT 1) = 2) , 1,0) AS 'TIENE ACCESO'" +
                      "FROM modules T0 WHERE T0.deleted = 0 AND T0.active = 1";


            DataTable dt = manejador.Listado(sql, new Parametro("@user_id", usuario.UsuarioId), CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                object[] vs = item.ItemArray;

                ///T0.code,T0.internal_name, T0.external_name
                ///
                var modulo = new Modulo();
                modulo.Codigo = Convert.ToInt32(vs[0]);
                modulo.NombreInterno = Convert.ToString(vs[1]);
                modulo.NombreExterno = Convert.ToString(vs[2]);
                modulo.TipoUsario = (TipoUsuario)Convert.ToInt16(vs[3]);
                modulo.TienePermiso = Convert.ToBoolean(vs[4]);
                GetVentanas(ref modulo, usuario);
                retorno.Add(modulo);

            }
            return retorno;
        }
        private void GetVentanas(ref Modulo modulo, Usuario usuario)
        {

            var sql = "SELECT " +
              /*0*/   "T0.window_id," +
              /*1*/   "T0.internal_name, " +
              /*2*/   "T0.external_name," +
              /*3*/   "T0.comments," +
              /*4*/   "T0.active  ," +
              /*5*/   "IFNULL((SELECT u_insert FROM windows_permission T10 WHERE  T10.window_id = T0.window_id AND T10.module_id = @module_id AND T10.user_id = @user_id LIMIT 1), 0)," +
              /*6*/   "IFNULL((SELECT u_update FROM windows_permission T10 WHERE  T10.window_id = T0.window_id AND T10.module_id = @module_id AND T10.user_id = @user_id LIMIT 1), 0)," +
              /*7*/   "IFNULL((SELECT u_remove FROM windows_permission T10 WHERE  T10.window_id = T0.window_id AND T10.module_id = @module_id AND T10.user_id = @user_id LIMIT 1), 0)," +
              /*8*/  "IFNULL((SELECT u_list FROM windows_permission T10 WHERE  T10.window_id = T0.window_id AND T10.module_id = @module_id AND T10.user_id = @user_id LIMIT 1), 0)," +
              /*9*/   "IF(((SELECT COUNT(*) FROM windows_permission T4 WHERE  T4.window_id = T0.window_id AND T4.module_id = T0.module_id AND T4.user_id = @user_id AND (T4.u_insert = 1 OR T4.u_update = 1 OR T4.u_remove = 1 OR T4.u_list = 1)) > 0) " +
                      "OR((SELECT T3.user_type + 0 FROM users T3 WHERE T3.user_id = @user_id LIMIT 1) = 2) , 1,0) AS 'TIENE ACCESO' " +
                      "FROM windows T0 WHERE T0.deleted = 0 AND T0.module_id = @module_id";



            DataTable dt = manejador.Listado(sql,
                new List<Parametro>() { new Parametro("@user_id", usuario.UsuarioId),
                new Parametro("@module_id", modulo.Codigo)}, CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                object[] vs = item.ItemArray;


                var ventana = new Ventana
                    (
                    ventanaInfo: (VentanaInfo)Convert.ToInt32(vs[0]),
                    code: Convert.ToInt32(vs[0]),
                    nombreInterno: Convert.ToString(vs[1]),
                    nombreExterno: Convert.ToString(vs[2]),
                    comentarios: Convert.ToString(vs[3]),
                    activa: Convert.ToBoolean(vs[4]),
                    tienePermiso: Convert.ToBoolean(vs[9]),
                    cRUDInsert: new CRUDItem(CRUDName.Insertar, Convert.ToBoolean(vs[5])),
                    cRUDUpdate: new CRUDItem(CRUDName.Actualizar, Convert.ToBoolean(vs[6])),
                    cRUDDeleted: new CRUDItem(CRUDName.Eliminar, Convert.ToBoolean(vs[7])),
                    cRUDLIst: new CRUDItem(CRUDName.Listar, Convert.ToBoolean(vs[8]))
                    );

                modulo.LstVentanas.Add(ventana);

            }

        }
    }
}
