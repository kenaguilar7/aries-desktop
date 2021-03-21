using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;

namespace CapaLogica
{
    public class CuentaCL
    {
        CuentaDao cuentaDao = new CuentaDao();
        FechaTransaccionCL _fechaTransaccionCL = new FechaTransaccionCL();
        public Boolean Deleted(Cuenta cuenta, Usuario usuario, out String mensaje)
        {
            if (!cuenta.Editable || cuenta.Indicador != IndicadorCuenta.Cuenta_Auxiliar)
            {
                mensaje = (cuenta.Editable) ? $"Cuentas con la propiedad: {cuenta.Indicador.ToString().Replace('_', ' ') } \n No pueden ser eliminadas" : $"Cuentas del sistema no pueden ser eliminadas.";  ;
                return false;
            }
            return cuentaDao.Deleted(cuenta, usuario, out mensaje);
        }
        public List<Cuenta> GetAll(Compañia t)
        {
            var cuentas = cuentaDao.GetAll(t);
            return Ordernar(cuentas);
        }
        public Boolean Insert(ref Cuenta nuevaCuenta, Cuenta cuentaPadre, out String Mensaje, Usuario user)
        {

            if (VerificarNombre(nuevaCuenta, nuevaCuenta.Nombre, out Mensaje, nuevaCuenta.MyCompania))
            {

                ///Reglas de negocio para heredar saldo
                /// Si la cuenta es auxiliar tiene que heredar el saldo de la anterior
                /// en el siguiente codigo estamos heredandole los debitos y credito y saldos anteriores
                /// aunque esos saldos no se vayan a insertar en la base de datos, lo jacemos para retornarla
                /// y el callingform pueda ver reflejada esos movimientos
                ///
                if (cuentaPadre.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
                {
                    nuevaCuenta.SaldoAnteriorColones = cuentaPadre.SaldoAnteriorColones;
                    nuevaCuenta.SaldoAnteriorDolares = cuentaPadre.SaldoAnteriorDolares;
                    nuevaCuenta.DebitosColones = cuentaPadre.DebitosColones;
                    nuevaCuenta.CreditosColones = cuentaPadre.CreditosColones;
                    nuevaCuenta.DebitosDolares = cuentaPadre.DebitosDolares;
                    nuevaCuenta.CreditosDolares = cuentaPadre.CreditosDolares;
                    //cambiarle el estado despues de insertar en la base de datos
                    //cuentaPadre.Indicador = IndicadorCuenta.Cuenta_De_Mayor; 
                }

                if (cuentaDao.Insert(ref nuevaCuenta, cuentaPadre, user, out Mensaje))
                {
                    Mensaje = "Cuenta guardada exitosamente";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public DataTable GetInfoCompleta(Cuenta cuenta)
        {
            var retorno = new DataTable();
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                retorno = cuentaDao.GetInfoCompletaCuentaAux(cuenta);
            }
            else
            {
                retorno = cuentaDao.GetInforCompletaCuentaMayor(cuenta);
            }


            //decimal acumulado = 0;

            decimal lastSaldoActual = 0m;
            foreach (DataRow item in retorno.Rows)
            {
                var rw = item["Saldo Actual"];
                decimal debito = String.IsNullOrWhiteSpace(item["Debito"].ToString()) ? 0m : Convert.ToDecimal(item["Debito"]);
                ITipoCuenta tpcnta = Cuenta.GenerarTipoCuenta(Convert.ToInt32(rw));
                lastSaldoActual = tpcnta.SaldoActual(saldo: lastSaldoActual, debito: debito, credito: string.IsNullOrWhiteSpace(item["Credito"].ToString()) ? 0m : Convert.ToDecimal(item["Credito"]));
                //acumulado += lastSaldoActual;

                item["Saldo Actual"] = string.Format("{0:n}", lastSaldoActual);


            }




            return retorno;
        }
        public Boolean Update(ref Cuenta cuenta, Usuario user, String nuevoNombre, Compañia compañia, String nuevaDesc, out String mensaje)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(nuevoNombre))
                {
                    mensaje = "INGRESE UN NOMBRE VALIDO";
                    return false;
                }
                else
                {
                    mensaje = "";

                    if (cuentaDao.VerificarNombre(cuenta, nuevoNombre, compañia))
                    {
                        cuenta.Nombre = nuevoNombre;
                        cuenta.Detalle = nuevaDesc;


                        return cuentaDao.UpdateNameInfo(cuenta, user, out mensaje);


                    }
                    else
                    {
                        mensaje = "Ya existe otra cuenta con este nombre, intente uno diferente";
                        return false;
                    }


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
            return cuentaDao.UpdatesettInfo(t, user, out mensaje); 
        }

        /// <summary>
        /// Vefica que no hayan mas cuentas con el mismo nombre 
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="tipoCuenta"></param>
        /// <param name="mensaje"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public Boolean VerificarNombre(Cuenta cuenta, String nombre, out String mensaje, Compañia compañia)///mejorar 
        {
            if (String.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "INGRESE UN NOMBRE VALIDO";
                return false;
            }
            else
            {

                if (cuentaDao.VerificarNombre(cuenta, nombre, compañia))
                {
                    mensaje = "El nombre puede ser utilizado";
                    return true;
                }
                else
                {
                    mensaje = "El nombre no puede ser utilizado";
                    return false;
                }


            }
        }
        public void LLenarConSaldos(DateTime fechaInicio, DateTime fechaFinal, List<Cuenta> lst, Compañia compañia)
        {
            lst.ForEach(delegate (Cuenta c)
            {
                //c.SaldoAnteriorColones = 0.00;
                //c.SaldoAnteriorDolares = 0.00;
                c.DebitosColones = 0.00m;
                c.CreditosColones = 0.00m;
                c.DebitosDolares = 0.00m;
                c.CreditosDolares = 0.00m;
            });

            cuentaDao.CuentaConSaldos(lst, compañia, fechaInicio, fechaFinal);

            ///Pasar este codigo a un metodo independiente

            ///LLenamos tambien la cuenta padre
            ///Como solo trae el saldo de las cuentas axiliares 
            ///entonces llenamos el arbol de cuentas hacia arriba
            foreach (var item in lst)
            {
                if (item.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
                {

                    var dummy = item;

                    while ((dummy = BuscarCuentaPadre(lst, dummy)) != null)
                    {
                        //dummy.SaldoAnteriorColones += item.SaldoAnteriorColones;
                        //dummy.SaldoAnteriorDolares += item.SaldoAnteriorDolares; 
                        dummy.DebitosColones += item.DebitosColones;
                        dummy.CreditosColones += item.CreditosColones;
                        dummy.DebitosDolares += item.DebitosDolares;
                        dummy.CreditosDolares += item.CreditosDolares;

                    }
                }
            }


        }
        public Cuenta BuscarCuentaPadre(List<Cuenta> lst, Cuenta cuentaHija)
        {
            if (lst.Count != 0)
            {
                foreach (Cuenta item in lst)
                {
                    if (item.Id == cuentaHija.Padre)
                    {
                        return item;
                    }

                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public List<Cuenta> Ordernar(List<Cuenta> lst)
        {

            List<Cuenta> retorno = new List<Cuenta>();

            ///Recorremos la lista para obtener solamente 
            ///Las cuentas guias
            foreach (Cuenta item in lst)
            {
                if (item.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    ///Por cada cuenta guia buscamos su hija
                    CargarNodos(item);
                }
            }

            ///buscamos las cuentas hijas 
            void CargarNodos(Cuenta cuenta)
            {
                ///toda cuenta que sea pasada por parametro sera agregada 
                ///de tal modo que siempre que se encuentre una hija
                ///esta sera agregada usara recursividas y si encuentra mas 
                ///el proceso se repetira
                retorno.Add(cuenta);

                var sql = from c in lst where c.Padre == cuenta.Id select c;

                ///devuelve todas las cuentas hijas
                var cueHijas = sql.ToArray<Cuenta>();

                ///recorremos todas
                foreach (Cuenta item in cueHijas)
                {
                    ///Usamos recursividad para buscar todas las cuentas hijas
                    CargarNodos(item);
                }
            }

            return retorno;
        }
        public Boolean VerificarSiEsApta(Cuenta cuentaPadre, out String Mensaje)
        {
            List<FechaTransaccion> meses = _fechaTransaccionCL.GetAllActive(cuentaPadre.MyCompania, null);

            if (meses.Count != 0)
            {
                var cuentaDummy = cuentaPadre.DeepCopy();
                var dummy = new List<Cuenta> { cuentaDummy };

                LLenarConSaldos(meses.Last().Fecha, meses[0].Fecha, dummy, cuentaPadre.MyCompania);

                if (cuentaDummy.Indicador == IndicadorCuenta.Cuenta_Auxiliar && cuentaDummy.CuentaConMovientos())
                {
                    Mensaje = $"Esta cuenta posee movimientos, si continua estos seran heredados a la nueva cuenta\n" +
                               $"Saldo Anterior      {string.Format("{0:₡###,###,###,##0.00##}", cuentaDummy.SaldoAnteriorColones)}\n" +
                               $"Debitos             {string.Format("{0:₡###,###,###,##0.00##}", cuentaDummy.DebitosColones)}\n" +
                               $"Creditos            {string.Format("{0:₡###,###,###,##0.00##}", cuentaDummy.CreditosColones)}\n" +
                               $"¿Desea continuar y crear una cuenta nueva?";
                    return false;
                }
            }

            Mensaje = "";
            return true;


        }
        /// <summary>
        /// Retorna una copia de la lista 
        /// sin las cuentas que no tienen saldos
        /// segun la logica de negocio
        /// </summary>
        /// <param name="lis"></param>
        /// <returns></returns>
        public List<Cuenta> QuitarCuentasSinSaldos(List<Cuenta> lis)
        {

            var retorno = new List<Cuenta>();

            foreach (var item in lis)
            {
                if (item.CuentaConMovientos() || !item.CuentaConMovientos() && item.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    retorno.Add(item.DeepCopy());
                }
            }

            return retorno;
        }

        public Boolean GenerarSaldosEnCeroParaCierreDeAsieto(Cuenta cuentaSaldoAsiento, Compañia compañia, Usuario usuario, int limitSec) {
            return cuentaDao.GenerarSaldosEnCeroParaCierreDeAsieto(cuentaSaldoAsiento, compañia, usuario, limitSec);
        }

    }

}