using AriesContador.Core.Models.Utils;
using System;

namespace AriesContador.Core.Models.JournalEntries
{
    /// <summary>
    /// Fórmula de monto que usa FrameAsientos al armar una línea.
    /// Colones: tipo de cambio 1 y el monto digitado es Amount.
    /// Dólares: el monto digitado es ForeignAmount; Amount = Truncate(ForeignAmount * Rate * 100) / 100.
    /// </summary>
    public static class JournalEntryLineAmount
    {
        public static void Apply(JournalEntryLine line, Currency currency, decimal enteredAmount, decimal enteredRate)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            if (currency == Currency.colones)
            {
                line.Currency = Currency.colones;
                line.RateAmount = 1.00m;
                line.Amount = enteredAmount;
            }
            else
            {
                line.Currency = Currency.dolares;
                line.RateAmount = enteredRate;
                line.ForeignAmount = enteredAmount;
                line.Amount = TruncateToTwoDecimals(enteredAmount * enteredRate);
            }
        }

        public static decimal TruncateToTwoDecimals(decimal value)
        {
            return Math.Truncate(100 * value) / 100;
        }
    }
}
