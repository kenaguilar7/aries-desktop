using System;

namespace AriesContador.Core.Models.Purchases
{
    public class PurchasePosting : BaseModel
    {
        public int PurchaseId { get; set; }

        public string CompanyId { get; set; }

        public int JournalEntryId { get; set; }

        public DateTime PostedAt { get; set; }

        public string TotalsHash { get; set; }
    }
}
