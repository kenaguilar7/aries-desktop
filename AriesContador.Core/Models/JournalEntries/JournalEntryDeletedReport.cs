using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Attributes;

namespace AriesContador.Core.Models.JournalEntries
{
    public class JournalEntryLineDeletedReport : JournalEntryReport
    {
        public int JournalEntryLineId { get; set; }

        [XLColumn(Header = "Eliminado Por")]
        [DisplayName("Eliminado Por")]
        public string DeletedBy { get; set; }

        [XLColumn(Header = "Fecha de acción")]
        [DisplayName("Fecha de acción")]
        public DateTime DeletedAt { get; set; }
    }

    public class JournalEntryDeletedReport 
    {
        public int JournalEntryId { get; set; }

        [DisplayName("Mes Contable")]
        public string PostingPeriodName { get; set; }

        [DisplayName("Número de Asiento")]
        public int JournalEntryNumber { get; set; }

        [DisplayName("Debitos")]
        public decimal DebitAmount { get; set; }

        [DisplayName("Creditos")]
        public decimal CreditAmount { get; set; }

        [DisplayName("Eliminado Por")]
        public string DeletedBy { get; set; }

        [DisplayName("Fecha de acción")]
      
        public DateTime DeletedAt { get; set; }
    }
}
