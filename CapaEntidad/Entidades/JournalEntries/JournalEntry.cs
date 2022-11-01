using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Utils;
using MathNet.Numerics.Financial; 

namespace CapaEntidad.Entidades.JournalEntries
{
    public class JournalEntry : JournalEntryHeader
    {
        public List<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
        
        [Obsolete]
        public DateTime FechaRegistro { get; set; }

        public override string ToString() => Convert.ToString(Number);

        public decimal DebitosColones => GetMontoTransaccion(DebOrCred.Debito);

        public decimal CreditosColones => GetMontoTransaccion(DebOrCred.Credito);

        public Boolean Cuadrado => (DebitosColones == CreditosColones) ? true : false;

        private decimal GetMontoTransaccion(DebOrCred comportamiento)
        {
            return JournalEntryLines.FindAll(x => x.DebOrCred == comportamiento).Sum(x => x.Amount);
        }
    }
}
