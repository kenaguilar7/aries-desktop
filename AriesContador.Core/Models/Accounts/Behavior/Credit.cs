using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Accounts.Behavior
{
    public class Credit : IBalanceBehavior
    {
        public decimal CurrentBalance(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo - debito + credito);
        }

        public decimal MontlyBalance(decimal debito, decimal credito)
        {
            return (credito - debito);
        }
    }
}
