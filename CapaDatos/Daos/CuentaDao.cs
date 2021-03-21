using CapaDatos.Conexion;
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
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector.Authentication;

namespace CapaDatos.Daos
{
    public class CuentaDao
    {
        private Manejador manejador = new Manejador();
        public Boolean Deleted(Cuenta cuenta, Usuario usuario, out String mensaje)
        {

            if (!Guachi.Consultar(usuario, VentanaInfo.FormMaestroCuenta, CRUDName.Eliminar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }
            var sql = "UPDATE accounts T01 " +
                "SET T01.active = 0, T01.updated_by = @updated_by " +
                "WHERE T01.account_id = @account_id AND T01.account_guide + 0 = 3 " +
                "AND(SELECT COUNT(*) " +
                    "FROM transactions_accounting T0 " +
                    "LEFT JOIN accounting_entries T1 USING(accounting_entry_id) " +
                    "LEFT JOIN accounting_months T2 USING(accounting_months_id) " +
                    "WHERE T2.active = 1 AND T2.closed = 0 AND T1.active = 1 AND T0.active = 1 " +
                    "AND T0.account_id = @account_id) = 0 LIMIT 1";
            var parametros = new List<Parametro>();
            parametros.Add(new Parametro("@account_id", cuenta.Id));
            parametros.Add(new Parametro("@updated_by", usuario.UsuarioId));

            var result = manejador.Ejecutar(sql, parametros, CommandType.Text);
            mensaje = (result == 1) ? "Cuenta eliminada correctamente" : "No se pudo eliminar la cuenta porque tiene movimientos";
            return (result == 1) ? true : false;
        }
        public List<Cuenta> GetAll(Compañia t, Usuario user = null)
        {
            var retorno = new List<Cuenta>();

            var sql = "SELECT " +
                "a.account_id, " +                 //0
                "n.name, " +                       //1    
                "IFNULL(b.account_id,0), " +       //2
                "a.previous_balance_c, " +         //3
                "a.previous_balance_d, " +         //4
                "a.account_type+0," +              //5
                "a.account_guide+0," +             //6
                "a.editable," +                    //7
                "a.detail," +                      //8
                "a.active " +                      //9
                "FROM accounts a LEFT JOIN accounts_names n on a.account_name_id = " +
                "n.account_name_id LEFT JOIN accounts b ON a.father_account = " +
                "b.account_id WHERE a.company_id = @company_id AND a.active = 1  ORDER BY a.account_id; ";

            DataTable dt = manejador.Listado(sql, new List<Parametro> { new Parametro("@company_id", t.Codigo) }, CommandType.Text);


            foreach (DataRow item in dt.Rows)
            {
                object[] vs = item.ItemArray;
                retorno.Add(new Cuenta(
                       id: Convert.ToInt32(vs[0]),
                       nombre: Convert.ToString(vs[1]),
                       padre: Convert.ToInt32(vs[2]),
                       saldoAnteriorColones: Convert.ToDecimal(vs[3]),
                       saldoAnteriorDolares: Convert.ToDecimal(vs[4]),
                       debitos: 0,
                       creditos: 0,
                       myCompania: t,
                       tipoCuenta: Cuenta.GenerarTipoCuenta(Convert.ToInt32(vs[5])),
                       indicador: (IndicadorCuenta)(Convert.ToInt32(vs[6])),
                       editable: Convert.ToBoolean(vs[7]),
                       detalle: Convert.ToString(vs[8]),
                       active: Convert.ToBoolean(vs[9])
                        ));

            }
            return retorno;
        }
        public DataTable GetInforCompletaCuentaMayor(Cuenta cuenta)
        {

            var sql = "SET lc_time_names = 'es_MX'; " +
               "SELECT " +
               "(SELECT T1.name FROM accounts_names T1 where T1.account_name_id = T3.account_name_id LIMIT 1) AS 'Nombre', " +
               "IF(T3.account_guide <> 'CUENTA AUXILIAR', 'Movimiento a hija', 'Movimiento a cuenta hija' ) AS 'Tipo Moviento'," +
               "T2.detail AS 'Detalle'," +
               "T2.reference AS 'Referencia', " +
               "T2.bill_date AS 'Fecha documento', " +
               "DATE_FORMAT(T5.month_report,'%M %Y') AS 'Mes Contable', " +
               "T4.entry_id 'Numero de Asiento'," +
               "IF(T2.balance_type+0 = 1,FORMAT(T2.balance,2), null) AS 'Debito', " +
               "IF(T2.balance_type+0 = 2,FORMAT(T2.balance,2), null) AS 'Credito', " +
               "FORMAT(T3.account_type+0,0) AS 'Saldo Actual'," +
               "FORMAT(T2.money_chance,2) AS 'Tipo Cambio'," +
               "IF(T2.money_type+0 = 2, FORMAT(T2.balance/T2.money_chance,2), FORMAT(0.00,2)) AS 'Monto Dolares',  " +
               "(SELECT T7.user_name FROM users T7 WHERE T7.user_id = T2.updated_by LIMIT 1)  AS 'Usuario registro', " +
               "T2.created_at AS 'Fecha de Registro' " +
               "FROM transactions_accounting T2 LEFT JOIN accounts T3 ON T2.account_id = T3.account_id " +
               "LEFT JOIN accounting_entries T4 ON T2.accounting_entry_id = T4.accounting_entry_id " +
               "LEFT JOIN accounting_months T5 USING(accounting_months_id) " +
               "WHERE T3.father_account = @account_id AND T2.active = 1 AND T3.active = 1 AND T4.active = 1 " +
               "ORDER BY T5.month_report, T4.entry_id";

            var parametros = new List<Parametro> {
                new Parametro("@account_id", cuenta.Id)
            };
            return manejador.Listado(sql, parametros, CommandType.Text);

        }
        public DataTable GetInfoCompletaCuentaAux(Cuenta cuenta)
        {
            var sql = "SET lc_time_names = 'es_MX'; " +
               "SELECT " +
               "(SELECT T1.name FROM accounts_names T1 where T1.account_name_id = T3.account_name_id LIMIT 1) AS 'Nombre', " +
               "IF(T3.account_guide <> 'CUENTA AUXILIAR', 'Movimiento a hija', 'Movimiento a cuenta' ) AS 'Tipo Moviento'," +
               "T2.detail AS 'Detalle'," +
               "T2.reference AS 'Referencia', " +
               "T2.bill_date AS 'Fecha Documento', " +
               "DATE_FORMAT(T5.month_report,'%M %Y') AS 'Mes Contable', " +
               "T4.entry_id 'Numero de Asiento'," +
               "IF(T2.balance_type+0 = 1,FORMAT(T2.balance,2), null) AS 'Debito', " +
               "IF(T2.balance_type+0 = 2,FORMAT(T2.balance,2), null) AS 'Credito', " +
               "FORMAT(T3.account_type+0,0) AS 'Saldo Actual'," +
               "FORMAT(T2.money_chance,2) AS 'Tipo Cambio'," +
               "IF(T2.money_type+0 = 2, FORMAT(T2.balance/T2.money_chance,2), FORMAT(0.00,2)) AS 'Monto Dolares',  " +
               "(SELECT T7.user_name FROM users T7 WHERE T7.user_id = T2.updated_by LIMIT 1)  AS 'Usuario registro', " +
               "T2.created_at AS 'Fecha de Registro' " +
               "FROM transactions_accounting T2 LEFT JOIN accounts T3 ON T2.account_id = T3.account_id " +
               "LEFT JOIN accounting_entries T4 ON T2.accounting_entry_id = T4.accounting_entry_id " +
               "LEFT JOIN accounting_months T5 USING(accounting_months_id) " +
               "WHERE T3.account_id = @account_id AND T2.active = 1 AND T3.active = 1 AND T4.active = 1 "+
               "ORDER BY T5.month_report, T4.entry_id";


            var parametros = new List<Parametro> {
                new Parametro("@account_id", cuenta.Id)
            };
            return manejador.Listado(sql, parametros, CommandType.Text);


        }
        public Boolean Insert(ref Cuenta t, Cuenta padre, Usuario user, out String mensaje)
        {
            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroCuenta, CRUDName.Insertar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }
            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {

                try
                {

                    ///Agregamos el nombre a la base de datos
                    ///Si el nombre no exite no se crea
                    var sql1 = "INSERT IGNORE INTO accounts_names(name) VALUES(@name);";
                    using (MySqlCommand cmd = new MySqlCommand(sql1, tr.Connection, tr))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter
                        {
                            SelectCommand = cmd
                        };

                        cmd.Parameters.AddWithValue("@name", t.Nombre);
                        cmd.ExecuteNonQuery();
                    }


                    ///Insertamos la cuenta en  la base de datos 
                    ///este query nos devuelve el id de la nueva cuenta 
                    var sql2 = "INSERT INTO accounts (account_name_id,previous_balance_c,previous_balance_d,father_account,company_id,account_type,detail,editable,updated_by) " +
                              "VALUES((SELECT account_name_id FROM accounts_names WHERE name = @name LIMIT 1)," +
                              "@previous_balance_c," +
                              "@previous_balance_d," +
                              "@father_account," +
                              "@company_id," +
                              "@account_type," +
                              "@detail," +
                              "@editable," +
                              "@updated_by);" +
                              "SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(sql2, tr.Connection, tr))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@name", t.Nombre);
                        cmd.Parameters.AddWithValue("@father_account", padre.Id);
                        cmd.Parameters.AddWithValue("@previous_balance_c", t.SaldoAnteriorColones);
                        cmd.Parameters.AddWithValue("@previous_balance_d", t.SaldoAnteriorDolares);
                        cmd.Parameters.AddWithValue("@company_id", t.MyCompania.Codigo);
                        cmd.Parameters.AddWithValue("@account_type", t.TipoCuenta.TipoCuenta.ToString().Replace('_', ' ').ToUpper());
                        cmd.Parameters.AddWithValue("@detail", t.Detalle);
                        cmd.Parameters.AddWithValue("@editable", t.Editable);
                        cmd.Parameters.AddWithValue("@updated_by", user.UsuarioId);
                        MySqlDataAdapter da = new MySqlDataAdapter
                        {
                            SelectCommand = cmd
                        };
                        var rer = cmd.ExecuteScalar().ToString();


                        if (padre.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
                        {

                            ///Actualizamos todas las transacciones que se hayan hecho con esa cuenta
                            ///pero solo en caso de la cuenta ser un auxiliar 
                            ///si la cuenta no es un auxiliar no tiene sentido 
                            ///las cuentas de mayor pueden heredar cuentas cuentas quieras 
                            ///sin tener que pasar el saldo 
                            cmd.CommandText = "UPDATE transactions_accounting SET account_id = @account_id WHERE account_id = @account_id_old;" +
                                              "UPDATE accounts SET account_guide = 2 WHERE account_id = @account_id_old LIMIT 1";
                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@account_id", rer);
                            cmd.Parameters.AddWithValue("@account_id_old", t.Padre);

                            padre.Indicador = IndicadorCuenta.Cuenta_De_Mayor;
                            cmd.ExecuteNonQuery();
                        }

                        t.Id = Convert.ToInt32(rer);
                        tr.Commit();
                        mensaje = "Asiento guardado correctamente";
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    mensaje = ex.Message;
                    return false;
                }
            }

        }

