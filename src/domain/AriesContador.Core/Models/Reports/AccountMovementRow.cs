using System;

namespace AriesContador.Core.Models.Reports
{
    public class AccountMovementRow
    {
        public string AccountName { get; set; }

        public string MovementType { get; set; }

        public string Memo { get; set; }

        public string Reference { get; set; }

        public DateTime DocumentDate { get; set; }

        public string PostingPeriodName { get; set; }

        public int JournalEntryNumber { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal RateAmount { get; set; }

        public decimal ForeignAmount { get; set; }

        public string UserName { get; set; }

        public DateTime RegisteredAt { get; set; }
    }
}
