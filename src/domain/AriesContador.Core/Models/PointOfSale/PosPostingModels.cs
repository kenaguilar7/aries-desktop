using System;
using System.Collections.Generic;
using AriesContador.Core.Models.JournalEntries;

namespace AriesContador.Core.Models.PointOfSale
{
    public class PosPostingPreview
    {
        public SalesRegisterSession Session { get; set; }

        public decimal CashSales { get; set; }

        public decimal CardSales { get; set; }

        public decimal TransferSales { get; set; }

        public decimal NetSales { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal CostAmount { get; set; }

        public decimal Difference { get; set; }

        public string TotalsHash { get; set; }

        public List<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

        public bool AlreadyPosted { get; set; }

        public int? JournalEntryId { get; set; }

        public int? PostingPeriodId { get; set; }

        public string PeriodName { get; set; }

        public string Warning { get; set; }
    }

    public class PosSessionReconciliationRow
    {
        public int SessionId { get; set; }

        public string RegisterCode { get; set; }

        public string RegisterName { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public decimal TotalSales { get; set; }

        public decimal CashSales { get; set; }

        public decimal CardSales { get; set; }

        public decimal TransferSales { get; set; }

        public decimal NetSales { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal CostAmount { get; set; }

        public decimal Difference { get; set; }

        public bool Posted { get; set; }

        public int? JournalEntryId { get; set; }

        public bool TotalsMatch { get; set; }

        public string Status { get; set; }
    }
}
