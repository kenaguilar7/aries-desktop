using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;

namespace CapaEntidad.Entidades.Cuentas
{
    public class Ingreso : ITipoCuenta
    {
        public TipoCuenta TipoCuenta { get { return TipoCuenta.Ingreso; } }
        public Comportamiento Comportamiento { get { return Comportamiento.Credito; } }
        public decimal SaldoActual(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo - debito + credito);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="debito"></param>
        /// <param name="credito"></param>
        /// <returns></returns>
        public decimal SaldoMensual(decimal debito, decimal credito)
        {
                /// credito - debito
            return (credito - debito);
        }
    }
}
