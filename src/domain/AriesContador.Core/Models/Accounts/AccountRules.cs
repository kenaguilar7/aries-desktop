using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AriesContador.Core.Models.Utils;

namespace AriesContador.Core.Models.Accounts
{
    /// <summary>
    /// Desktop account rules from CuentaCL, on Core Account. No Dapper.
    /// OrderByTree / roll-up reuse ManageAccountListExtension.
    /// </summary>
    public static class AccountRules
    {
        public const string BlankNameMessage = "INGRESE UN NOMBRE VALIDO";
        public const string NameTakenMessage = "Ya existe otra cuenta con este nombre, intente uno diferente";
        public const string NameCannotBeUsedMessage = "El nombre no puede ser utilizado";
        public const string SystemAccountDeleteMessage = "Cuentas del sistema no pueden ser eliminadas.";
        public const string DeleteWithMovementsMessage = "No se pudo eliminar la cuenta porque tiene movimientos";
        public const string DeleteSuccessMessage = "Cuenta eliminada correctamente";
        public const string CreateSuccessMessage = "Cuenta guardada exitosamente";
        public const string UpdateSuccessMessage = "Cuenta actualizada correctamente";

        public static IList<Account> OrderByTree(IEnumerable<Account> accounts)
            => accounts.OrderByTree().ToList();

        public static void ApplyRollUp(IList<Account> accounts)
        {
            if (accounts == null) return;
            accounts.BuildAccountsBalance();
        }

        public static void InheritBalancesIfParentIsAuxiliar(Account child, Account parent)
        {
            if (child == null || parent == null) return;
            if (parent.AccountType != AccountType.Cuenta_Auxiliar) return;

            child.PriorBalance = parent.PriorBalance;
            child.PriorBalanceForeign = parent.PriorBalanceForeign;
            child.DebitBalance = parent.DebitBalance;
            child.CreditBalance = parent.CreditBalance;
            child.DebitBalanceForeign = parent.DebitBalanceForeign;
            child.CreditBalanceForeign = parent.CreditBalanceForeign;
        }

        public static bool HasMovement(Account account)
        {
            if (account == null) return false;
            return account.PriorBalance != 0m
                || account.DebitBalance != 0m
                || account.CreditBalance != 0m;
        }

        public static IList<Account> RemoveAccountsWithoutBalances(IEnumerable<Account> accounts)
        {
            var result = new List<Account>();
            if (accounts == null) return result;

            foreach (var item in accounts)
            {
                if (HasMovement(item) || item.AccountType == AccountType.Cuenta_Titulo)
                    result.Add(item);
            }

            return result;
        }

        public static bool ValidateName(string name, out string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                message = BlankNameMessage;
                return false;
            }

            message = "El nombre puede ser utilizado";
            return true;
        }

        public static bool CanDelete(Account account, out string message)
        {
            if (account == null)
            {
                message = SystemAccountDeleteMessage;
                return false;
            }

            if (!account.Editable)
            {
                message = SystemAccountDeleteMessage;
                return false;
            }

            if (account.AccountType != AccountType.Cuenta_Auxiliar)
            {
                var indicator = account.AccountType.ToString().Replace('_', ' ');
                message = $"Cuentas con la propiedad: {indicator} \n No pueden ser eliminadas";
                return false;
            }

            message = "";
            return true;
        }

        public static string ParentHasMovementsWarning(Account parent)
        {
            var prior = parent?.PriorBalance ?? 0m;
            var debit = parent?.DebitBalance ?? 0m;
            var credit = parent?.CreditBalance ?? 0m;
            return "Esta cuenta posee movimientos, si continua estos seran heredados a la nueva cuenta\n" +
                   $"Saldo Anterior      {string.Format(CultureInfo.InvariantCulture, "{0:₡###,###,###,##0.00##}", prior)}\n" +
                   $"Debitos             {string.Format(CultureInfo.InvariantCulture, "{0:₡###,###,###,##0.00##}", debit)}\n" +
                   $"Creditos            {string.Format(CultureInfo.InvariantCulture, "{0:₡###,###,###,##0.00##}", credit)}\n" +
                   "¿Desea continuar y crear una cuenta nueva?";
        }

        public static string ToYearMonthKey(System.DateTime date)
            => $"{date.Year}{date.Month.ToString("D2")}";
    }
}
