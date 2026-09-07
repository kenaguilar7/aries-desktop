using AriesContador.Core.Models.Accounts.Behavior;
using AriesContador.Core.Models.Utils;

namespace AriesContador.Core.Models.Accounts
{
    public class Account : BaseAccount
    {
        //public List<JournalEntryLine> JournalEntryLines { get; set; }
        //    = new List<JournalEntryLine>();

        public int? FatherAccount { get; set; }

        public DebOrCred DebOCred { get; set; }

        public decimal PriorBalance { get; set; }

        public decimal PriorBalanceForeign { get; set; }

        public decimal CurrentBalance
            => BalanceBehavior
            .CurrentBalance(PriorBalance, DebitBalance, CreditBalance);

        public decimal CurrentBalanceForeign
            => BalanceBehavior
                .CurrentBalance(PriorBalanceForeign, DebitBalanceForeign, CreditBalanceForeign);

        public decimal MontlyBalance
            => BalanceBehavior
            .MontlyBalance(DebitBalance, CreditBalance);

        public decimal MontlyBalanceForeign
            => BalanceBehavior
                .MontlyBalance(DebitBalanceForeign, CreditBalanceForeign);

        private IBalanceBehavior BalanceBehavior
        {
            get
            {
                if (DebOCred == DebOrCred.Debito)
                { return new Debit(); }
                else { return new Credit(); }
            }
        }

        public decimal DebitBalance { get; set;  }
        public decimal DebitBalanceForeign { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditBalanceForeign { get; set; }


        //public decimal GetCreditBalance()
        //{
        //    var output = JournalEntryLines.Where
        //        (x => x.DebOrCred == DebOrCred.Credito)
        //        .Sum(x => x.Amount);

        //    return output;
        //}

    }
}
