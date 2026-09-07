using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Accounts.Behavior
{
    class Debit : IBalanceBehavior
    {
        public decimal CurrentBalance(decimal saldoAnterior, decimal debitos, decimal creditos)
        {
            return (saldoAnterior + debitos - creditos);
        }

        public decimal MontlyBalance(decimal debitos, decimal creditos)
        {
            return (debitos - creditos);
        }
    }
}
