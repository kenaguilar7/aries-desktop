namespace AriesContador.Core.Models.PointOfSale
{
    public class Product : BaseModel
    {
        public string CompanyId { get; set; }

        public string Barcode { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public decimal Stock { get; set; }

        public bool SoldByWeight { get; set; }

        public decimal? PricePerKilo { get; set; }

        public bool TaxExempt { get; set; }
    }
}
