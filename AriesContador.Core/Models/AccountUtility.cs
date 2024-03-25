using AriesContador.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriesContador.Core.Models
{
    public static class AccountUtility
    {

        public static string[] GetPathName(this Account account, List<Account> accounts)
        {
            var hierarchy = new List<string>();
            var currentAccount = account;

            do
            {
                hierarchy.Insert(0, currentAccount.Name); // Insert at the beginning to build the hierarchy in reverse order
                currentAccount = accounts.FirstOrDefault(acc => acc.Id == currentAccount.FatherAccount);
            } while (currentAccount != null);

            if (hierarchy.Count > 1)
            {
                hierarchy.RemoveAt(hierarchy.Count - 1); // Remove the last element
                hierarchy.Add(account.Name); // Add the current account's name at the end
            }

            // Fill with blanks except the last element
            return hierarchy.Select((name, index) => index < hierarchy.Count - 1 ? "" : name).ToArray();
        }

        
    }
}
