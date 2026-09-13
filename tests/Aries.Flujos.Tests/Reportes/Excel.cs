using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xunit;

namespace Aries.Flujos.Tests.Reportes
{
    internal static class Excel
    {
        public const int MaxAccountNameLength = 50;
        public const string PlantillaComprobacion = "01-comprobacion.xlsx";
        public const string PlantillaEstadoResultado = "02-estado-resultado.xlsx";
        public const string FuenteAsientos = "03-asientos.xlsx";
        public const string PlantillaAuxiliares = "04-auxiliares.xlsx";
        public const string PlantillaBalanceSituacion = "05-balance-situacion.xlsx";
        public const string PlantillaMovimientosAuxiliar = "06-movimientos-auxiliar.xlsx";
        public const string PlantillaMovimientosTitulo = "06b-movimientos-titulo.xlsx";
        public const string PlantillaMaestro = "07-maestro-cuentas.xlsx";
        public const string CuentaMovimientosTitulo = "INGRESOS MEDICOS-2024";

        public static string FixturePath(string fileName)
        {
            var fromOutput = Path.Combine(AppContext.BaseDirectory, "Reportes", "Fixtures", fileName);
            if (File.Exists(fromOutput))
                return fromOutput;

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "tests", "Aries.Flujos.Tests", "Reportes", "Fixtures", fileName);
                if (File.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }

            throw new FileNotFoundException("No está el fixture " + fileName);
        }

        public static XLWorkbook OpenWorkbook(string path)
        {
            try
            {
                return new XLWorkbook(path);
            }
            catch (ArgumentException)
            {
                var copy = Path.Combine(Path.GetTempPath(), "aries-xlsx-" + Guid.NewGuid().ToString("N") + ".xlsx");
                File.Copy(path, copy, true);
                StripTables(copy);
                return new XLWorkbook(copy);
            }
        }

