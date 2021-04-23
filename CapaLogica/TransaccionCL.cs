using CapaDatos.Daos;
using CapaEntidad.Entidades.Usuarios;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class TransaccionCL
    {
        TransaccionDao transaccionDao = new TransaccionDao();
        ///// <summary>
        ///// Inserta la transaccion
        ///// </summary>
        ///// <param name="journalEntryLine"></param>
        ///// <param name="idAsiento"></param>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public JournalEntryLine Insert(JournalEntryLine journalEntryLine, decimal idAsiento, Usuario user)
        //{
        //    return transaccionDao.Insert(journalEntryLine, idAsiento, user);
        //}
        //public String Delete(List<JournalEntryLine> lst, decimal idAsiento, Usuario user)
        //{
        //    return transaccionDao.Delete(lst, idAsiento, user);
        //}
        ///// <summary>
        ///// Devuelve el asiento pasado por parametros lleno, con todas las transacciones activas
        ///// tambien devuelve el estado de "cuadrado" que indica si el asiento fue procesado como correcto por el 
        ///// usuario
        ///// </summary>
        ///// <param name="journalEntry"></param>
        ///// <returns></returns>
        //public List<JournalEntryLine> GetCompleto(JournalEntry journalEntry)
        //{
        //    return transaccionDao.GetCompleto(journalEntry);
        //}
        //public bool Update(JournalEntryLine tr, Usuario usuario, out String mensaje)
        //{
        //    return transaccionDao.Update(tr, usuario, out mensaje); 
        //}
    }
}
