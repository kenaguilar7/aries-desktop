using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using System;


namespace Aries.Reporting.Entidades.Cuentas
{
    public class Pasivo : ITipoCuenta
    {
        public TipoCuenta TipoCuenta { get { return TipoCuenta.Pasivo; } }
        public Comportamiento Comportamiento { get { return Comportamiento.Credito; } }
        public decimal SaldoActual(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo - debito + credito);
        }
        public decimal SaldoMensual(decimal debito, decimal credito)
        {
            ///credito - debito
            return (credito - debito);
        }
    }
}