        public bool GenerarSaldosEnCeroParaCierreDeAsieto(Cuenta cuentaSaldoAsiento, Compañia compañia, Usuario usuario, int limitSecy)
        {
            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {

                try
                {
                    var sql1 = "UPDATE accounts SET previous_balance_c = 0 , previous_balance_d = 0, updated_by = @updated_by WHERE company_id = @company_id LIMIT @limit_sec";
                    using (MySqlCommand cmd = new MySqlCommand(sql1, tr.Connection, tr))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter
                        {
                            SelectCommand = cmd
                        };
                        cmd.Parameters.AddWithValue("@updated_by", usuario.UsuarioId);
                        cmd.Parameters.AddWithValue("@company_id", compañia.Codigo);
                        cmd.Parameters.AddWithValue("@limit_sec", limitSecy);
                        cmd.ExecuteNonQuery();

                    }


                    ///Insertamos la cuenta en  la base de datos 
                    ///este query nos devuelve el id de la nueva cuenta 
                    var sql2 = "UPDATE accounts SET previous_balance_c = @previous_balance_c , previous_balance_d = @previous_balance_c, updated_by = @updated_by WHERE account_id = @account_id AND company_id = @company_id LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(sql2, tr.Connection, tr))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@previous_balance_c", cuentaSaldoAsiento.SaldoActualColones);
                        cmd.Parameters.AddWithValue("@previous_balance_d", cuentaSaldoAsiento.SaldoActualDolares);
                        cmd.Parameters.AddWithValue("@updated_by", usuario.UsuarioId);
                        cmd.Parameters.AddWithValue("@account_id", cuentaSaldoAsiento.Id);
                        cmd.Parameters.AddWithValue("@company_id", compañia.Codigo);
                        MySqlDataAdapter da = new MySqlDataAdapter
                        {
                            SelectCommand = cmd
                        };

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new Exception("Ocurrio un error al guardar el saldo del asiento de cierre");
                        }
                        tr.Commit();
                        return true;

                    }
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    throw ex;
                }
            }
        }

        public Boolean UpdateNameInfo(Cuenta t, Usuario user, out String mensaje)
        {
            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroCuenta, CRUDName.Actualizar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            try
            {
                manejador.Ejecutar("INSERT IGNORE INTO accounts_names(name) VALUES(@name);",
                                 new List<Parametro> { new Parametro("@name", t.Nombre) }, CommandType.Text);

                var sql = "UPDATE accounts " +
                          "SET account_name_id = (SELECT account_name_id FROM accounts_names where name = @name LIMIT 1), " +
                          "detail = @detail," +
                          "updated_by = @updated_by " +
                          "WHERE account_id = @account_id and company_id = @company_id;";

                var retorno = manejador.Ejecutar(sql, new List<Parametro> {
                                                     new Parametro("@name",t.Nombre),
                                                     new Parametro("@detail",t.Detalle),
                                                     new Parametro("@updated_by",user.UsuarioId),
                                                     new Parametro("@account_id",t.Id),
                                                     new Parametro("@company_id",t.MyCompania.Codigo)},
                                                     CommandType.Text);
                if (retorno > 0)
                {
                    mensaje = "Cuenta actualizada correctamente";
                    return true;

                }
                else
                {
                    mensaje = "No se pudo actualizar la cuenta";
                    return false;
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;

            }


        }
        public Boolean UpdatesettInfo(Cuenta t, Usuario user, out String mensaje)
        {
            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroCuenta, CRUDName.Actualizar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            try
            {
                manejador.Ejecutar("INSERT IGNORE INTO accounts_names(name) VALUES(@name);",
                                 new List<Parametro> { new Parametro("@name", t.Nombre) }, CommandType.Text);

                var sql = "UPDATE accounts " +
                          "SET account_guide = @account_guide , " +
                          "updated_by = @updated_by " +
                          "WHERE account_id = @account_id and company_id = @company_id;";

                var retorno = manejador.Ejecutar(sql, new List<Parametro> {
                                                     new Parametro("@account_guide",(int)t.Indicador),
                                                     new Parametro("@updated_by",user.UsuarioId),
                                                     new Parametro("@account_id",t.Id),
                                                     new Parametro("@company_id",t.MyCompania.Codigo)},
                                                     CommandType.Text);
                if (retorno > 0)
                {
                    mensaje = "Cuenta actualizada correctamente";
                    return true;

                }
                else
                {
                    mensaje = "No se pudo actualizar la cuenta";
                    return false;
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;

            }


        }
        public void CuentaConSaldos(List<Cuenta> cuentas, Compañia compañia, DateTime dateTime1, DateTime dateTime2)
        {

            var sql = "SELECT account_id, SUM(debito),SUM(credito),SUM(debito_USD), SUM(credito_USD), SUM(cuadrado)  " +
                      "FROM account_info WHERE company_id = @company_id " +
                      "AND month_report between @date1 and @date2 " +
                      "GROUP BY account_id";

            var parametros = new List<Parametro>() {
                        new Parametro("@company_id", compañia.Codigo),
                        new Parametro("@date1", $"{dateTime1.Year}{String.Format("{0, 0:D2}", dateTime1.Month)}"),
                        new Parametro("@date2", $"{dateTime2.Year}{String.Format("{0, 0:D2}", dateTime2.Month)}")
            };
            var dt = manejador.Listado(sql, parametros, CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                Object[] vs = item.ItemArray;

                cuentas.ForEach((Cuenta) =>
                {
                    if (Cuenta.Id == Convert.ToDouble(vs[0]))
                    {
                        Cuenta.DebitosColones = Convert.ToDecimal(vs[1]);
                        Cuenta.CreditosColones = Convert.ToDecimal(vs[2]);
                        Cuenta.DebitosDolares = Convert.ToDecimal(vs[3]);
                        Cuenta.CreditosDolares = Convert.ToDecimal(vs[4]);
                        Cuenta.Cuadrada = (Convert.ToInt32(vs[5]) == 0) ? false : true;
                        return;
                    }
                });

            }
        }
        public Boolean VerificarNombre(Cuenta cuenta, String nuevoNombre, Compañia compañia)
        {
            try
            {

                var sql = "SELECT COUNT(*) FROM accounts AS ac JOIN accounts_names AS an ON ac.account_name_id = an.account_name_id " +
                          "WHERE ac.account_id <> @account_id AND company_id = @company_id AND ac.account_type = 1 AND an.name = @name";
                using (MySqlCommand cmd = new MySqlCommand(sql, manejador.GetConnection()))
                {

                    cmd.Parameters.AddWithValue("@account_id", cuenta.Id);
                    cmd.Parameters.AddWithValue("@company_id", compañia.Codigo);
                    cmd.Parameters.AddWithValue("@name", nuevoNombre);
                    var cot = Convert.ToInt32(cmd.ExecuteScalar());

                    if (cot == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public Boolean CopiarCuentas(Compañia compañiaModelo, Compañia compañiaNueva, Usuario usuario)
        {

            ///Empezamos insertando de arriba a abajo 
            ///entonces  primero  hay que buscar las titulo
            ///insertamos la lista de titulo 
            ///Buscamos la lista de sus hijas y le asignamos el padre id con el nuevo valor 
            ///insertamos cuenta por cuenta y hacemos el mismo proceso 

            var cuenntas = GetAll(compañiaModelo, usuario);

            foreach (var cuenta in cuenntas)
            {
                ///Si la cuenta es titulo es por ahi de donde debemos empezar
                if (cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    var cuentaPadre = cuenta.DeepCopy();
                    cuentaPadre.MyCompania = compañiaNueva;
                    cuentaPadre.SaldoAnteriorColones = 0;
                    cuentaPadre.SaldoAnteriorDolares = 0;
                    cuentaPadre.DebitosColones = 0;
                    cuentaPadre.CreditosColones = 0;
                    cuentaPadre.DebitosDolares = 0;
                    cuentaPadre.CreditosDolares = 0;

                    ///Insertar la cuenta
                    if (!InserSimple(ref cuentaPadre, usuario))
                    {
                        return false;
                    }

                    //Insert(ref cuentaPadre, cuentaPadre, usuario, out String mensaje);

                    ///Buscamos todas las cuentas hijas 
                    var rs = GetHijas(cuenntas, cuenta);
                    ///Asigamos el nuevo id de padre a la cuentas hijas
                    rs.ForEach((x) =>
                    {
                        x.Padre = cuentaPadre.Id;
                    });

                    ///Asiganamos el nuevo id a la cuenta
                    cuenta.Id = cuentaPadre.Id;
                    ///Llamamos al metodo interno para insertar las cuentas
                    if (!InsertT())
                    {
                        return false;
                    }

                    bool InsertT()
                    {

                        ///Mientras la lista sea nula 
                        while (rs != null && rs.Count != 0)
                        {
                            foreach (var item in rs)
                            {
                                var cuentaPadre2 = item.DeepCopy();
                                cuentaPadre2.MyCompania = compañiaNueva;
                                cuentaPadre2.SaldoAnteriorColones = 0;
                                cuentaPadre2.SaldoAnteriorDolares = 0;
                                cuentaPadre2.DebitosColones = 0;
                                cuentaPadre2.CreditosColones = 0;
                                cuentaPadre2.DebitosDolares = 0;
                                cuentaPadre2.CreditosDolares = 0;

                                if (!InserSimple(ref cuentaPadre2, usuario))
                                {
                                    return false;
                                }
                                //Insert(ref cuentaPadre2, cuentaPadre, usuario, out String mensajew);

                                rs = GetHijas(cuenntas, item);
                                rs.ForEach((x) =>
                                {
                                    x.Padre = cuentaPadre2.Id;
                                });
                                item.Id = cuentaPadre2.Id;
                                ///Volvemos a llamar al metodo interno 
                                InsertT();
                            }

                        }
                        return true;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Buscamos todas las cuentas padre
        /// hasta llegar a las titulo y ahi empezamos a insertar y corregir 
        /// los id
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="padre"></param>
        /// <returns></returns>
        private List<Cuenta> GetHijas(List<Cuenta> lst, Cuenta padre)
        {
            var retorno = lst.FindAll(x => x.Padre == padre.Id);

            return retorno ?? new List<Cuenta>();
        }
        private bool InserSimple(ref Cuenta cuenta, Usuario usuario)
        {
            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var sql2 = "INSERT INTO accounts (account_name_id,father_account,company_id,account_type,account_guide,detail,updated_by) " +
                               "VALUES((SELECT account_name_id FROM accounts_names WHERE name = @name LIMIT 1)," +
                               "@father_account," +
                               "@company_id," +
                               "@account_type," +
                               "@account_guide," +
                               "@detail," +
                               "@updated_by);" +
                               "SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(sql2, tr.Connection, tr))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@name", cuenta.Nombre);
                        if (cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                        {
                            cmd.Parameters.AddWithValue("@father_account", null);

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@father_account", cuenta.Padre);
                        }
                        cmd.Parameters.AddWithValue("@company_id", cuenta.MyCompania.Codigo);
                        cmd.Parameters.AddWithValue("@account_type", cuenta.TipoCuenta.TipoCuenta.ToString().Replace('_', ' ').ToUpper());
                        cmd.Parameters.AddWithValue("@account_guide", (int)cuenta.Indicador);
                        cmd.Parameters.AddWithValue("@detail", cuenta.Detalle);
                        cmd.Parameters.AddWithValue("@updated_by", usuario.UsuarioId);

                        MySqlDataAdapter da = new MySqlDataAdapter
                        {
                            SelectCommand = cmd
                        };
                        cuenta.Id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                        tr.Commit();
                        return true;
                    }
                }
                catch (Exception)
                {
                    tr.Rollback();
                    return false;
                }
            }
        }
    }
}
