namespace AriesContador.Core.Models.Purchases
{
    /// <summary>
    /// Detalle operativo + estado de posteo/pago para la UI de factura.
    /// </summary>
    public class PurchaseDetail
    {
        public Purchase Purchase { get; set; }

        public bool Posted { get; set; }

        public int? JournalEntryId { get; set; }

        public bool Paid { get; set; }

        public SupplierPayment Payment { get; set; }

        /// <summary>Saldo pendiente (Total si no pagada; 0 si pagada). Solo OnAccount.</summary>
        public decimal OutstandingBalance { get; set; }

        public bool CanPay =>
            Purchase != null
            && Purchase.Active
            && Purchase.Status == PurchaseStatus.Confirmed
            && Purchase.PaymentMethod == PurchaseSettlement.OnAccount
            && Posted
            && !Paid
            && OutstandingBalance > 0;
    }
}
