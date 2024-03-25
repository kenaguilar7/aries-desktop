using AriesContador.Core.Models.Accounts.Behavior;
using AriesContador.Core.Models.Utils;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;

namespace AriesContador.Core.Models.Accounts
{
    public class Account : BaseAccount
    {
        private string _name;

        public string Name
        {
            get { return DetailedName; }
            set { _name = value; }
        }

        public string DetailedName
        {
            get { return $"{_name} - {this.DebOrCred.ToString()}"; }
        }
        public int? FatherAccount { get; set; }

        public DebOrCred DebOrCred { get; set; }

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
                if (this.DebOrCred == DebOrCred.Debito)
                { return new Debit(); }
                else { return new Credit(); }
            }
        }


        public decimal DebitBalance { get; set;  }
        public decimal DebitBalanceForeign { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditBalanceForeign { get; set; }

        public bool HasBalances() 
        {
            if (PriorBalance == 0.00m && 
                PriorBalanceForeign == 0.00m && 
                DebitBalance == 0.00m && 
                DebitBalanceForeign == 0.00m && 
                CreditBalance == 0.00m && 
                CreditBalanceForeign == 0.00m)
                return false; 
            return true;

        }
    }
}
