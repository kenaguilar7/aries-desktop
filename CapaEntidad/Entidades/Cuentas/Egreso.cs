using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;


namespace CapaEntidad.Entidades.Cuentas
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
            return (credito - debito);
        }
    }
}
