using System.Collections.Generic;

namespace AriesContador.Core.Models.PointOfSale
{
    public class PosTodaySalesReport
    {
        public int SaleCount { get; set; }

        public decimal Total { get; set; }

        public decimal CashTotal { get; set; }

        public decimal CardTotal { get; set; }

        public decimal TransferTotal { get; set; }

        public int ProductCount { get; set; }

        public int LowStockCount { get; set; }

        public IEnumerable<Sale> RecentSales { get; set; } = new List<Sale>();
    }
}
