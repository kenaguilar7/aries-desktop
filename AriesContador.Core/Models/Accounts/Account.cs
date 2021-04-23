using AriesContador.Core.Models.Accounts.Behavior;
using AriesContador.Core.Models.Utils;
using System.Collections.Generic;
using System.Linq;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Utils;

namespace AriesContador.Core.Models.Accounts
{
    public class Account : BaseAccount
    {
        public List<JournalEntryLine> JournalEntryLines { get; set; }
            = new List<JournalEntryLine>();

        public int? FatherAccount { get; set; }

        public DebOCred DebOCred { get; set; }

        public decimal PriorBalance { get; set; }

        public decimal PriorBalanceForeign { get; set; }

        public decimal CurrentBalance
            => BalanceBehavier
            .CurrentBalance(PriorBalance, GetDebitBalance(), GetCreditBalance());

        public decimal MontlyBalance
            => BalanceBehavier
            .MontlyBalance(GetDebitBalance(), GetCreditBalance());

        private IBalanceBehavior BalanceBehavier
        {
            get
            {
                if (DebOCred == DebOCred.Debito)
                { return new Debit(); }
                else { return new Credit(); }
            }
        }

        public decimal GetDebitBalance()
        {
            var output = JournalEntryLines.Where
                (x => x.DebOrCred == DebOrCred.Debito)
                .Sum(x => x.Amount);

            return output;
        }

        public decimal GetCreditBalance()
        {
            var output = JournalEntryLines.Where
                (x => x.DebOrCred == DebOrCred.Credito)
                .Sum(x => x.Amount);

            return output;
        }

    }
}
