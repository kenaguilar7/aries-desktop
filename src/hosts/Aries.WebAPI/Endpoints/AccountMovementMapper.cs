using System.Data;
using System.Globalization;
using AriesContador.Core.Models.Reports;

namespace Aries.WebAPI.Endpoints
{
    internal static class AccountMovementMapper
    {
        public static List<AccountMovementRow> FromTable(DataTable table)
        {
            var rows = new List<AccountMovementRow>();
            if (table == null)
                return rows;

            foreach (DataRow row in table.Rows)
            {
                rows.Add(new AccountMovementRow
                {
                    AccountName = Text(row, "Nombre"),
                    MovementType = FirstText(row, "Tipo Moviento", "Tipo Movimiento"),
                    Memo = Text(row, "Detalle"),
                    Reference = Text(row, "Referencia"),
                    DocumentDate = Date(row, "Fecha Documento"),
                    PostingPeriodName = Text(row, "Mes Contable"),
                    JournalEntryNumber = Int(row, "Numero de Asiento"),
                    DebitAmount = Amount(row, "Debito"),
                    CreditAmount = Amount(row, "Credito"),
                    CurrentBalance = Amount(row, "Saldo Actual"),
                    RateAmount = Amount(row, "Tipo Cambio"),
                    ForeignAmount = Amount(row, "Monto Dolares"),
                    UserName = Text(row, "Usuario registro"),
                    RegisteredAt = Date(row, "Fecha de Registro")
                });
            }

            return rows;
        }

        private static string FirstText(DataRow row, params string[] names)
        {
            foreach (var name in names)
            {
                var value = Text(row, name);
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return string.Empty;
        }

        private static string Text(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return string.Empty;
            return Convert.ToString(row[column]) ?? string.Empty;
        }

        private static DateTime Date(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return DateTime.MinValue;
            return Convert.ToDateTime(row[column], CultureInfo.InvariantCulture);
        }

        private static int Int(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return 0;
            var text = Convert.ToString(row[column]);
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                return value;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out value))
                return value;
            return Convert.ToInt32(row[column]);
        }

        private static decimal Amount(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return 0m;

            var raw = row[column];
            if (raw is decimal decimalValue)
                return decimalValue;
            if (raw is double doubleValue)
                return (decimal)doubleValue;

            var text = Convert.ToString(raw);
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var amount))
                return amount;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                return amount;
            return 0m;
        }
    }
}
