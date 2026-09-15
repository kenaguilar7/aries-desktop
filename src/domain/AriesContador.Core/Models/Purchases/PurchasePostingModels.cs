using System.Collections.Generic;
using System.Globalization;
using AriesContador.Core.Models.JournalEntries;

namespace AriesContador.Core.Models.Purchases
{
    public class PurchasePostingPreview
    {
        public Purchase Purchase { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Total { get; set; }

        public PurchaseSettlement PaymentMethod { get; set; }

        public string TotalsHash { get; set; }

        public List<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

        public bool AlreadyPosted { get; set; }

        public int? JournalEntryId { get; set; }

        public int? PostingPeriodId { get; set; }

        public string PeriodName { get; set; }

        public string Warning { get; set; }
    }

    public static class PurchasePostingHash
    {
        public static string Compute(decimal net, decimal tax, decimal total, PurchaseSettlement method)
        {
            return string.Join("|", new[]
            {
                net.ToString("0.00", CultureInfo.InvariantCulture),
                tax.ToString("0.00", CultureInfo.InvariantCulture),
                total.ToString("0.00", CultureInfo.InvariantCulture),
                PurchaseSettlementNames.ToDb(method)
            });
        }
    }
}
