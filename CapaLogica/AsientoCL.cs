using CapaDatos.Daos;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaLogica
{
    public class AsientoCL
    {
        AsientosDao asientoDao = new AsientosDao();
        public bool Delete(Asiento t, Usuario user, out String mensaje)
        {
            return asientoDao.Delete(t, user, out mensaje);
        }
        public DataTable GetDataTable(Compañia t, Usuario user)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Se inserta.
        /// Recomendacion de uso:
        /// se inserta cuando recibe un registro por primer vez 
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Asiento Insert(Asiento t, Usuario user, out String mensaje)
        {
            return asientoDao.Insert(t, user, out mensaje);
        }
        /// <summary>
        /// Actualiza el asiento pasado por parametro
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns>
        /// <param name="Asiento">Devuelve el asiento</param>
        /// </returns>
        public bool Update(Asiento t, Usuario user, out String mensaje)
        {
            return asientoDao.Update(t, user, out mensaje);
        }
        /// <summary>
        /// Busca todos los asientos de una fecha y compañia especifica
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="compania"></param>
        /// <param name="traerInfoCompleta">True si quiere los asientos con todas las transacciones</param>
        /// <param name="traerNuevo">False si quiere que no venga con un nuevo(Esto se usa cuando se quiere regitrar nuevos y se necesita una nuevo(FrameAsiento))/param>
        /// <returns>
        /// <param name="List">Lista con todos los asientos encontrados en esa fecha
        /// y con esa compañia
        /// </param>
        /// </returns>
        public List<Asiento> GetPorFecha(FechaTransaccion fecha, Compañia compania, Boolean traerInfoCompleta = false, Boolean traerNuevo = true)
        {
            List<Asiento> lst = asientoDao.GetPorFecha(fecha, compania, traerInfoCompleta);

            //esto lo unico que hace es generar un nuevo asiento para que se posicione de primer lugar la lista
            if (traerNuevo)
            {
                var dummy = new Asiento(
                             numeroAsiento: asientoDao.GetConsecutivo(compania, fecha.Fecha),
                                 compania: compania
                              )
                {
                    FechaAsiento = fecha
                };

                lst.Insert(0, dummy);
            }

            return lst;

        }
        /// <summary>
        /// Esta funcion nos permite conocer si todos los asientos estas cuadrados
        /// o si hay alguno que este pendiente de cuadrar
        /// nos devovera los asientos pendientes de cuadrar
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="compania"></param>
        /// <returns></returns>
        public List<Asiento> GetMesesPendientes(FechaTransaccion fecha, Compañia compania)
        {

            var dummy = new List<Asiento>();

            foreach (var item in GetPorFecha(fecha, compania, traerInfoCompleta: true))
            {
                if (item.Estado == EstadoAsiento.Proceso)
                {
                    dummy.Add(item);
                }
            }

            return dummy;
        }
        public DataTable ReporteAsientos(Compañia compañia, FechaTransaccion fecha, Boolean traerTodos)
        {
            return asientoDao.ReporteAsientos(compañia, fecha, traerTodos);
        }
        /// <summary>
        /// Esta funcion devuelve una tabla con los asientos 
        /// que tengan desigual cantidad de debitos y creditos
        /// </summary>
        /// <returns></returns>
        public DataTable ListadoAsientosDescuadrados(Compañia compañia, FechaTransaccion fechaTransaccion) {
            return asientoDao.ListadoAsientosDescuadrados( compañia,  fechaTransaccion); 
        }
    }
}
