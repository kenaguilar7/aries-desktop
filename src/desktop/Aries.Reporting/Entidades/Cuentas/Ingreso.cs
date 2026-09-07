using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using System;

namespace Aries.Reporting.Entidades.Cuentas
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
            return (credito - debito);
        }
    }
}
