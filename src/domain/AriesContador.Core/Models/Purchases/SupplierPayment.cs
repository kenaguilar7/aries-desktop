using System;

namespace AriesContador.Core.Models.Purchases
{
    /// <summary>
    /// Pago a proveedor (v1: liquidación total de una factura a crédito ya asentada).
    /// </summary>
    public class SupplierPayment : BaseModel
    {
        public string CompanyId { get; set; }

        public int SupplierId { get; set; }

        /// <summary>Factura liquidada; unique en v1.</summary>
        public int PurchaseId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaidAt { get; set; }

        /// <summary>Cash / Card / Transfer (no OnAccount).</summary>
        public PurchaseSettlement PaymentMethod { get; set; }

        public string PaymentReference { get; set; }

        public int JournalEntryId { get; set; }

        public string Notes { get; set; }

        /// <summary>Display.</summary>
        public string SupplierName { get; set; }

        /// <summary>Display.</summary>
        public string DocumentNumber { get; set; }
    }
}
