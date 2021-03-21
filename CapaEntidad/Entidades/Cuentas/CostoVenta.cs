using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;


namespace CapaEntidad.Entidades.Cuentas
{
    public class CostoVenta : ITipoCuenta
    {
        public TipoCuenta TipoCuenta { get { return TipoCuenta.Costo_Venta; } }
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
