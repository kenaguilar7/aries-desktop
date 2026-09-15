namespace AriesContador.Core.Models.Purchases
{
    public class PurchaseLine : BaseModel
    {
        public int PurchaseId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        /// <summary>Precio unitario bruto si PricesIncludeTax.</summary>
        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public bool TaxExempt { get; set; }

        /// <summary>Neto de la línea; el costo unitario del producto es NetAmount / Quantity.</summary>
        public decimal CostAmount { get; set; }
    }
}