        private static void StripTables(string path)
        {
            using (var zip = ZipFile.Open(path, ZipArchiveMode.Update))
            {
                foreach (var entry in zip.Entries.Where(e =>
                    e.FullName.StartsWith("xl/tables/", StringComparison.OrdinalIgnoreCase)).ToList())
                    entry.Delete();

                foreach (var sheet in zip.Entries.Where(e =>
                    e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase)
                    && e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    var fullName = sheet.FullName;
                    string xml;
                    using (var stream = sheet.Open())
                    using (var reader = new StreamReader(stream))
                        xml = reader.ReadToEnd();

                    var cleaned = System.Text.RegularExpressions.Regex.Replace(
                        xml,
                        @"<tableParts\b[^>]*>.*?</tableParts>",
                        string.Empty,
                        System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (cleaned == xml)
                        continue;

                    sheet.Delete();
                    var created = zip.CreateEntry(fullName);
                    using (var stream = created.Open())
                    using (var writer = new StreamWriter(stream))
                        writer.Write(cleaned);
                }
            }
        }

        public static string ClipName(string name)
        {
            var text = Normalize(name);
            if (text.Length <= MaxAccountNameLength)
                return text;
            return text.Substring(0, MaxAccountNameLength).Trim();
        }

        public static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;
            return string.Join(" ", name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static bool NamesEqual(string left, string right)
        {
            return string.Equals(ClipName(left), ClipName(right), StringComparison.OrdinalIgnoreCase);
        }

        public static string CellText(IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return string.Empty;
            return Convert.ToString(cell.Value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static decimal CellAmount(IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return 0m;
            try
            {
                if (cell.DataType == XLDataType.Number)
                    return Convert.ToDecimal(cell.Value, CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            var text = CellText(cell).Replace("₡", "").Replace("$", "").Trim();
            if (text.Length == 0)
                return 0m;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("es-CR"), out var cr))
                return cr;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var inv))
                return inv;
            return 0m;
        }

        public static DateTime CellDate(IXLCell cell, DateTime fallback)
        {
            if (cell == null || cell.IsEmpty())
                return fallback;
            try
            {
                if (cell.DataType == XLDataType.DateTime)
                    return cell.GetDateTime();
            }
            catch
            {
            }

            if (cell.Value is DateTime date)
                return date;
            if (double.TryParse(Convert.ToString(cell.Value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var oa))
                return DateTime.FromOADate(oa);
            return fallback;
        }
    }

    internal sealed class ExcelSheet
    {
        private readonly Dictionary<(int Row, int Col), string> _cells;
        public int LastRow { get; }
        public int LastCol { get; }

        private ExcelSheet(Dictionary<(int, int), string> cells)
        {
            _cells = cells;
            LastRow = cells.Count == 0 ? 0 : cells.Keys.Max(k => k.Item1);
            LastCol = cells.Count == 0 ? 0 : cells.Keys.Max(k => k.Item2);
        }

        public string Text(int row, int col)
        {
            return _cells.TryGetValue((row, col), out var value) ? value ?? string.Empty : string.Empty;
        }

        public decimal Amount(int row, int col)
        {
            var text = Text(row, col).Replace("₡", "").Replace("$", "").Trim();
            if (text.Length == 0)
                return 0m;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("es-CR"), out var cr))
                return cr;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var inv))
                return inv;
            return 0m;
        }

        public bool Has(int row, int col)
        {
            return _cells.ContainsKey((row, col));
        }

        public static ExcelSheet Load(string path)
        {
            using (var zip = ZipFile.OpenRead(path))
            {
                var shared = ReadSharedStrings(zip);
                var sheetEntry = zip.Entries.First(e =>
                    e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase)
                    && e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
                using (var stream = sheetEntry.Open())
                {
                    var doc = XDocument.Load(stream);
                    XNamespace ns = doc.Root.Name.Namespace;
                    var cells = new Dictionary<(int, int), string>();
                    foreach (var cell in doc.Descendants(ns + "c"))
                    {
                        var reference = (string)cell.Attribute("r");
                        if (string.IsNullOrEmpty(reference) || !TryParseRef(reference, out var row, out var col))
                            continue;
                        var type = (string)cell.Attribute("t");
                        var value = cell.Element(ns + "v")?.Value;
                        var inline = cell.Element(ns + "is")?.Value;
                        string text;
                        if (string.Equals(type, "s", StringComparison.OrdinalIgnoreCase)
                            && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx)
                            && idx >= 0 && idx < shared.Count)
                            text = shared[idx];
                        else if (string.Equals(type, "inlineStr", StringComparison.OrdinalIgnoreCase))
                            text = inline ?? string.Empty;
                        else
                            text = value ?? inline ?? string.Empty;
                        cells[(row, col)] = text;
                    }

                    return new ExcelSheet(cells);
                }
            }
        }

        private static List<string> ReadSharedStrings(ZipArchive zip)
        {
            var values = new List<string>();
            var entry = zip.Entries.FirstOrDefault(e =>
                e.FullName.Equals("xl/sharedStrings.xml", StringComparison.OrdinalIgnoreCase));
            if (entry == null)
                return values;
            using (var stream = entry.Open())
            {
                var doc = XDocument.Load(stream);
                XNamespace ns = doc.Root.Name.Namespace;
                foreach (var item in doc.Descendants(ns + "si"))
                {
                    var parts = item.Descendants(ns + "t").Select(t => t.Value);
                    values.Add(string.Concat(parts));
                }
            }

            return values;
        }

        private static bool TryParseRef(string reference, out int row, out int col)
        {
            row = 0;
            col = 0;
            var match = Regex.Match(reference, @"^([A-Z]+)(\d+)$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return false;
            col = ColumnIndex(match.Groups[1].Value);
            row = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            return col > 0 && row > 0;
        }

        private static int ColumnIndex(string letters)
        {
            var col = 0;
            foreach (var ch in letters.ToUpperInvariant())
                col = col * 26 + (ch - 'A' + 1);
            return col;
        }
    }

    internal sealed class NamedAmounts
    {
        public string Name { get; set; }
        public decimal[] Amounts { get; set; }
        public int Row { get; set; }

        public bool Significant
        {
            get { return Amounts != null && Amounts.Any(a => Math.Abs(a) >= 0.01m); }
        }
    }

    internal sealed class AsientoFila
    {
        public int Number { get; set; }
        public string AccountName { get; set; }
        public string Reference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Rate { get; set; }
        public decimal ForeignAmount { get; set; }
    }

    internal sealed class MovimientoFila
    {
        public string AccountName { get; set; }
        public string Memo { get; set; }
        public int JournalNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Saldo { get; set; }
    }

    internal static class ExcelCompare
    {
        public const decimal Tolerance = 0.02m;

        public static List<NamedAmounts> LeerCuentas(string path, int startRow, int amountCount)
        {
            var rows = new List<NamedAmounts>();
            var sheet = ExcelSheet.Load(path);
            var lastRow = sheet.LastRow;
            var lastCol = sheet.LastCol;
            var nameLastCol = amountCount <= 0 ? lastCol : Math.Max(1, lastCol - amountCount);
            for (var r = startRow; r <= lastRow; r++)
            {
                var name = string.Empty;
                var nameColLimit = amountCount <= 0 ? lastCol : nameLastCol;
                for (var c = 1; c <= nameColLimit; c++)
                {
                    var text = Excel.Normalize(sheet.Text(r, c));
                    if (text.Length == 0 || LooksLikeHeader(text))
                        continue;
                    name = text;
                    break;
                }

                if (name.Length == 0)
                    continue;

                decimal[] amounts;
                if (amountCount <= 0)
                {
                    var last = 0m;
                    var found = false;
                    for (var c = lastCol; c >= 1; c--)
                    {
                        if (!sheet.Has(r, c))
                            continue;
                        last = sheet.Amount(r, c);
                        found = true;
                        break;
                    }

                    if (!found)
                        continue;
                    amounts = new[] { last };
                }
                else
                {
                    amounts = new decimal[amountCount];
                    for (var i = 0; i < amountCount; i++)
                        amounts[i] = sheet.Amount(r, nameLastCol + 1 + i);
                }

                rows.Add(new NamedAmounts
                {
                    Name = Excel.ClipName(name),
                    Amounts = amounts,
                    Row = r
                });
            }

            return rows;
        }

        public static List<AsientoFila> LeerAsientos(string path)
        {
            var rows = new List<AsientoFila>();
            using (var workbook = Excel.OpenWorkbook(path))
            {
                var sheet = workbook.Worksheet(1);
                var last = sheet.LastRowUsed()?.RowNumber() ?? 0;
                for (var r = 5; r <= last; r++)
                {
                    var name = Excel.ClipName(Excel.CellText(sheet.Cell(r, 3)));
                    if (name.Length == 0)
                        continue;
                    var numberText = Excel.CellText(sheet.Cell(r, 2));
                    if (!int.TryParse(numberText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
                        number = (int)Excel.CellAmount(sheet.Cell(r, 2));
                    rows.Add(new AsientoFila
                    {
                        Number = number,
                        AccountName = name,
                        Reference = Excel.Normalize(Excel.CellText(sheet.Cell(r, 4))),
                        Debit = Excel.CellAmount(sheet.Cell(r, 7)),
                        Credit = Excel.CellAmount(sheet.Cell(r, 8)),
                        Rate = Excel.CellAmount(sheet.Cell(r, 10)),
                        ForeignAmount = Excel.CellAmount(sheet.Cell(r, 11))
                    });
                }
            }

            return rows;
        }

        public static List<MovimientoFila> LeerMovimientos(string path)
        {
            var rows = new List<MovimientoFila>();
            using (var workbook = Excel.OpenWorkbook(path))
            {
                var sheet = workbook.Worksheet(1);
                var last = sheet.LastRowUsed()?.RowNumber() ?? 0;
                for (var r = 6; r <= last; r++)
                {
                    var name = Excel.ClipName(Excel.CellText(sheet.Cell(r, 1)));
                    if (name.Length == 0)
                        continue;
                    var numberText = Excel.CellText(sheet.Cell(r, 7));
                    if (!int.TryParse(numberText.Replace(",", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
                        number = (int)Excel.CellAmount(sheet.Cell(r, 7));
                    rows.Add(new MovimientoFila
                    {
                        AccountName = name,
                        Memo = Excel.Normalize(Excel.CellText(sheet.Cell(r, 3))),
                        JournalNumber = number,
                        Debit = Excel.CellAmount(sheet.Cell(r, 8)),
                        Credit = Excel.CellAmount(sheet.Cell(r, 9)),
                        Saldo = Excel.CellAmount(sheet.Cell(r, 10))
                    });
                }
            }

            return rows;
        }

        public static List<string> DiffCuentas(IReadOnlyList<NamedAmounts> expected, IReadOnlyList<NamedAmounts> generated)
        {
            var errors = new List<string>();
            foreach (var row in expected.Where(r => r.Significant || IsTotalName(r.Name)))
            {
                var matches = generated.Where(g => Excel.NamesEqual(g.Name, row.Name)).ToList();
                if (matches.Count == 0)
                {
                    errors.Add("No salió: " + row.Name + " " + Format(row.Amounts));
                    continue;
                }

                if (!matches.Any(g => AmountsClose(g.Amounts, row.Amounts)))
                    errors.Add(row.Name + " esperado " + Format(row.Amounts) + " generado " + Format(matches[0].Amounts));
            }

            var extra = generated.Where(g => g.Significant
                && expected.All(e => !Excel.NamesEqual(e.Name, g.Name))).Take(12).ToList();
            foreach (var row in extra)
                errors.Add("De más: " + row.Name + " " + Format(row.Amounts));
            return errors;
        }

        public static List<string> DiffAsientos(IReadOnlyList<AsientoFila> expected, IReadOnlyList<AsientoFila> generated)
        {
            var errors = new List<string>();
            if (expected.Count != generated.Count)
                errors.Add("Filas plantilla " + expected.Count + ", generado " + generated.Count);

            var remaining = generated.ToList();
            foreach (var row in expected)
            {
                var idx = remaining.FindIndex(g =>
                    g.Number == row.Number
                    && Excel.NamesEqual(g.AccountName, row.AccountName)
                    && Math.Abs(g.Debit - row.Debit) < Tolerance
                    && Math.Abs(g.Credit - row.Credit) < Tolerance);
                if (idx < 0)
                {
                    errors.Add("Falta asiento " + row.Number + " " + row.AccountName
                               + " deb=" + row.Debit.ToString(CultureInfo.InvariantCulture)
                               + " cred=" + row.Credit.ToString(CultureInfo.InvariantCulture));
                    continue;
                }

                remaining.RemoveAt(idx);
            }

            foreach (var row in remaining.Take(12))
                errors.Add("De más asiento " + row.Number + " " + row.AccountName);
            return errors;
        }

        public static List<string> DiffMovimientos(IReadOnlyList<MovimientoFila> expected, IReadOnlyList<MovimientoFila> generated)
        {
            var errors = new List<string>();
            if (expected.Count != generated.Count)
                errors.Add("Filas plantilla " + expected.Count + ", generado " + generated.Count);

            var remaining = generated.ToList();
            var missing = 0;
            foreach (var row in expected)
            {
                var idx = remaining.FindIndex(g =>
                    Excel.NamesEqual(g.AccountName, row.AccountName)
                    && string.Equals(g.Memo, row.Memo, StringComparison.OrdinalIgnoreCase)
                    && Math.Abs(g.Debit - row.Debit) < Tolerance
                    && Math.Abs(g.Credit - row.Credit) < Tolerance);
                if (idx < 0)
                {
                    missing++;
                    if (missing <= 12)
                        errors.Add("Falta movimiento " + row.AccountName + " " + row.Memo
                                   + " deb=" + row.Debit.ToString(CultureInfo.InvariantCulture)
                                   + " cred=" + row.Credit.ToString(CultureInfo.InvariantCulture));
                    continue;
                }

                remaining.RemoveAt(idx);
            }

            if (missing > 12)
                errors.Add("... +" + (missing - 12) + " movimientos de la plantilla que no salieron");
            foreach (var row in remaining.Take(8))
                errors.Add("De más movimiento " + row.AccountName + " " + row.Memo);
            return errors;
        }

        public static void AssertNoDiff(IReadOnlyList<string> errors, string titulo)
        {
            Xunit.Assert.True(
                errors.Count == 0,
                titulo + Environment.NewLine + string.Join(Environment.NewLine, errors.Take(60))
                + (errors.Count > 60 ? Environment.NewLine + "... +" + (errors.Count - 60) + " más" : string.Empty));
        }

        private static bool AmountsClose(decimal[] left, decimal[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;
            for (var i = 0; i < left.Length; i++)
            {
                if (Math.Abs(left[i] - right[i]) >= Tolerance)
                    return false;
            }

            return true;
        }

        private static string Format(decimal[] amounts)
        {
            return "[" + string.Join(", ", amounts.Select(a => a.ToString("0.##", CultureInfo.InvariantCulture))) + "]";
        }

        private static bool IsTotalName(string name)
        {
            return name.StartsWith("TOTAL ", StringComparison.OrdinalIgnoreCase)
                   || name.IndexOf("UTILIDAD/PERDIDA", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool LooksLikeHeader(string text)
        {
            if (text.Equals("Cuentas", StringComparison.OrdinalIgnoreCase)
                || text.Equals("COL", StringComparison.OrdinalIgnoreCase)
                || text.Equals("USD", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("Saldo ", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("Nombre ", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Debitos", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Creditos", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Debito", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Credito", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
