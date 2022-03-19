using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace CapaLogica
{
    public class FechaTransaccionCL
    {
        private FechaTransaccionDao _fechaDao = new FechaTransaccionDao();
        private AsientoCL _asientoCL = new AsientoCL();

        public List<FechaTransaccion> GetAll(Compañia t, Usuario user)
        {
            return _fechaDao.GetAll(t, user);
        }
        public DataTable GetDataTable(Compañia t, Usuario user)
        {
            return _fechaDao.GetDataTable(t, user);
        }
        public Boolean Insert(FechaTransaccion t, Compañia compañia, Usuario user, out String mensaje)
        {
            var lstTodosMeses = this.GetAll(compañia, user);
            ///Se va a hacer un indice unico en la base de datos para evitar esto 
            ///quita-----
            if (lstTodosMeses.Find(ms => ms.Fecha == t.Fecha) != null)
            {
                mensaje = $"Este mes ya se encuentra registrado";
                return false;
            }

            else
            {
                if (_fechaDao.Insert(t, compañia, user, out mensaje))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Devuelve los meses disponibles para abrir.
        /// 1. solo devolvera dos meses, el ultimo pasado y el primero presente
        /// 2. 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<FechaTransaccion> FechaAbrirMes(Compañia t, Usuario user)
        {
            List<FechaTransaccion> lst = _fechaDao.GetAll(t, user);


            List<FechaTransaccion> retorno = new List<FechaTransaccion>();

            if (lst.Count == 0)
            {
                //si la cuenta es cero, quiere decir que no hay meses entonces se pone a disposicion 
                //el mes actual para partir de ahi.
                var mesActual = (DateTime.Today);

                retorno.Add(new FechaTransaccion(
                        id: 332,
                        fecha: mesActual,
                        cerrada: false
                    ));
            }
            else
            {

                var ultimoprente = (from c in lst orderby c.Fecha descending select c).ToArray<FechaTransaccion>()[0];

                var ultimopasado = (from c in lst orderby c.Fecha ascending select c).ToArray<FechaTransaccion>()[0];



                FechaTransaccion pasado = new FechaTransaccion()
                {
                    Fecha = ultimopasado.Fecha.AddMonths(-1)
                };

                FechaTransaccion presente = new FechaTransaccion()
                {
                    Fecha = ultimoprente.Fecha.AddMonths(1)
                };
                    

                retorno.Add(pasado);
                retorno.Add(presente);

            }
            return retorno;
        }
        /// <summary>
        /// lista con todos los meses abiertos
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<FechaTransaccion> FechaCerrarMes(Compañia t, Usuario user)
        {
            List<FechaTransaccion> lst = _fechaDao.GetAll(t, user);
            List<FechaTransaccion> retorno = new List<FechaTransaccion>();
            var lis1 = (from c in lst where c.Cerrada == false orderby c.Fecha ascending select c).ToArray<FechaTransaccion>();

            if (lis1.Count() > 0)
            {
                var ultimoprente = lis1[0];



                var lis2 = (from c in lst where c.Cerrada == false orderby c.Fecha descending select c).ToArray<FechaTransaccion>();

                if (lis2.Count() > 0)
                {
                    var ultimopasado = lis2[0];

                    //Este if lo que hace es evitar que cuando solo haya un mes este se repita dos veces
                    if (ultimoprente.Id == ultimopasado.Id)
                    {
                        retorno.Add(ultimoprente);
                    }
                    else
                    {
                        retorno.Add(ultimoprente);
                        retorno.Add(ultimopasado);
                    }
                }

            }

            if (lst.Count == 1)
            {

                if (!lst[0].Cerrada)
                {
                    retorno.Add(lst[0]);
                }
            }

            return retorno;
        }
        public List<FechaTransaccion> GetAllActive(Compañia t, Usuario user, Boolean traerAsientos = false)
        {
            List<FechaTransaccion> retorno = GetAll(t, user);
            var Lstretorno = retorno.OrderBy(x=> x.Fecha).ToList<FechaTransaccion>();
            if (traerAsientos)
            {
                foreach (var item in Lstretorno)
                {
                    //var lstAsiento = _asientoCL.GetPorFecha(item, t, traerInfoCompleta: true, traerNuevo: false);

                    //item.Asientos = lstAsiento;
                }

            }

            return Lstretorno;
        }
        public Boolean CerrarMes(FechaTransaccion t, Compañia compañia, Usuario user, out String mensaje)
        {
            ///Primero verificamos que este mes sea apto para cerrarse
            var meses = this.FechaCerrarMes(compañia, user);

            if (meses[0].Fecha == t.Fecha)
            {
                var cuentaCL = new CuentaCL();

                if (!BuscarAsientosDescuadrados(compañia, t, out mensaje))
                {
                    var lstCuentas = cuentaCL.GetAll(compañia);

                    cuentaCL.LLenarConSaldos(t.Fecha, t.Fecha, lstCuentas, compañia);

                    if (_fechaDao.CerrarMes(t, compañia, user, lstCuentas, out mensaje))
                    {
                        mensaje = "Se cerro correctamente el mes!!!";
                        return true;
                    }
                    else
                    {
                        mensaje = "No se pudo cerrar el mes!!!";
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            else
            {
                mensaje = "Este mes no se puede cerrar";
                return false;
            }


        }

        /// <summary>
        /// Devulve true si encontro asientos descuadrados y falso si todo esta bien
        /// </summary>
        /// <param name="compañia"></param>
        /// <param name="fechaTransaccion"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public Boolean BuscarAsientosDescuadrados(Compañia compañia, FechaTransaccion fechaTransaccion, out String mensaje)
        {

            var asientCL = new AsientoCL();

            //var lstCuentasPedientes = asientCL.ListadoAsientosDescuadrados(compañia, fechaTransaccion);
            //if (lstCuentasPedientes.Rows.Count > 0)
            //{
            //    mensaje = "Faltan asientos por cuadrar: \n";
            //    mensaje += $"Mes contable: {fechaTransaccion.ToString()} \n";
            //    foreach (DataRow item in lstCuentasPedientes.Rows)
            //    {
            //        object[] vs = item.ItemArray;
            //        mensaje += $"Asiento: {vs[1]}- Debitos: {vs[2]} Creditos: {vs[3]} \n";
            //    }

            //    return true;
            //}
            //else
            //{
            //    mensaje = "tbd";
            //    return false;
            //}
            mensaje = "";
            return true;

        }

        public List<Cuenta> AsientoDeCierre(Compañia compañia, Usuario usuario, IEnumerable<FechaTransaccion> fechaTransaccions, Cuenta cuenta)
        {
            var meses = this.FechaCerrarMes(compañia, usuario);

            var mensaje = "";
            var firstMont = fechaTransaccions.OrderBy(x => x.Fecha).LastOrDefault();
            //var lst = fechaTransaccions.OrderByDescending(x => x.Fecha).ToList(); 
            //var lst2 = fechaTransaccions.OrderBy(x => x.Fecha); 
            foreach (FechaTransaccion item in fechaTransaccions.OrderBy(x => x.Fecha))

            {
                item.Cerrada = true;
                if (!CerrarMes(item, compañia, usuario, out mensaje))
                {
                    throw new Exception(mensaje);
                }

            }
            ///Una vez cerrado todos los mese debemos 
            ///1- sacar el saldo del balance de comprobacion. 
            ///   para esto usamos el ultimo mes en la lista
            ///2- Actualizar todas las cuentas en saldo cero y
            ///   poner el saldo en una cuenta
            ///   
            ///  ***Guardar una copia de las cuentas para retornarla y generar el balance de 
            ///  comprobación y que este pueda ser impreso
            CuentaCL _cuentaCL = new CuentaCL();
            var cuentas = _cuentaCL.GetAll(compañia);
            _cuentaCL.LLenarConSaldos(meses[0].Fecha, firstMont.Fecha, cuentas, compañia);
            var _cuenta = SaldoReporComprobacion(cuentas, cuenta);

            var rsl = _cuentaCL.GenerarSaldosEnCeroParaCierreDeAsieto(_cuenta, compañia, usuario, cuentas.Count);
            if (!rsl)
            {
                throw new Exception("error al guardar el saldo");
            }


            return cuentas;
        }
        /// <summary>
        /// Este metodo no se puede quedar aqui cambiar, cambiar!!!
        /// </summary>
        private Cuenta SaldoReporComprobacion(List<Cuenta> list, Cuenta cuenta)
        {

            ///total debitos colones
            var TotalColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
            .Sum(c => c.SaldoActualColones);
            ///total credito colone
            var TotalColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
            .Sum(c => c.SaldoActualColones);
            ///total debito dolares
            var TotalDolaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
            .Sum(c => c.SaldoActualDolares);
            ///total credito dolares
            var TotalDolaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
            .Sum(c => c.SaldoActualDolares);

            cuenta.DebitosColones = TotalColonesDebito;
            cuenta.CreditosColones = TotalColonesCredito;
            cuenta.DebitosDolares = TotalDolaresDebito;
            cuenta.CreditosDolares = TotalDolaresCredito;

            return cuenta;
        }
    }
}
