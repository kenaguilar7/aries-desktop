using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Utils;
using System;

namespace CapaEntidad.Entidades.JournalEntries
{
    public class JournalEntryLine : BaseModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountPath { get; set; }
        public int JournalEntryId { get; set; }
        public string Reference { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }
        public decimal RateAmount { get; set; } = 1.00m; 

        private decimal _monto;
        public decimal Monto {
            get { return _monto; }
            ///Todo número se almacenara siempre con dos decimales
            set { _monto = Decimal.Truncate(value * 100) / 100; ; }
        }

        public decimal Amount { get; set; }

        public decimal ForeignAmount
        {
            get { return (Currency == Currency.dolares) ? Amount / RateAmount : 0; }
            set { }
        }

        public DebOrCred DebOrCred { get; set; }

        public JournalEntryLine() {
            
        }

        
    }
}
