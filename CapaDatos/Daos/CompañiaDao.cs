using CapaDatos.Conexion;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
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
    public class CompañiaDao 
    {
        private Manejador manejador = new Manejador(); 

        /// <summary>
        /// Devuelve la lista con todas las compañias
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Compañia> GetAll(Usuario user)
        {
            var retorno = new List<Compañia>();

            var sqlFisicas = "";
            var sqlJuridicas = "";

            if (user.TipoUsuario == TipoUsuario.Usuario)
            {
                sqlJuridicas = "SELECT " +
               "T0.company_id," +
               "T0.type_id+0," +
               "T0.number_id," +
               "T0.name," +
               "T0.money_type+0," +
               "T0.op1," +
               "T0.op2," +
               "T0.address," +
               "T0.website," +
               "T0.mail," +
               "T0.phone_number1," +
               "T0.phone_number2," +
               "T0.notes," +
               "T0.user_id," +
               "T0.created_at," +
               "T0.updated_at," +
               "T0.active " +
               "FROM companies AS T0 JOIN companies_permission AS T1 ON T0.company_id = T1.company_id " +
               "WHERE T0.type_id = 1 AND T1.user_id = @user_id AND T1.active = 1 AND T1.deleted = 0";

                sqlFisicas = "SELECT " +
               "T0.company_id," +
               "T0.type_id+0," +
               "T0.number_id," +
               "T0.name," +
               "T0.money_type+0," +
               "T0.op1," +
               "T0.op2," +
               "T0.address," +
               "T0.website," +
               "T0.mail," +
               "T0.phone_number1," +
               "T0.phone_number2," +
               "T0.notes," +
               "T0.user_id," +
               "T0.created_at," +
               "T0.updated_at," +
               "T0.active " +
               "FROM companies AS T0  JOIN companies_permission AS T1 ON T0.company_id = T1.company_id " +
               "WHERE T0.type_id <> 1 AND T1.user_id = @user_id AND T1.active = 1 AND T1.deleted = 0";
            }
            else
            {
                sqlJuridicas = "SELECT " +
               "T0.company_id," +
               "T0.type_id+0," +
               "T0.number_id," +
               "T0.name," +
               "T0.money_type+0," +
               "T0.op1," +
               "T0.op2," +
               "T0.address," +
               "T0.website," +
               "T0.mail," +
               "T0.phone_number1," +
               "T0.phone_number2," +
               "T0.notes," +
               "T0.user_id," +
               "T0.created_at," +
               "T0.updated_at," +
               "T0.active " +
               "FROM companies AS T0 " +
               "WHERE T0.type_id = 1";
                sqlFisicas = "SELECT " +
               "T0.company_id," +
               "T0.type_id+0," +
               "T0.number_id," +
               "T0.name," +
               "T0.money_type+0," +
               "T0.op1," +
               "T0.op2," +
               "T0.address," +
               "T0.website," +
               "T0.mail," +
               "T0.phone_number1," +
               "T0.phone_number2," +
               "T0.notes," +
               "T0.user_id," +
               "T0.created_at," +
               "T0.updated_at," +
               "T0.active " +
               "FROM companies AS T0 " +
               "WHERE T0.type_id <> 1";
            }
            retorno.AddRange(this.PersonasFisicas(user, sqlFisicas));
            retorno.AddRange(this.PersonasJuridicas(user, sqlJuridicas));

            return retorno;
        }
        public DataTable GetDataTable(Compañia t, Usuario user)
        {
            return manejador.Listado("SELECT * FROM companies", CommandType.Text);
        }
        public Boolean Insert(Compañia compania, Usuario user, Compañia copiarMAestroCuenta, out String mensaje)
        {

            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroCompanias, CRUDName.Insertar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }

            var sql = "INSERT INTO companies(company_id,type_id,number_id,name,money_type,op1,op2,address,website,mail,phone_number1,phone_number2,notes,user_id)" +
                           "VALUES(@company_id,@type_id,@number_id,@name,@money_type,@op1,@op2,@address,@website,@mail,@phone_number1,@phone_number2,@notes,@user_id);";
            try
            {
                List<Parametro> lst = new List<Parametro>();
                compania.Codigo = NuevoCodigo();

                lst.Add(new Parametro("@company_id", compania.Codigo));
                lst.Add(new Parametro("@type_id", (int)compania.TipoId));
                lst.Add(new Parametro("@number_id", compania.NumeroCedula));
                lst.Add(new Parametro("@name", compania.Nombre));
                lst.Add(new Parametro("@money_type", (int)compania.TipoMoneda));
                lst.Add(new Parametro("@address", compania.Direccion));
                lst.Add(new Parametro("@website", compania.Web));
                lst.Add(new Parametro("@mail", compania.Correo));
                lst.Add(new Parametro("@phone_number1", compania.Telefono[0]));
                lst.Add(new Parametro("@phone_number2", compania.Telefono[1]));
                lst.Add(new Parametro("@notes", compania.Observaciones));
                lst.Add(new Parametro("@user_id", user.UsuarioId));
                lst.Add(new Parametro("@op1", ((compania is PersonaJuridica) ? ((PersonaJuridica)compania).MyIDRepresentanteLegal : ((PersonaFisica)compania).MyApellidoPaterno)));
                lst.Add(new Parametro("@op2", ((compania is PersonaJuridica) ? ((PersonaJuridica)compania).MyRepresentanteLegal : ((PersonaFisica)compania).MyApellidoMaterno)));

                if (manejador.Ejecutar(sql, lst, CommandType.Text) != 0)
                {
                    if (copiarMAestroCuenta.Codigo == "POR DEFECTO")
                    {
                        GenerarCuentasDefault(compania, user);
                    }
                    else
                    {
                        if (!CopiarMaestroDeCuenta(copiarMAestroCuenta, user, compania))
                        {
                            mensaje = "No se pudo clonar el maestro de cuentas";
                            return false;
                        }

                    }
                    mensaje = "Se registro la compañia correctamente";
                    return true;
                }
                else
                {
                    mensaje = "No se pudo ingresar la compañia";
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool CopiarMaestroDeCuenta(Compañia copiarMAestroCuenta, Usuario usuario, Compañia compañia)
        {
            return new CuentaDao().CopiarCuentas(copiarMAestroCuenta, compañia, usuario);
        }
        public DataTable GetDataTable(TipoID tipoID, Boolean todos = false)
        {
            var sql = "";

            if (todos)
            {
                sql = " SET lc_time_names = 'es_MX'; SELECT " +
                   "T0.company_id AS 'Código'," +
                   "T0.type_id AS 'Tipo ID'," +
                   "T0.number_id AS 'Número Identicación'," +
                   "T0.name AS 'Nombre'," +
                   "T0.op1 AS 'Apellido Paterno'," +
                   "T0.op2 AS 'Apellido Materno'," +
                   "T0.address AS 'Dirección'," +
                   "T0.website AS 'Sitio Web'," +
                   "T0.mail AS 'Correro Electronico'," +
                   "T0.phone_number1 AS 'Telefono'," +
                   "T0.phone_number2 AS 'Telefono'," +
                   "T0.notes AS 'Observaciones'," +
                   "(SELECT CONCAT(u.name, ' ', u.lastname_p, ' ', u.lastname_m) FROM users AS u WHERE u.user_id = T0.user_id LIMIT 1) AS 'Usuario'," +
                   "IF(T0.active <> 1, 'No', 'Si') AS 'Acivo' " +
                   "FROM companies AS T0 WHERE T0.type_id <> 1";
                return manejador.Listado(sql, CommandType.Text);
            }

            if (tipoID == TipoID.CEDULA_JURIDICA)
            {
                sql = "SET lc_time_names = 'es_MX'; SELECT " +
                           "company_id AS 'Código'," +
                           "type_id AS 'Tipo ID'," +
                           "number_id AS 'Número Identicación'," +
                           "name AS 'Nombre'," +
                           "op1 AS 'Representante Legal'," +
                           "op2 AS 'ID Representante'," +
                           "address AS 'Dirección'," +
                           "website AS 'Sitio Web'," +
                           "mail AS 'Correro Electronico'," +
                           "phone_number1 AS 'Telefono'," +
                           "phone_number2 AS 'Telefono'," +
                           "notes AS 'Observaciones'," +
                           "(SELECT CONCAT(name, ' ', lastname_p, ' ', lastname_m) FROM users AS u WHERE u.user_id = user_id LIMIT 1) AS 'Usuario'," +
                           "IF(active <> 1, 'No', 'Si') AS 'Acivo' " +
                           "FROM companies WHERE type_id = 1";
                return manejador.Listado(sql, CommandType.Text);
            }
            else
            {
                sql = "SET lc_time_names = 'es_MX'; SELECT " +
                     "company_id AS 'Código'," +
                     "type_id AS 'Tipo ID'," +
                     "number_id AS 'Número Identicación'," +
                     "name AS 'Nombre'," +
                     "op1 AS 'Apellido Paterno'," +
                     "op2 AS 'Apellido Materno'," +
                     "address AS 'Dirección'," +
                     "website AS 'Sitio Web'," +
                     "mail AS 'Correro Electronico'," +
                     "phone_number1 AS 'Telefono'," +
                     "phone_number2 AS 'Telefono'," +
                     "notes AS 'Observaciones'," +
                     "(SELECT CONCAT(name, ' ', lastname_p, ' ', lastname_m) FROM users AS u WHERE u.user_id = user_id LIMIT 1) AS 'Usuario'," +
                     "IF(active <> 1, 'No', 'Si') AS 'Acivo' " +
                     "FROM companies WHERE type_id = @type_id";
                return manejador.Listado(sql, new Parametro("@type_id", Convert.ToInt16(tipoID)), CommandType.Text);
            }


        }
        public Boolean Update(Compañia compania, Usuario user, out String mensaje)
        {

            if (!Guachi.Consultar(user, VentanaInfo.FormMaestroCompanias, CRUDName.Actualizar))
            {
                mensaje = "Acceso denegado!!!";
                return false;
            }


            var message = compania.Nombre + " ACTUAIZADA CORRECTAMENTE";
            var sql = "UPDATE companies SET " +
                      "name=@name,money_type=@money_type,op1=@op1,op2=@op2,address=@address,website=@website,mail=@mail,phone_number1=@phone_number1," +
                      "phone_number2=@phone_number2,notes=@notes,user_id=@user_id,active=@active WHERE company_id=@company_id";
            try
            {

                List<Parametro> lst = new List<Parametro>();

                lst.Add(new Parametro("@company_id", compania.Codigo));
                lst.Add(new Parametro("@name", compania.Nombre));
                lst.Add(new Parametro("@money_type", compania.TipoMoneda));
                lst.Add(new Parametro("address", compania.Direccion));
                lst.Add(new Parametro("@website", compania.Web));
                lst.Add(new Parametro("@mail", compania.Correo));
                lst.Add(new Parametro("@phone_number1", compania.Telefono[0]));
                lst.Add(new Parametro("@phone_number2", compania.Telefono[1]));
                lst.Add(new Parametro("@notes", compania.Observaciones));
                lst.Add(new Parametro("@user_id", user.UsuarioId));
                lst.Add(new Parametro("@active", compania.Activo));
                lst.Add(new Parametro("@op1", ((compania is PersonaJuridica) ? ((PersonaJuridica)compania).MyRepresentanteLegal : ((PersonaFisica)compania).MyApellidoPaterno)));
                lst.Add(new Parametro("@op2", ((compania is PersonaJuridica) ? ((PersonaJuridica)compania).MyIDRepresentanteLegal : ((PersonaFisica)compania).MyApellidoMaterno)));

                if (manejador.Ejecutar(sql, lst, CommandType.Text) == 0)
                {
                    mensaje = "No se pudo actualizar la compañia";
                    return false;
                }
                else
                {
                    mensaje = "Se actualizo la compañia";
                    return true;
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }
        public Compañia GetCompañia(String id, TipoID tipoID)
        {
            if (tipoID == TipoID.CEDULA_NACIONAL)
            {
                //tratar de hacerlo mas eficiente
                var comdando = "SELECT * FROM maestro_compania m, personas p WHERE M.codigo = P.IDCompania AND p.cedula = @p1 ";


                using (MySqlCommand cmd = new MySqlCommand(comdando, manejador.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@p1", id);
                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow item in dt.Rows)
                    {
                        Object[] vn = item.ItemArray;

                        var c = new PersonaFisica();
                        c.Codigo = Convert.ToString(vn[0]);
                        c.TipoId = (TipoID)Convert.ToInt32(vn[1]);
                        c.Direccion = Convert.ToString(vn[2]);
                        c.Web = Convert.ToString(vn[3]);
                        c.Correo = Convert.ToString(vn[4]);
                        c.Observaciones = Convert.ToString(vn[6]);
                        c.NumeroCedula = Convert.ToString(vn[10]);
                        c.Nombre = Convert.ToString(vn[11]);
                        c.MyApellidoPaterno = Convert.ToString(vn[12]);
                        c.MyApellidoMaterno = Convert.ToString(vn[13]);
                        return c;
                    }



                }

            }
            else
            {
                //DataTable dt = Listado("sp_buscar_fisicasID", new ClsParametros("@id", id), CommandType.Text);
                //foreach (DataRow item in dt.Rows)
                //{
                //    return SetinComp.CrearPersonaFisica(item.ItemArray);
                //}
            }

            return null;
        }

        #region funciones de soporte
        public string NuevoCodigo()
        {
            String sql = "SELECT c.company_id FROM companies c ORDER BY c.company_id DESC LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(sql, manejador.GetConnection()))
            {
                MySqlDataAdapter da = new MySqlDataAdapter
                {
                    SelectCommand = cmd
                };
                var cfd = "";
                if (cmd.ExecuteScalar() != null)
                {
                    cfd = cmd.ExecuteScalar().ToString();
                }
                else
                {
                    cfd = "C000";
                }
                //creo que falta cerrar la conexión
                return "C" + (
                              (
                               (int.Parse
                                (cfd
                                 .Substring(1, 3)
                                )
                               ) + 1
                              )
                               .ToString("000")
                            );
            }
        }
        private List<PersonaJuridica> PersonasJuridicas(Usuario usuario, String sql)
        {
            List<PersonaJuridica> retorno = new List<PersonaJuridica>();
            DataTable dt = manejador.Listado(sql,
               new List<Parametro> { new Parametro("@user_id", usuario.UsuarioId) },
               CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                object[] vs = item.ItemArray;
                retorno.Add(new PersonaJuridica(
                    codigo: Convert.ToString(vs[0]),
                    tipoID: ((TipoID)Convert.ToInt32(vs[1])),
                    numeroId: Convert.ToString(vs[2]),
                    nombre: Convert.ToString(vs[3]),
                    TipoMoneda: (TipoMonedaCompañia)Convert.ToInt16(vs[4]),
                    representanteLegal: Convert.ToString(vs[5]),
                    IDRepresentante: Convert.ToString(vs[6]),
                    direccion: Convert.ToString(vs[7]),
                    web: Convert.ToString(vs[8]),
                    correo: Convert.ToString(vs[9]),
                    telefono: new string[] { Convert.ToString(vs[10]), Convert.ToString(vs[11]) },
                    observaciones: Convert.ToString(vs[12]),
                    activo: Convert.ToBoolean(vs[16])
                    ));

            }
            return retorno;
        }
        private List<PersonaFisica> PersonasFisicas(Usuario usuario, String sql)
        {

            List<PersonaFisica> retorno = new List<PersonaFisica>();
            DataTable dt = manejador.Listado(sql,
               new List<Parametro> { new Parametro("@user_id", usuario.UsuarioId) },
               CommandType.Text);

            foreach (DataRow item in dt.Rows)
            {
                object[] vs = item.ItemArray;
                retorno.Add(new PersonaFisica(
                          codigo: Convert.ToString(vs[0]),
                          tipoID: ((TipoID)Convert.ToInt32(vs[1])),
                          numeroId: Convert.ToString(vs[2]),
                          nombre: Convert.ToString(vs[3]),
                          TipoMoneda: (TipoMonedaCompañia)Convert.ToInt16(vs[4]),
                          apellidoPaterno: Convert.ToString(vs[5]),
                          apellidoMaterno: Convert.ToString(vs[6]),
                          direccion: Convert.ToString(vs[7]),
                          web: Convert.ToString(vs[8]),
                          correo: Convert.ToString(vs[9]),
                          telefono: new string[] { Convert.ToString(vs[10]), Convert.ToString(vs[11]) },
                          observaciones: Convert.ToString(vs[12]),
                          activo: Convert.ToBoolean(vs[16])
                                              ));

            }

            return retorno;
        }
        #endregion
        //1 tipo cuenta 2 guia ,3 cuenta padre
        public void GenerarCuentasDefault(Compañia c, Usuario user)
        {
            using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {

                var sql = "INSERT INTO accounts (account_name_id,father_account,company_id,account_type,account_guide,editable,updated_by) VALUES" +
                                               "(@account_name_id,@father_account,@company_id,@account_type,@account_guide,0,@updated_by);SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(sql, manejador.GetConnection(), tr))
                {

                    var gestor = new decimal[60];
                    for (int j = 1; j <= 57; j++)
                    {

                        MySqlDataAdapter da = new MySqlDataAdapter { SelectCommand = cmd };

                        var r = EstablecerHijos(gestor, j);
                        if (r.Item3 == 0.0m)
                        {
                            cmd.Parameters.AddWithValue("@father_account", null);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@father_account", r.Item3);
                        }

                        cmd.Parameters.AddWithValue("@account_name_id", r.Item4);
                        cmd.Parameters.AddWithValue("@company_id", c.Codigo);
                        cmd.Parameters.AddWithValue("@account_type", r.Item1);
                        cmd.Parameters.AddWithValue("@account_guide", r.Item2);
                        cmd.Parameters.AddWithValue("@updated_by", 1);
                        gestor[j] = Convert.ToDecimal(cmd.ExecuteScalar());
                        cmd.Parameters.Clear();

                    }
                    tr.Commit();
                }
            }
        }
        private Tuple<String, String, decimal, int> EstablecerHijos(decimal[] gestor, int j)
        {
            String[] account_type = new string[] { "ACTIVO", "PASIVO", "PATRIMONIO", "INGRESO", "COSTO VENTA", "EGRESO" };
            String[] account_guide = new string[] { "TITULO", "CUENTA DE MAYOR", "CUENTA AUXILIAR" };

            if (j <= 6)
            {
                return Tuple.Create(account_type[j - 1], account_guide[0], gestor[0], j);
            }
            if (j >= 7 && j <= 8)//activo
            {
                return Tuple.Create(account_type[0], account_guide[1], gestor[1], j);
            }
            else if (j >= 9 && j <= 10)//pasivo
            {
                return Tuple.Create(account_type[1], account_guide[1], gestor[2], j);
            }
            else if (j >= 11 && j <= 15)//patrimonio
            {
                return Tuple.Create(account_type[2], account_guide[1], gestor[3], j);
            }
            //else if (j==16)//ingreso
            //{
            //    return Tuple.Create(account_type[3], account_guide[1], gestor[4]);
            //}
            else if (j >= 16 && j <= 18)//egreso
            {
                return Tuple.Create(account_type[5], account_guide[1], gestor[6], j);
            }
            else if (j >= 19 && j <= 24)//ACTIVO CORREINTE
            {
                return Tuple.Create(account_type[0], account_guide[2], gestor[7], j);
            }
            else if (j >= 25 && j <= 29)//ACTIVO NO CORREINTE
            {
                return Tuple.Create(account_type[0], account_guide[2], gestor[8], j);
            }
            else if (j >= 30 && j <= 51)//gastos administrativos
            {
                return Tuple.Create(account_type[5], account_guide[2], gestor[16], j);
            }
            else if (j >= 52 && j <= 54)//gastos financieros
            {
                return Tuple.Create(account_type[5], account_guide[2], gestor[17], j);
            }
            else if (j == 55)//INGRESO
            {
                return Tuple.Create(account_type[3], account_guide[1], gestor[4], 4);
            }
            else if (j == 56)//COSTO VENTA
            {
                return Tuple.Create(account_type[4], account_guide[1], gestor[5], 5);
            }
            else if (j == 57)//EGRESO
            {
                return Tuple.Create(account_type[5], account_guide[1], gestor[6], 6);
            }
            else
            {
                return Tuple.Create("", "", 0.0m, 0);
            }
        }
    }

}

