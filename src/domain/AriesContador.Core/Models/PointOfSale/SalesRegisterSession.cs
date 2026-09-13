using System;

namespace AriesContador.Core.Models.PointOfSale
{
    public class SalesRegisterSession : BaseModel
    {
        public int SalesRegisterId { get; set; }

        public string CompanyId { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public decimal OpeningAmount { get; set; }

        public string OpeningNotes { get; set; }

        public decimal CashSales { get; set; }

        public decimal CardSales { get; set; }

        public decimal TransferSales { get; set; }

        public decimal ExpectedClosingAmount { get; set; }

        public decimal? DeclaredClosingAmount { get; set; }

        public decimal? Difference { get; set; }

        public string ClosingNotes { get; set; }

        public string RegisterCode { get; set; }

        public string RegisterName { get; set; }

        public bool IsOpen => !ClosedAt.HasValue;

        public decimal TotalSales => CashSales + CardSales + TransferSales;
    }
}
