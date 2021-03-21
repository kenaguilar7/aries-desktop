using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using System.Configuration;

namespace CapaDatos.Conexion
{
    public class Manejador
    {
        private MySqlConnection databaseConnection =
            new MySqlConnection(ConfigurationManager.ConnectionStrings["DBconnectionString"].ConnectionString);

        public void OpenConnection()
        {
            if (databaseConnection.State == ConnectionState.Closed)
            {
                databaseConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (databaseConnection.State == ConnectionState.Open)
            {
                databaseConnection.Close();
            }
        }
        public MySqlConnection GetConnection()
        {
            OpenConnection();
            return databaseConnection;
        }
        public Manejador() { }
        /// <summary>
        /// metodo para ejecutar sp(insert, delete, ,update)
        /// </summary>
        /// <param name="nombreSp"></param>
        /// <param name="lst"></param>
        public int Ejecutar(String nombreSp, List<Parametro> lst, CommandType type)
        {
            var retorno = 0;

            using (MySqlTransaction tr = GetConnection().BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(nombreSp, databaseConnection, tr))
                    {
                        cmd.CommandType = type;

                        foreach (var item in lst)
                        {
                            if (item.myDireccion == ParameterDirection.Input)
                            {
                                cmd.Parameters.AddWithValue(item.myNombre, item.myValor);
                            }
                            if (item.myDireccion == ParameterDirection.Output)
                            {
                                cmd.Parameters.Add(item.myNombre, item.myTipoDato, item.myTamanyo).Direction = ParameterDirection.Output;
                            }
                        }

                        retorno = cmd.ExecuteNonQuery();
                        tr.Commit();

                        for (int i = 0; i < lst.Count; i++)
                        {
                            if (cmd.Parameters[i].Direction == ParameterDirection.Output)
                            {
                                lst[i].myValor = cmd.Parameters[i].Value.ToString();
                            }
                        }
                    }
                }


                catch (Exception ex)
                {
                    tr.Rollback();
                    throw ex;

                }
                finally
                {
                    CloseConnection();
                }
            }
            return retorno;
        }
        /// <summary>
        /// metodo para Listado o consultas (select)
        /// </summary>
        /// <param name="nombreSp"></param>
        /// <param name="lst"></param>
        /// <returns>la consulta</returns>
        #region LISTADOS
        public DataTable Listado(String nombreSp, List<Parametro> lst, CommandType type)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter da;
            try
            {
                da = new MySqlDataAdapter(nombreSp, GetConnection());
                da.SelectCommand.CommandType = type;
                if (lst != null)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        da.SelectCommand.Parameters.AddWithValue(lst[i].myNombre, lst[i].myValor);

                    }
                }

                da.Fill(dt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        /// <summary>
        /// Usar para obtener listas que no requiera parametros 
        /// </summary>
        /// <param name="nombreSp"></param>
        /// <returns></returns>
        public DataTable Listado(String nombreSp, CommandType type)
        {
            return Listado(nombreSp, new List<Parametro>(), type);
        }
        /// <summary>
        /// Usar para obtener lista que requiere solo de un parametro
        /// </summary>
        /// <param name="nombreSp"></param>
        /// <param name="parametro"></param>
        /// <returns></returns>
        public DataTable Listado(String nombreSp, Parametro parametro, CommandType type)
        {
            var par = new List<Parametro>();
            par.Add(parametro);
            return Listado(nombreSp, par, type);
        }
        #endregion endlistadosregion
    }

}
