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
        public DateTime FechaRegistro { get; set; }
        
        public JournalEntry()
        {
        }

        public override string ToString()
        {
            return Convert.ToString(Number);
        }

        public decimal DebitosColones
        {
            get
            {
                return GetMontoTransaccion(DebOrCred.Debito);
            }
        }


        public decimal CreditosColones
        {
            get
            {
                return GetMontoTransaccion(DebOrCred.Credito);
            }
        }

        public Boolean Cuadrado
        {
            get
            {
                return (DebitosColones == CreditosColones) ? true : false;
            }
        }

        private decimal GetMontoTransaccion(DebOrCred comportamiento)
        {
            return JournalEntryLines.FindAll(x => x.DebOrCred == comportamiento).Sum(x => x.Amount);
        }
    }
}
