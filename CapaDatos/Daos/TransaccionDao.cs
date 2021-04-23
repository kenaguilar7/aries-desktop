using CapaDatos.Conexion;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Daos
{

    /// <summary>
    /// Esta clase requiere de revision
    /// </summary>
    public class TransaccionDao

    {
        Manejador manejador = new Manejador();
        ///// <summary>
        ///// Inserta en la tabla la transaccion y devuelve la misma con su id asiganada en la base de datos
        ///// </summary>
        ///// <param name="journalEntryLine"></param>
        ///// <param name="idAsiento"></param>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public JournalEntryLine Insert(JournalEntryLine journalEntryLine, decimal idAsiento, Usuario user)
        //{
        //    using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
        //    {
        //        try
        //        {
        //            var sql = "INSERT INTO transactions_accounting " +
        //                      "(account_id,accounting_entry_id,reference,detail,balance, balance_type,money_type, money_chance,bill_date, updated_by) " +
        //                      "VALUES (@account_id,@accounting_entry_id,@reference,@detail,@balance,@balance_type,@money_type,@money_chance,@bill_date,@updated_by); SELECT LAST_INSERT_ID();";
        //            using (MySqlCommand cmd = new MySqlCommand(sql, tr.Connection, tr))
        //            {
        //                List<Parametro> lst = new List<Parametro>();
        //                cmd.Parameters.AddWithValue("@account_id", journalEntryLine.AccountId.Id);
        //                cmd.Parameters.AddWithValue("@accounting_entry_id", idAsiento);
        //                cmd.Parameters.AddWithValue("@reference", journalEntryLine.Reference);
        //                cmd.Parameters.AddWithValue("@detail", journalEntryLine.Memo);
        //                cmd.Parameters.AddWithValue("@balance", journalEntryLine.Monto);
        //                cmd.Parameters.AddWithValue("@balance_type", Convert.ToInt16(journalEntryLine.DebOrCred));
        //                cmd.Parameters.AddWithValue("@money_type", Convert.ToInt16(journalEntryLine.Currency));
        //                cmd.Parameters.AddWithValue("@money_chance", journalEntryLine.RateAmount);
        //                cmd.Parameters.AddWithValue("@bill_date", journalEntryLine.Date);
        //                cmd.Parameters.AddWithValue("@updated_by", user.UsuarioId);

        //                var id = cmd.ExecuteScalar();
        //                journalEntryLine.Id = Convert.ToInt32(id);
        //            }
        //            tr.Commit();
        //            return journalEntryLine;
        //        }
        //        catch (Exception)
        //        {
        //            tr.Rollback();
        //            throw;
        //        }
        //    }
        //}
        //public bool Update(JournalEntryLine tr, Usuario usuario, out String mensaje)
        //{
        //    if (!Guachi.Consultar(usuario, VentanaInfo.FormAsientos, CRUDName.Actualizar))
        //    {
        //        mensaje = "Acceso denegado!!!";
        //        return false;
        //    }
        //    try
        //    {
        //        var sql = "UPDATE transactions_accounting " +
        //                  "SET account_id = @account_id, " +
        //                  "reference = @reference, " +
        //                  "detail = @detail, " +
        //                  "balance = @balance," +
        //                  "balance_type = @balance_type, " +
        //                  "money_type = @money_type, " +
        //                  "money_chance = @money_chance, " +
        //                  "bill_date = @bill_date," +
        //                  "updated_by = @updated_by " +
        //                  "WHERE transaction_accounting_id = @transaction_accounting_id";

        //        List<Parametro> lst = new List<Parametro>
        //        {
        //            new Parametro("@account_id", tr.AccountId.Id),
        //            new Parametro("@reference", tr.Reference),
        //            new Parametro("@detail", tr.Memo),
        //            new Parametro("@balance", tr.Monto),
        //            new Parametro("@balance_type", (int)tr.DebOrCred),
        //            new Parametro("@money_type", (int)tr.Currency),
        //            new Parametro("@money_chance", tr.RateAmount),
        //            new Parametro("@bill_date", tr.Date),
        //            new Parametro("@updated_by", usuario.UsuarioId),
        //            new Parametro("@transaction_accounting_id", tr.Id)
        //        };

        //        if (manejador.Ejecutar(sql, lst, CommandType.Text) == 0)
        //        {
        //            mensaje = "No se pudo actualizar ningun registro";
        //            return false;
        //        }
        //        else
        //        {
        //            mensaje = "Se actualizaron datos correctamente";
        //            return true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        mensaje = ex.Message;
        //        return false;
        //    }

        //}
        ///// <summary>
        ///// Devuelve una lista con todas las transacciones activas de ese asiento
        ///// </summary>
        ///// <param name="journalEntry"></param>
        ///// <returns></returns>
        //public List<JournalEntryLine> GetCompleto(JournalEntry journalEntry)
        //{

        //    var retorno = new List<JournalEntryLine>();

        //    var sql = "select " +
        //              "TA.transaction_accounting_id, " +        //0
        //              "TA.reference, " +                        //1
        //              "TA.detail, " +                           //2
        //              "TA.bill_date, " +                        //3
        //              "TA.balance, " +                          //4
        //              "TA.balance_type+0, " +                    //5
        //              "TA.money_type+0, " +                       //6
        //              "TA.money_chance, " +                       //7
        //              "AN.name, " +                             //8
        //              "AC.account_id, " +                       //9
        //              "AC.father_account, " +                    //10
        //              "AC.account_guide+0, " +
        //              " GETFULLPATH(AC.account_id) " +
        //              "FROM transactions_accounting AS TA JOIN accounting_entries AS AE ON TA.accounting_entry_id = AE.accounting_entry_id AND AE.accounting_entry_id = @accounting_entry_id " +
        //              "JOIN accounts AS AC ON TA.account_id = AC.account_id JOIN accounts_names AS AN ON AC.account_name_id = AN.account_name_id " +
        //              "WHERE TA.active = 1 AND AE.active = 1 AND AC.active = 1 ; ";

        //    DataTable dt = manejador.Listado(sql, new Parametro("@accounting_entry_id", journalEntry.Id), CommandType.Text);

        //    foreach (DataRow item in dt.Rows)
        //    {
        //        Object[] vs = item.ItemArray;
        //        JournalEntryLine tran = new JournalEntryLine(
        //            id: Convert.ToInt32(vs[0]),
        //            referencia: Convert.ToString(vs[1]),
        //            detalle: Convert.ToString(vs[2]),
        //            fechaFactura: Convert.ToDateTime(vs[3]),
        //            balance: Convert.ToDecimal(vs[4]),
        //            comportamientoCuenta: ((Comportamiento)Convert.ToInt32(vs[5])),
        //            tipoCambio: ((TipoCambio)Convert.ToInt16(vs[6])),
        //            montoTipoCambio: Convert.ToDecimal(vs[7]),
        //            accountId: new Cuenta
        //            {
        //                Nombre = Convert.ToString(vs[8]),
        //                Id = Convert.ToInt32(vs[9]),
        //                Padre = Convert.ToInt32(vs[10]),
        //                Indicador = (IndicadorCuenta)Convert.ToInt32(vs[11]),
        //                PathDirection = Convert.ToString(vs[12]),
        //            }
        //            );

        //        retorno.Add(tran);

        //    }

        //    return retorno;

        //}
        ////importante, esto no debe de estar enlazado cuando se eliman los asientos
        //// porque si quiera restaurar y hay asientos que fueron previamente elimandos estos tambien
        ////se restaurarian y seria un problema.
        //public String Delete(List<JournalEntryLine> lst, decimal idAsiento, Usuario user)
        //{

        //    using (MySqlTransaction tr = manejador.GetConnection().BeginTransaction(IsolationLevel.Serializable))
        //    {
        //        try
        //        {
        //            var cont = 0;

        //            var sql = "UPDATE transactions_accounting SET active = 0, updated_by = @updated_by " +
        //                      "WHERE transaction_accounting_id = @transaction_accounting_id " +
        //                      "AND accounting_entry_id = @accounting_entry_id";

        //            using (MySqlCommand cmd = new MySqlCommand(sql, tr.Connection, tr))
        //            {
        //                foreach (var item in lst)
        //                {
        //                    cmd.Parameters.Clear();
        //                    cmd.Parameters.AddWithValue("@updated_by", user.UsuarioId);
        //                    cmd.Parameters.AddWithValue("@transaction_accounting_id", item.Id);
        //                    cmd.Parameters.AddWithValue("@accounting_entry_id", idAsiento);
        //                    cont += cmd.ExecuteNonQuery();
        //                    if (cont == 0)
        //                    {
        //                        throw new Exception("No se pudo eliminar este registro");
        //                    }
        //                }
        //            }
        //            tr.Commit();
        //            return $"Se elimino {cont} registros de transacciones";
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

    }
}
