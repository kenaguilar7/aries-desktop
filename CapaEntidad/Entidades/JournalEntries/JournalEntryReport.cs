using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.JournalEntries
{
    public class JournalEntryReport
    {
        [DisplayName("Mes Contable")]
        public string PostingPeriodName { get; set; }
        
        [DisplayName("Número de Asiento")]
        public int JournalEntryNumber { get; set; }
        
        [DisplayName("Nombre de Cuenta")]
        public string AccountName { get; set; }

        [DisplayName("Referencia")]
        public string Reference { get; set; }
        
        [DisplayName("Detalle")]
        public string Memo { get; set; }
        [DisplayName("Fecha de Documento")]

        public DateTime DocDate { get; set; }
        
        [DisplayName("Debitos")]
        public decimal DebitAmount { get; set; }
        
        [DisplayName("Creditos")]
        public decimal CreditAmount { get; set; }
        
        [DisplayName("Moneda")]
        public string Currency { get; set; }
        
        [DisplayName("Monto Tipo Cambio")]
        public decimal RateAmount { get; set; }
        
        [DisplayName("Monto Dolares")]
        public decimal ForeignAmount { get; set; }

    }

    public class JournalEntryReportParam
    {
        public string CompanyId { get; set; }
        public string FirstDate { get; set; }
        public string EndDate { get; set; }
    }
}
