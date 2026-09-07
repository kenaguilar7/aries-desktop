using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using System;

namespace Aries.Reporting.Entidades.Cuentas
{
    public class Patrimonio :  ITipoCuenta
    {
        public TipoCuenta TipoCuenta { get { return TipoCuenta.Patrimonio; } }
        public Comportamiento Comportamiento { get { return Comportamiento.Credito; } }
        public decimal SaldoActual(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo - debito + credito);
        }
        public decimal SaldoMensual(decimal debito, decimal credito)
        {
            //Cresdito - debito
            return (credito - debito);
        }
    }
}
