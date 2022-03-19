using CapaDatos.Conexion;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Daos
{
    public class FechaTransaccionDao
    {
        Manejador manejador = new Manejador();

        public List<FechaTransaccion> GetAll(Compañia t, Usuario user)
        {
            try
            {
                List<FechaTransaccion> retorno = new List<FechaTransaccion>();
                var sql = "SELECT accounting_months_id, month_report, closed FROM accounting_months WHERE company_id = @company_id";
                DataTable dt = manejador.Listado(sql, new Parametro("@company_id", t.Codigo), CommandType.Text);

                foreach (DataRow item in dt.Rows)
                {
                    object[] vs = item.ItemArray;

                    retorno.Add(new FechaTransaccion(
                            id: Convert.ToInt32(vs[0]),
                            fecha: Convert.ToDateTime(vs[1]),
                            cerrada: Convert.ToBoolean(vs[2])
                        ));
                }
                return retorno;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetDataTable(Compañia t, Usuario user)
        {
            var sql = "SET lc_time_names = 'es_ES';" +
                      "SELECT DATE_FORMAT(ac.month_report,'%M %Y') AS 'Mes'," +
                      "IF(ac.closed, 'Cerrado','Abierto') AS 'Estado'," +
                      "DATE_FORMAT(ac.created_at,'%d %M %Y') AS 'Creado '," +
                      "IF(ac.closed,DATE_FORMAT(ac.updated_at,'%d %M %Y'), '') AS 'Cerrado'," +
                      "(SELECT user_name FROM users us WHERE  us.user_id = ac.updated_by  LIMIT 1) AS 'Usuario' " +
                      "FROM accounting_months ac " +
                      "WHERE ac.company_id = @company_id AND ac.active = 1 ORDER BY ac.month_report DESC";
            return manejador.Listado(sql, new Parametro("@company_id", t.Codigo), CommandType.Text);

        }
        public Boolean Insert(FechaTransaccion t, Compañia compañia, Usuario user, out String mensaje)
        {

            if (!Guachi.Consultar(user, VentanaInfo.FormAdminMeses, CRUDName.Insertar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            try
            {

                var sql = "INSERT INTO accounting_months(month_report,company_id,updated_by) VALUES(@month_report,@company_id,@updated_by);";

                var parametros = new List<Parametro>() {
                    new Parametro("@month_report", t.Fecha),
                    new Parametro("@company_id", compañia.Codigo),
                    new Parametro("@updated_by", user.UsuarioId)
                    };

                if (manejador.Ejecutar(sql, parametros, CommandType.Text) == 0)
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
        /// <summary>
        /// Se hace todo en un metodo porque debemos de asegurarnos que el mes quede cerrado y los datos quedeb tambien guardados
        /// primero se intenta cerrar el mes 
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="compañia"></param>
        /// <param name="user"></param>
        /// <param name="lstCuentas"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public Boolean CerrarMes(FechaTransaccion t, Compañia compañia, Usuario user, List<Cuenta> lstCuentas, out String mensaje)
        {

            /**
             * Primero se consulta si el usuario tiene permisos para hacer la accción
             * 
             */

            if (!Guachi.Consultar(user, VentanaInfo.FormAsientos, CRUDName.Actualizar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }


            /**
             * Se abre una transacción que sera la encargada de insertar los datos 
             * 
             * primero cerramos el mes y nos aseguramos que de verdad se haya cerrado
             */

            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var sql = "UPDATE accounting_months SET closed = @closed, updated_by = @updated_by " +
                              "WHERE company_id = @company_id AND accounting_months_id = @accounting_months_id ";

                    using (MySqlCommand cmd = new MySqlCommand(sql, tr.Connection, tr))
                    {
                        //primero se actualiza el mes y se pone como cerrado. 

                        cmd.Parameters.AddWithValue("@closed", Convert.ToByte(t.Cerrada));
                        cmd.Parameters.AddWithValue("@updated_by", user.UsuarioId);
                        cmd.Parameters.AddWithValue("@company_id", compañia.Codigo);
                        cmd.Parameters.AddWithValue("@accounting_months_id", t.Id);

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            mensaje = "No se pudo cerrar el mes";
                            return false;
                        }

                    }

                    var sql2 = "UPDATE accounts SET previous_balance_c = @previous_balance_c," +
                               "previous_balance_d = @previous_balance_d, updated_by = @updated_by " +
                               "WHERE account_id = @account_id AND company_id = @company_id;";

                    using (MySqlCommand cmd = new MySqlCommand(sql2, tr.Connection, tr))
                    {
                        /**
                         * usamos la lista de cuentas pasadas por parametro, estas cuentas son 
                         * todas las que tienen que ser actualizadas
                         */

                        foreach (var item in lstCuentas)
                        {
                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@previous_balance_c", item.SaldoActualColones);
                            cmd.Parameters.AddWithValue("@previous_balance_d", item.SaldoActualDolares);
                            cmd.Parameters.AddWithValue("@updated_by", user.UsuarioId);
                            cmd.Parameters.AddWithValue("@account_id", item.Id);
                            cmd.Parameters.AddWithValue("@company_id", compañia.Codigo);
                            cmd.ExecuteNonQuery();

                        }

                    }

                    tr.Commit();
                    mensaje = "Datos actualizados correctamente";
                    return true;
                }
                catch (Exception ex)
                {
                    tr.Rollback();

                    mensaje = ex.Message;
                    return false;
                }
            }
        }
    }
}
