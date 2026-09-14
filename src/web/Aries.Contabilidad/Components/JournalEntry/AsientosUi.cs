using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;

namespace Aries.Contabilidad.Components.JournalEntry
{
    public static class AsientosUi
    {
        public static string PeriodKey(PostingPeriod period) =>
            $"{period.Date.Year}{period.Date.Month:D2}";

        public static IEnumerable<PostingPeriod> EndOptions(IEnumerable<PostingPeriod> periods, int startId)
        {
            var start = periods.FirstOrDefault(p => p.Id == startId);
            return periods.Where(p => start == null || p.Date >= start.Date);
        }

        public static string Colones(decimal amount) => $"₡{amount:N2}";

        public static string Dollars(decimal amount) => $"${amount:N2}";

        public static string CurrencyLabel(Currency currency) =>
            currency == Currency.dolares ? "USD" : "CRC";

        public static bool ShowForeignColumns(CurrencyTypeCompany moneyType) =>
            moneyType != CurrencyTypeCompany.Solo_Colones;

        public static bool CurrencyLocked(CurrencyTypeCompany moneyType) =>
            moneyType != CurrencyTypeCompany.Dolares_y_Colones;

        public static Currency DefaultCurrency(CurrencyTypeCompany moneyType) =>
            moneyType == CurrencyTypeCompany.Solo_Dolares ? Currency.dolares : Currency.colones;

        public static string CsvEscape(string? value)
        {
            var text = value ?? string.Empty;
            if (text.Contains('"') || text.Contains(',') || text.Contains('\n') || text.Contains('\r'))
                return $"\"{text.Replace("\"", "\"\"")}\"";
            return text;
        }
    }
}
