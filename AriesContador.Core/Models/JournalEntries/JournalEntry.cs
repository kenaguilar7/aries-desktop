using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Utils; 

namespace AriesContador.Core.Models.JournalEntries
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

        /// <summary>
        /// Regla de FrameAsientos: no se puede salir ni cambiar de compañía si el asiento
        /// ya está persistido (Id != 0) y no está cuadrado. Borrador (Id == 0) sí se puede cerrar.
        /// </summary>
        public bool CanNavigateAway()
        {
            if (Id == 0) return true;
            return Cuadrado;
        }

        /// <summary>
        /// Misma regla que ValidateEqualDebAndCred en FrameAsientos.
        /// </summary>
        public void ApplyStatusFromBalance()
        {
            JournalEntryStatus = Cuadrado
                ? JournalEntryStatus.Approved
                : JournalEntryStatus.Progress;
        }

        private decimal GetMontoTransaccion(DebOrCred comportamiento)
        {
            return JournalEntryLines.FindAll(x => x.DebOrCred == comportamiento).Sum(x => x.Amount);
        }
    }
}
