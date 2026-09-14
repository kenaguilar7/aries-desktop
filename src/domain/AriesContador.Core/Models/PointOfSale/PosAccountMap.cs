namespace AriesContador.Core.Models.PointOfSale
{
    public class PosAccountMap : BaseModel
    {
        public const string VentasName = "VENTAS";
        public const string IvaPorPagarName = "IVA POR PAGAR";
        public const string CostoMercaderiaName = "COSTO DE MERCADERÍA";
        public const string FaltanteCajaName = "FALTANTE DE CAJA";
        public const string SobranteCajaName = "SOBRANTE DE CAJA";

        public string CompanyId { get; set; }

        public int CashAccountId { get; set; }

        public int CardAccountId { get; set; }

        public int TransferAccountId { get; set; }

        public int SalesAccountId { get; set; }

        public int TaxAccountId { get; set; }

        public int InventoryAccountId { get; set; }

        public int CogsAccountId { get; set; }

        public int CashShortAccountId { get; set; }

        public int CashOverAccountId { get; set; }

        public decimal TaxRate { get; set; } = PosTax.DefaultRate;

        public bool PricesIncludeTax { get; set; } = true;

        public bool IsComplete =>
            CashAccountId > 0
            && CardAccountId > 0
            && TransferAccountId > 0
            && SalesAccountId > 0
            && TaxAccountId > 0
            && InventoryAccountId > 0
            && CogsAccountId > 0
            && CashShortAccountId > 0
            && CashOverAccountId > 0;
    }
}
