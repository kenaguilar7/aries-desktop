using Aries.Reporting.Enumeradores;
using System;

namespace Aries.Reporting.Interfaces
{
    public interface ITipoCuenta
    {
        TipoCuenta TipoCuenta { get ; }
        Comportamiento Comportamiento { get; }
        decimal SaldoActual(decimal saldo, decimal debito, decimal credito);
        decimal SaldoMensual(decimal debito, decimal credito); 
    }

}
