using CapaDatos.Daos;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.Usuarios;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class TransaccionCL
    {
        TransaccionDao transaccionDao = new TransaccionDao();
        /// <summary>
        /// Inserta la transaccion
        /// </summary>
        /// <param name="transaccion"></param>
        /// <param name="idAsiento"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Transaccion Insert(Transaccion transaccion, decimal idAsiento, Usuario user)
        {
            return transaccionDao.Insert(transaccion, idAsiento, user);
        }
        public String Delete(List<Transaccion> lst, decimal idAsiento, Usuario user)
        {
            return transaccionDao.Delete(lst, idAsiento, user);
        }
        /// <summary>
        /// Devuelve el asiento pasado por parametros lleno, con todas las transacciones activas
        /// tambien devuelve el estado de "cuadrado" que indica si el asiento fue procesado como correcto por el 
        /// usuario
        /// </summary>
        /// <param name="asiento"></param>
        /// <returns></returns>
        public List<Transaccion> GetCompleto(Asiento asiento)
        {
            return transaccionDao.GetCompleto(asiento);
        }
        public bool Update(Transaccion tr, Usuario usuario, out String mensaje)
        {
            return transaccionDao.Update(tr, usuario, out mensaje); 
        }
    }
}
