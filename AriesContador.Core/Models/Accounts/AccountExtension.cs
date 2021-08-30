using System;
using System.Collections.Generic;
using System.Linq; 

namespace AriesContador.Core.Models.Accounts
{
    public static class AccountExtension
    {
        public static void FillPriorBalance(this IEnumerable<Account> accounts, IEnumerable<Account> accountWhitBalance)
        {
            foreach (var account in accounts)
            {
                var priorAccount = accountWhitBalance.First(x => x.Id == account.Id);
                account.PriorBalance = priorAccount.CurrentBalance;
                account.PriorBalanceForeign = priorAccount.CurrentBalanceForeign; 
            }
        }

        public static decimal GetTotalPeridasYGanancias(this IEnumerable<Account> accounts)
        {
            var searchAccounts = accounts.Where(x => x.AccountType == AccountType.Cuenta_Titulo);

            var ingreso = searchAccounts.First(a => a.AccountTag == AccountTag.Ingreso);
            var Egreso = searchAccounts.First(a => a.AccountTag == AccountTag.Egreso);
            var costoVenta = searchAccounts.First(a => a.AccountTag == AccountTag.CostoVenta);

            return ingreso.CurrentBalance - costoVenta.CurrentBalance - Egreso.CurrentBalance;
        }

        //        var ingreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Ingreso);
        //        var Egreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Egreso);
        //        var costoVenta = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Costo_Venta);

        //        return  ingreso.SaldoActualColones - costoVenta.SaldoActualColones - Egreso.SaldoActualColones; 
    }
}
