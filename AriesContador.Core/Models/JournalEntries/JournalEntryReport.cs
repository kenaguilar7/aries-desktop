using System;
using System.ComponentModel;
using ClosedXML.Attributes;

namespace AriesContador.Core.Models.JournalEntries
{
    public class JournalEntryReport
    {
        [XLColumn(Header = "Mes Contable")]
        [DisplayName("Mes Contable")]
        public string PostingPeriodName { get; set; }

        [XLColumn(Header = "Número de Asiento")]
        [DisplayName("Número de Asiento")]
        public int JournalEntryNumber { get; set; }

        [XLColumn(Header = "Nombre de Cuenta")]
        [DisplayName("Nombre de Cuenta")]
        public string AccountName { get; set; }

        [XLColumn(Header = "Referencia")]
        [DisplayName("Referencia")]
        public string Reference { get; set; }

        [XLColumn(Header = "Detalle")]
        [DisplayName("Detalle")]
        public string Memo { get; set; }

        [XLColumn(Header = "Fecha de Documento")]
        [DisplayName("Fecha de Documento")]
        public DateTime DocDate { get; set; }

        [XLColumn(Header = "Debitos")]
        [DisplayName("Debitos")]
        public decimal DebitAmount { get; set; }
        
        [XLColumn(Header = "Creditos")]
        [DisplayName("Creditos")]
        public decimal CreditAmount { get; set; }

        [XLColumn(Header = "Moneda")]
        [DisplayName("Moneda")]
        public string Currency { get; set; }

        [XLColumn(Header = "Monto Tipo Cambio")]
        [DisplayName("Monto Tipo Cambio")]
        public decimal RateAmount { get; set; }

        [XLColumn(Header = "Monto Dólares ")]
        [DisplayName("Monto Dólares ")]
        public decimal ForeignAmount { get; set; }
    }

    public class BasicReportParam
    {
        public string CompanyId { get; set; }
        public string FirstDate { get; set; }
        public string EndDate { get; set; }
    }
}
