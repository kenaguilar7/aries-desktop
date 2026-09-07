using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using System;


namespace Aries.Reporting.Entidades.Cuentas
{
    public class Egreso : ITipoCuenta
    {
        public TipoCuenta TipoCuenta { get { return TipoCuenta.Egreso; } }
        public Comportamiento Comportamiento { get { return Comportamiento.Debito; } }
        public decimal SaldoActual(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo + debito - credito);
        }
        public decimal SaldoMensual(decimal debito, decimal credito)
        {
            return (debito - credito);
        }
    }
}
