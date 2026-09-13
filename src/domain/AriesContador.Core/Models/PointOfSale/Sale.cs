using System;
using System.Collections.Generic;

namespace AriesContador.Core.Models.PointOfSale
{
    public class Sale : BaseModel
    {
        public string CompanyId { get; set; }

        public int SalesRegisterId { get; set; }

        public int SessionId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string PaymentReference { get; set; }

        public decimal Total { get; set; }

        public DateTime SoldAt { get; set; }

        public string RegisterCode { get; set; }

        public string RegisterName { get; set; }

        public List<SaleLine> Lines { get; set; } = new List<SaleLine>();
    }
}
