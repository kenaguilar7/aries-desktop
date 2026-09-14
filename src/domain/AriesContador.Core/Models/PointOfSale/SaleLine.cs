namespace AriesContador.Core.Models.PointOfSale
{
    public class SaleLine : BaseModel
    {
        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public bool SoldByWeight { get; set; }

        public decimal? PricePerKilo { get; set; }

        public decimal WeightGrams { get; set; }

        public decimal LineTotal { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal CostAmount { get; set; }

        public bool TaxExempt { get; set; }

        public decimal StockToDecrement => SoldByWeight ? WeightGrams : Quantity;
    }
}
