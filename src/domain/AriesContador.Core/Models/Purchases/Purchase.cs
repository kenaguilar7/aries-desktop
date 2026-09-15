using System;
using System.Collections.Generic;

namespace AriesContador.Core.Models.Purchases
{
    public class Purchase : BaseModel
    {
        public string CompanyId { get; set; }

        public int SupplierId { get; set; }

        /// <summary>Número de factura del proveedor (único por compañía + proveedor).</summary>
        public string DocumentNumber { get; set; }

        public DateTime PurchasedAt { get; set; }

        public PurchaseSettlement PaymentMethod { get; set; }

        public string PaymentReference { get; set; }

        public decimal Total { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public string Notes { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;

        /// <summary>Solo lectura / display (join con suppliers).</summary>
        public string SupplierName { get; set; }

        public List<PurchaseLine> Lines { get; set; } = new List<PurchaseLine>();
    }
}
