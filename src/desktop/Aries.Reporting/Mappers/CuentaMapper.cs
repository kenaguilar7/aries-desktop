using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.PostingPeriods;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.FechaTransacciones;
using Aries.Reporting.Enumeradores;

namespace Aries.Reporting.Mappers
{
    public static class CuentaMapper
    {
        public static Account ToAccount(Cuenta cuenta)
        {
            if (cuenta == null) return null;

            return new Account
            {
                Id = cuenta.Id,
                Name = cuenta.Nombre,
                Memo = cuenta.Detalle,
                Editable = cuenta.Editable,
                Active = cuenta.Active,
                FatherAccount = cuenta.Padre == 0 ? (int?)null : cuenta.Padre,
                CompanyId = cuenta.MyCompania != null ? cuenta.MyCompania.Code : null,
                AccountType = ToAccountType(cuenta.Indicador),
                AccountTag = cuenta.TipoCuenta != null
                    ? (AccountTag)(int)cuenta.TipoCuenta.TipoCuenta
                    : default,
                PriorBalance = cuenta.SaldoAnteriorColones,
                PriorBalanceForeign = cuenta.SaldoAnteriorDolares,
                DebitBalance = cuenta.DebitosColones,
                CreditBalance = cuenta.CreditosColones,
                DebitBalanceForeign = cuenta.DebitosDolares,
                CreditBalanceForeign = cuenta.CreditosDolares,
                PathDirection = cuenta.PathDirection
            };
        }

        public static Cuenta ToCuenta(Account account, Company company)
        {
            if (account == null) return null;

            var cuenta = new Cuenta
            {
                Id = account.Id,
                Nombre = account.Name,
                Detalle = account.Memo,
                Editable = account.Editable,
                Active = account.Active,
                Padre = account.FatherAccount ?? 0,
                MyCompania = company,
                Indicador = ToIndicador(account.AccountType),
                TipoCuenta = Cuenta.GenerarTipoCuenta((int)account.AccountTag),
                SaldoAnteriorColones = account.PriorBalance,
                SaldoAnteriorDolares = account.PriorBalanceForeign,
                DebitosColones = account.DebitBalance,
                CreditosColones = account.CreditBalance,
                DebitosDolares = account.DebitBalanceForeign,
                CreditosDolares = account.CreditBalanceForeign,
                PathDirection = account.PathDirection
            };
            return cuenta;
        }

        public static List<Cuenta> ToCuentaList(IEnumerable<Account> accounts, Company company)
        {
            if (accounts == null) return new List<Cuenta>();
            return accounts.Select(a => ToCuenta(a, company)).ToList();
        }

        public static void CopyBalancesToCuenta(Account from, Cuenta to)
        {
            if (from == null || to == null) return;
            to.Id = from.Id;
            to.SaldoAnteriorColones = from.PriorBalance;
            to.SaldoAnteriorDolares = from.PriorBalanceForeign;
            to.DebitosColones = from.DebitBalance;
            to.CreditosColones = from.CreditBalance;
            to.DebitosDolares = from.DebitBalanceForeign;
            to.CreditosDolares = from.CreditBalanceForeign;
            to.Indicador = ToIndicador(from.AccountType);
        }

        public static void CopyBalancesToCuentas(IEnumerable<Account> from, List<Cuenta> to)
        {
            if (from == null || to == null) return;
            var byId = from.ToDictionary(a => a.Id);
            foreach (var cuenta in to)
            {
                if (byId.TryGetValue(cuenta.Id, out var account))
                    CopyBalancesToCuenta(account, cuenta);
            }
        }

        public static FechaTransaccion ToFechaTransaccion(PostingPeriod period)
        {
            if (period == null) return null;
            return new FechaTransaccion(period.Date, period.Id, period.Closed);
        }

        public static List<FechaTransaccion> ToFechaTransaccionList(IEnumerable<PostingPeriod> periods)
        {
            if (periods == null) return new List<FechaTransaccion>();
            return periods.Select(ToFechaTransaccion).ToList();
        }

        public static AccountType ToAccountType(IndicadorCuenta indicador)
        {
            switch (indicador)
            {
                case IndicadorCuenta.Cuenta_Titulo:
                    return AccountType.Cuenta_Titulo;
                case IndicadorCuenta.Cuenta_De_Mayor:
                    return AccountType.Cuenta_De_Mayor;
                case IndicadorCuenta.Cuenta_Auxiliar:
                    return AccountType.Cuenta_Auxiliar;
                default:
                    return (AccountType)indicador;
            }
        }

        public static IndicadorCuenta ToIndicador(AccountType accountType)
        {
            switch (accountType)
            {
                case AccountType.Cuenta_Titulo:
                    return IndicadorCuenta.Cuenta_Titulo;
                case AccountType.Cuenta_De_Mayor:
                    return IndicadorCuenta.Cuenta_De_Mayor;
                case AccountType.Cuenta_Auxiliar:
                    return IndicadorCuenta.Cuenta_Auxiliar;
                default:
                    return (IndicadorCuenta)accountType;
            }
        }
    }
}
