using System;

namespace AriesContador.Core.Models.PointOfSale
{
    public class PosSessionPosting : BaseModel
    {
        public int SessionId { get; set; }

        public string CompanyId { get; set; }

        public int JournalEntryId { get; set; }

        public DateTime PostedAt { get; set; }

        public string TotalsHash { get; set; }
    }
}
