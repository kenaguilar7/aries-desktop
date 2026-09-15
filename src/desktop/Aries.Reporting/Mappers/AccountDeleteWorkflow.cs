using System.Collections.Generic;
using AriesContador.Core.Models.Accounts;
using Aries.Reporting.Entidades.Cuentas;

namespace Aries.Reporting.Mappers
{
    /// <summary>
    /// WinForms maestro de cuentas: prepare the Core payload and drop the
    /// row from the in-memory tree list. The form still talks to IFinancialService.
    /// </summary>
    public static class AccountDeleteWorkflow
    {
        public const string SelectAccountMessage = "Seleccione una cuenta";

        public static bool CanDelete(Cuenta cuenta, out string error)
        {
            return TryPrepare(cuenta, updatedBy: 0, out _, out error);
        }

        public static bool TryPrepare(Cuenta cuenta, int updatedBy, out Account account, out string error)
        {
            account = null;
            if (cuenta == null)
            {
                error = SelectAccountMessage;
                return false;
            }

            account = CuentaMapper.ToAccount(cuenta);
            if (account == null || account.Id == 0)
            {
                error = SelectAccountMessage;
                return false;
            }

            account.UpdatedBy = updatedBy;
            if (!AccountRules.CanDelete(account, out error))
                return false;

            error = string.Empty;
            return true;
        }

        public static void RemoveFromList(IList<Cuenta> cuentas, int accountId)
        {
            if (cuentas == null)
                return;

            for (var i = cuentas.Count - 1; i >= 0; i--)
            {
                if (cuentas[i] != null && cuentas[i].Id == accountId)
                    cuentas.RemoveAt(i);
            }
        }
    }
}
