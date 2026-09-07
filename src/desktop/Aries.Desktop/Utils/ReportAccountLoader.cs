using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Mappers;

namespace Aries.Desktop.Utils
{
    public static class ReportAccountLoader
    {
        public static async Task<List<Cuenta>> LoadAsync(IFinancialService financial, Company company)
        {
            var accounts = await financial.GetAccountsAsync(company.Code);
            return CuentaMapper.ToCuentaList(accounts, company);
        }

        public static async Task FillBalancesAsync(IFinancialService financial, List<Cuenta> cuentas, DateTime from, DateTime to)
        {
            if (cuentas == null || cuentas.Count == 0)
                return;

            var accounts = cuentas.Select(CuentaMapper.ToAccount).ToList();
            await financial.FillAccountsWithBalancesAsync(accounts, from, to);
            CuentaMapper.CopyBalancesToCuentas(accounts, cuentas);
        }

        public static List<Cuenta> WithoutEmptyBalances(List<Cuenta> cuentas)
        {
            var accounts = cuentas.Select(CuentaMapper.ToAccount).ToList();
            var filtered = AccountRules.RemoveAccountsWithoutBalances(accounts);
            var byId = cuentas.ToDictionary(c => c.Id);
            return filtered.Select(a => byId[a.Id].DeepCopy()).ToList();
        }
    }
}
