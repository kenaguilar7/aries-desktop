using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Accounts.Behavior
{
    public interface IBalanceBehavior
    {
        decimal CurrentBalance(decimal priorBalance, decimal debitBalance, decimal creditBalance);

        decimal MontlyBalance(decimal debitBalance, decimal creditBalance);
    }
}
