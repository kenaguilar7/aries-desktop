using AriesContador.Core.Models.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace AriesContador.Core.Models.Utils
{
    public static class ManageAccountListExtension
    {
        public static IEnumerable<Account> GetLowLevelAccounts(this IEnumerable<Account> cuentas, int accountId)
        {

            List<Account> retorno = GetListWithMainAccount(cuentas, accountId);
            List<Account> auxList1 = retorno;
            List<Account> auxList2 = new List<Account>();

            do
            {
                foreach (Account cuenta in auxList1)
                {
                    auxList2.AddRange(GetAllChildAccounts(cuentas, cuenta));
                }
                retorno.AddRange(auxList2);
                auxList1 = auxList2.ToList();
                auxList2.Clear();

            } while (auxList1.Count() != 0);

            return retorno;
        }

        private static IEnumerable<Account> GetAllChildAccounts(IEnumerable<Account> cuentas, Account cuenta)
            => cuentas.Where(cnt => cnt.FatherAccount == cuenta.Id);

        private static List<Account> GetListWithMainAccount(IEnumerable<Account> cuentas, int accountId)
            => new List<Account> { cuentas.FirstOrDefault(cuenta => cuenta.Id == accountId) };

        public static IEnumerable<Account> GetHightLevelAccounts(this IEnumerable<Account> cuentas, double accountId)
        {
            List<Account> retorno = new List<Account>();
            var cuentaPrincipal = cuentas.First(row => row.Id == accountId);

            var lstaccounts = cuentas.Where(x => x.Id == cuentaPrincipal.FatherAccount || x.Id == accountId);

            retorno.AddRange(lstaccounts);

            foreach (Account cuenta in lstaccounts)
            {
                if (cuenta.Id != accountId)
                {
                    var upLevelOfAccountList = cuentas.Where(x => x.Id == cuenta.FatherAccount);
                    retorno.AddRange(GetLowLevelAccounts(upLevelOfAccountList, cuenta.Id));
                }
            }

            return retorno;
        }

        public static IEnumerable<Account> BuildAccountsBalance(this IEnumerable<Account> cuentas)
        {
            var lstLowerLevel = cuentas.Where(row => row.AccountType == AccountType.Cuenta_Auxiliar);

            foreach (Account lowAccount in lstLowerLevel)
            {
                GetAndSumAllAccountsFromLowerToHigherLevel(cuentas, lowAccount);
            }

            return cuentas;
        }

        private static void GetAndSumAllAccountsFromLowerToHigherLevel(IEnumerable<Account> cuentas,
                                                                                      Account cuentaConSaldo)
        {
            Account cuentaAux = cuentaConSaldo;

            do
            {
                cuentaAux = cuentas.FirstOrDefault(x => x.Id == cuentaAux.FatherAccount);

                if (cuentaAux != null)
                    SumBalance(cuentaConSaldo, cuentaAux);

            } while (cuentaAux != null);

        }
        private static void SumBalance(Account cuentaSumFrom, Account cuentaSumTo)
        {
            //cuentaSumTo.JournalEntryLines.AddRange(cuentaSumFrom.JournalEntryLines);

            cuentaSumTo.CreditBalance += cuentaSumFrom.CreditBalance;
            cuentaSumTo.DebitBalance += cuentaSumFrom.DebitBalance;
            cuentaSumTo.CreditBalanceForeign += cuentaSumFrom.CreditBalanceForeign;
            cuentaSumTo.DebitBalanceForeign += cuentaSumFrom.DebitBalanceForeign;
        }


        public static IEnumerable<Account> OrderByTree(this IEnumerable<Account> lst)
        {

            List<Account> retorno = new List<Account>();

            foreach (Account item in lst)
            {
                if (item.AccountType == AccountType.Cuenta_Titulo)
                {
                    CargarNodos(item);
                }
            }

            void CargarNodos(Account cuenta)
            {
                retorno.Add(cuenta);
                var sql = from c in lst where c.FatherAccount == cuenta.Id select c;
                var cueHijas = sql.ToArray<Account>();
                foreach (Account item in cueHijas)
                {
                    CargarNodos(item);
                }
            }

            return retorno;
        }

        public static IEnumerable<Account> OrderByDescTree(this IEnumerable<Account> lst)
        {
            lst = lst.OrderByDescending(x=>x.Id); 
            List<Account> retorno = new List<Account>();


            foreach (Account item in lst)
            {
                if (item.AccountType == AccountType.Cuenta_Titulo)
                {
                    CargarNodos(item);
                }
            }

            void CargarNodos(Account cuenta)
            {
                var sql = from c in lst where c.FatherAccount == cuenta.Id select c;
                var cueHijas = sql.ToArray<Account>();
                foreach (Account item in cueHijas)
                {
                    CargarNodos(item);
                }
                retorno.Add(cuenta);
            }
            retorno.Reverse();
            return retorno;
        }

        //public static IEnumerable<Account> OrderByDescTree(this IEnumerable<Account> lst)
        //{
        //    List<Account> retorno = new List<Account>();

        //    foreach (Account item in lst)
        //    {
        //        if (item.AccountType == AccountType.Cuenta_Titulo)
        //        {
        //            retorno.AddRange(Test(item, lst));
        //        }
        //    }


        //    return retorno;
        //}

        //private static IEnumerable<Account> Test(Account account, IEnumerable<Account> lst)
        //{
        //    var returnList = new List<Account>
        //    {
        //        account
        //    };

        //    var sql = from c in lst where c.FatherAccount == account.Id select c;

        //    foreach (Account item in sql)
        //    {
        //        if (sql.Count() != 0)
        //            continue; 

        //        returnList.AddRange(Test(item, lst));
        //    }
        //    return returnList;
        //}
    }
}
