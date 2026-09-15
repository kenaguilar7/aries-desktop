using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Models.Purchases
{
    public class PurchaseAccountMap : BaseModel
    {
        public string CompanyId { get; set; }

        public int InventoryAccountId { get; set; }

        /// <summary>IVA SOPORTADO (acreditable).</summary>
        public int TaxAccountId { get; set; }

        /// <summary>CUENTAS POR PAGAR.</summary>
        public int PayableAccountId { get; set; }

        public int CashAccountId { get; set; }

        public int CardAccountId { get; set; }

        public int TransferAccountId { get; set; }

        public decimal TaxRate { get; set; } = PosTax.DefaultRate;

        public bool PricesIncludeTax { get; set; } = true;

        public bool IsComplete =>
            InventoryAccountId > 0
            && TaxAccountId > 0
            && PayableAccountId > 0
            && CashAccountId > 0
            && CardAccountId > 0
            && TransferAccountId > 0;
    }
}
