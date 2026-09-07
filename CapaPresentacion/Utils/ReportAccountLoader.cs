using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Mappers;

namespace CapaPresentacion.Utils
{
    public static class ReportAccountLoader
    {
        public static List<Cuenta> Load(IFinancialService financial, Company company)
        {
            var accounts = financial.GetAccounts(company.Code);
            return CuentaMapper.ToCuentaList(accounts, company);
        }

        public static void FillBalances(IFinancialService financial, List<Cuenta> cuentas, DateTime from, DateTime to)
        {
            if (cuentas == null || cuentas.Count == 0)
                return;

            var accounts = cuentas.Select(CuentaMapper.ToAccount).ToList();
            financial.FillAccountsWithBalances(accounts, from, to);
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
