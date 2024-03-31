using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
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
    
        public static List<Account> GetUniqueAccountsByPeriod(this IDictionary<PostingPeriod, List<Account>> reportData)
        {
            return reportData.SelectMany(kv => kv.Value)
                          .GroupBy(account => account.Id)
                          .Select(group => group.First())
                          .ToList();
        }

        public static Account[] TransformUniqueAccounts(this IDictionary<PostingPeriod, List<Account>> allData)
        {
            return allData.GetUniqueAccountsByPeriod()
                    .Select(c => new Account()
                    {
                        Name = c.Name,
                        Id = c.Id,
                        FatherAccount =
                        c.FatherAccount
                    }).ToArray();
        }

        public static List<Account> RemoveAccountWithOutBalances(this IEnumerable<Account> accounts)
        {
            return accounts.Where(x => x.HasBalances() ||
                                (!x.HasBalances() && x.AccountType == AccountType.Cuenta_Titulo))
                                .ToList(); 
        }

        public static Dictionary<PostingPeriod, Account[]> GetReportTable(this IDictionary<PostingPeriod, List<Account>> allData)
        {
            var tablaCuentas = new Dictionary<PostingPeriod, Account[]>();

            foreach (var mes in allData)
            {
                Account[] cuentasConSaldo = allData.TransformUniqueAccounts();

                foreach (var cuenta in cuentasConSaldo)
                {
                    var cuentaMes = mes.Value.FirstOrDefault(cv => cv.Id == cuenta.Id);
                    if (cuentaMes != null)
                    {
                        cuenta.DebOrCred = cuentaMes.DebOrCred;
                        cuenta.AccountTag = cuentaMes.AccountTag;
                        cuenta.AccountType = cuentaMes.AccountType;
                        cuenta.PriorBalance = cuentaMes.PriorBalance;
                        cuenta.PriorBalanceForeign = cuentaMes.PriorBalanceForeign;
                        cuenta.DebitBalance = cuentaMes.DebitBalance;
                        cuenta.CreditBalance = cuentaMes.CreditBalance;
                        cuenta.DebitBalanceForeign = cuentaMes.DebitBalanceForeign;
                        cuenta.CreditBalanceForeign = cuentaMes.CreditBalanceForeign;
                    }
                    else
                    {
                        cuenta.AccountType = AccountType.Cuenta_Auxiliar;
                        cuenta.PriorBalance = 0.00m;
                        cuenta.PriorBalanceForeign = 0.00m;
                        cuenta.DebitBalance = 0.00m;
                        cuenta.CreditBalance = 0.00m;
                        cuenta.DebitBalanceForeign = 0.00m;
                        cuenta.CreditBalanceForeign = 0.00m;
                    }
                }

                tablaCuentas.Add(mes.Key, cuentasConSaldo);
            }
            return tablaCuentas;
        }
    }
}
