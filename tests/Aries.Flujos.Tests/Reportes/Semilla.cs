using System.Collections.Generic;
using ClosedXML.Excel;
using System;
using System.Globalization;
using AriesContador.Core.Models.Utils;
using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Cuentas;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;

namespace Aries.Flujos.Tests.Reportes
{
    internal sealed class EriFila
    {
        public int Depth { get; set; }
        public string Name { get; set; }
        public decimal Saldo { get; set; }
        public bool IsTotal { get; set; }
        public bool IsPeriodResult { get; set; }
    }

    internal static class EriPlantilla
    {
        public static IReadOnlyList<EriFila> Leer(string path = null)
        {
            path = path ?? Excel.FixturePath(Excel.PlantillaEstadoResultado);
            var rows = new List<EriFila>();
            using (var workbook = new XLWorkbook(path))
            {
                var sheet = workbook.Worksheet(1);
                var last = sheet.LastRowUsed()?.RowNumber() ?? 0;
                for (var r = 5; r <= last; r++)
                {
                    var depth = 0;
                    var name = string.Empty;
                    for (var c = 1; c <= 5; c++)
                    {
                        var text = Excel.Normalize(Excel.CellText(sheet.Cell(r, c)));
                        if (text.Length == 0)
                            continue;
                        depth = c;
                        name = text;
                        break;
                    }

                    if (name.Length == 0)
                        continue;

                    rows.Add(new EriFila
                    {
                        Depth = depth,
                        Name = Excel.ClipName(name),
                        Saldo = Excel.CellAmount(sheet.Cell(r, 6)),
                        IsTotal = name.StartsWith("TOTAL "),
                        IsPeriodResult = name.IndexOf("UTILIDAD/PERDIDA", System.StringComparison.OrdinalIgnoreCase) >= 0
                    });
                }
            }

            return rows;
        }
    }

    internal sealed class AsientoLinea
    {
        public int Number { get; set; }
        public string AccountName { get; set; }
        public string Reference { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Currency Currency { get; set; }
        public decimal Rate { get; set; }
        public decimal ForeignAmount { get; set; }
    }

    internal sealed class AsientoFuente
    {
        public int Number { get; set; }
        public List<AsientoLinea> Lines { get; } = new List<AsientoLinea>();
    }

    internal static class AsientosFuente
    {
        public static IReadOnlyList<AsientoFuente> Leer(string path = null)
        {
            path = path ?? Excel.FixturePath(Excel.FuenteAsientos);
            var byNumber = new SortedDictionary<int, AsientoFuente>();
            using (var workbook = new XLWorkbook(path))
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

                    if (!byNumber.TryGetValue(number, out var entry))
                    {
                        entry = new AsientoFuente { Number = number };
                        byNumber[number] = entry;
                    }

                    var currencyText = Excel.Normalize(Excel.CellText(sheet.Cell(r, 9)));
                    var line = new AsientoLinea
                    {
                        Number = number,
                        AccountName = name,
                        Reference = Clip(Excel.CellText(sheet.Cell(r, 4)), 100),
                        Memo = Clip(Excel.CellText(sheet.Cell(r, 5)), 100),
                        Date = Excel.CellDate(sheet.Cell(r, 6), new DateTime(2020, 12, 31)),
                        Debit = Excel.CellAmount(sheet.Cell(r, 7)),
                        Credit = Excel.CellAmount(sheet.Cell(r, 8)),
                        Currency = currencyText.StartsWith("Dol", StringComparison.OrdinalIgnoreCase)
                            ? Currency.dolares
                            : Currency.colones,
                        Rate = Excel.CellAmount(sheet.Cell(r, 10)),
                        ForeignAmount = Excel.CellAmount(sheet.Cell(r, 11))
                    };
                    if (line.Rate == 0m)
                        line.Rate = 1m;
                    entry.Lines.Add(line);
                }
            }

            return new List<AsientoFuente>(byNumber.Values);
        }

        public static string ParentHint(string accountName)
        {
            var name = Excel.ClipName(accountName);
            if (name.StartsWith("BAC ", StringComparison.OrdinalIgnoreCase))
                return "BANCOS";
            if (name.StartsWith("CXC ", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("CLIENTES PENDIENTES", StringComparison.OrdinalIgnoreCase))
                return "CUENTAS POR COBRAR";
            if (name.StartsWith("IVA - APLICADO", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("RETENCION", StringComparison.OrdinalIgnoreCase)
                || name.Equals("IMPUESTO DE RENTA", StringComparison.OrdinalIgnoreCase))
                return "PASIVO CORTO PLAZO";
            if (name.StartsWith("PAGOS PARCIALES RENTA", StringComparison.OrdinalIgnoreCase))
                return "OTROS ACTIVOS CORRIENTES";
            if (name.StartsWith("DEPRECIACION ACUMULADA", StringComparison.OrdinalIgnoreCase))
                return "ACTIVO FIJO";
            if (name.StartsWith("UTILIDADES ACUMULADAS", StringComparison.OrdinalIgnoreCase))
                return "UTILIDADES ACUMULADAS";
            if (name.IndexOf("KARLA DELGADO", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("WARNER RODRIGUEZ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "COSTO VENTA";
            return null;
        }

        private static string Clip(string text, int max)
        {
            var value = text ?? string.Empty;
            return value.Length <= max ? value : value.Substring(0, max);
        }
    }

    internal sealed class Semilla
    {
        public Company Company { get; private set; }
        public PostingPeriod Periodo { get; private set; }
        public IReadOnlyList<EriFila> Plantilla { get; private set; }
        public IReadOnlyList<AsientoFuente> Asientos { get; private set; }

        public static async Task<Semilla> CrearAsync(DesktopServices services)
        {
            var financial = services.Financial;
            var company = FlujoBuilders.NuevaJuridica(
                HopeMuestra.CompaniaNombre,
                HopeMuestra.CompaniaMail,
                createdBy: FlujosDbFixture.AdminUserId);
            await services.Administration.CreateCompanyAsync(company);

            var ctx = new Semilla
            {
                Company = company,
                Plantilla = EriPlantilla.Leer(),
                Asientos = AsientosFuente.Leer()
            };

            var chart = (await financial.GetAccountsAsync(company.Code)).ToList();
            await ctx.CrearArbolDesdePlantillaAsync(financial, chart);
            chart = (await financial.GetAccountsAsync(company.Code)).ToList();
            await ctx.CrearCuentasDeAsientosFaltantesAsync(financial, chart);

            var periodo = new PostingPeriod
            {
                Date = HopeMuestra.MesContable,
                Closed = false,
                CompanyId = company.Code,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.CreatePostingPeriodAsync(periodo);
            ctx.Periodo = periodo;

            chart = (await financial.GetAccountsAsync(company.Code)).ToList();
            await ctx.CrearAsientosAsync(financial, chart);
            return ctx;
        }

        private async Task CrearArbolDesdePlantillaAsync(IFinancialService financial, List<Account> chart)
        {
            var stack = new Account[6];
            foreach (var fila in Plantilla)
            {
                if (fila.IsPeriodResult)
                    continue;

                if (fila.IsTotal)
                {
                    var titulo = fila.Name.Substring("TOTAL ".Length).Trim();
                    stack[1] = FindTitulo(chart, titulo);
                    for (var i = 2; i < stack.Length; i++)
                        stack[i] = null;
                    continue;
                }

                var parent = stack[fila.Depth - 1];
                if (parent == null)
                    throw new InvalidOperationException("No hay padre para " + fila.Name + " (columna " + fila.Depth + ")");

                var existing = FindHija(chart, parent, fila.Name);
                if (existing == null)
                {
                    existing = await CrearHijaAsync(financial, parent, fila.Name, Company.Code, editable: fila.Saldo == 0m);
                    chart.Add(existing);
                    var reloadedParent = await financial.FindAccountAsync(parent.Id);
                    if (reloadedParent != null)
                    {
                        parent.AccountType = reloadedParent.AccountType;
                        Replace(chart, reloadedParent);
                    }
                }

                stack[fila.Depth] = existing;
                for (var i = fila.Depth + 1; i < stack.Length; i++)
                    stack[i] = null;
            }
        }

        private async Task CrearCuentasDeAsientosFaltantesAsync(IFinancialService financial, List<Account> chart)
        {
            var names = Asientos.SelectMany(a => a.Lines).Select(l => l.AccountName).Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var name in names)
            {
                if (chart.Any(a => Excel.NamesEqual(a.Name, name)))
                    continue;

                var parentHint = AsientosFuente.ParentHint(name);
                if (string.IsNullOrEmpty(parentHint))
                    throw new InvalidOperationException("No hay padre para la cuenta de asiento " + name);

                var parent = FindPorNombre(chart, parentHint)
                    ?? throw new InvalidOperationException("No está la cuenta padre " + parentHint + " para " + name);
                var created = await CrearHijaAsync(financial, parent, name, Company.Code, editable: true);
                chart.Add(created);
            }
        }

        private async Task CrearAsientosAsync(IFinancialService financial, List<Account> chart)
        {
            foreach (var asiento in Asientos)
            {
                var number = await financial.CreateJournalEntryConsecutiveAsync(Periodo.Id);
                var entry = new JournalEntry
                {
                    Number = number,
                    PostingPeriodId = Periodo.Id,
                    JournalEntryStatus = JournalEntryStatus.Progress,
                    UpdatedBy = FlujosDbFixture.AdminUserId,
                    CreatedBy = FlujosDbFixture.AdminUserId,
                    Active = true
                };

                foreach (var linea in asiento.Lines)
                {
                    var account = chart.FirstOrDefault(a => Excel.NamesEqual(a.Name, linea.AccountName))
                        ?? throw new InvalidOperationException("No está la cuenta " + linea.AccountName);
                    var amount = linea.Debit != 0m ? linea.Debit : linea.Credit;
                    var lado = linea.Debit != 0m ? DebOrCred.Debito : DebOrCred.Credito;
                    var line = new JournalEntryLine
                    {
                        AccountId = account.Id,
                        AccountName = account.Name,
                        Reference = linea.Reference,
                        Memo = linea.Memo,
                        Date = linea.Date,
                        DebOrCred = lado,
                        Currency = linea.Currency,
                        RateAmount = linea.Rate,
                        ForeignAmount = linea.ForeignAmount,
                        Amount = amount,
                        CreatedBy = FlujosDbFixture.AdminUserId,
                        UpdatedBy = FlujosDbFixture.AdminUserId,
                        Active = true
                    };
                    entry.JournalEntryLines.Add(line);
                }

                entry.ApplyStatusFromBalance();
                await financial.CreateJournalEntryAsync(entry);
            }
        }

        private static async Task<Account> CrearHijaAsync(
            IFinancialService financial,
            Account parent,
            string name,
            string companyCode,
            bool editable)
        {
            var clipped = Excel.ClipName(name);
            await financial.EnsureAccountNameAsync(clipped);
            var child = new Account
            {
                Name = clipped,
                CompanyId = companyCode,
                FatherAccount = parent.Id,
                Memo = "HOPE C151",
                Editable = editable,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.CreateAccountAsync(child, parent);

            if (child.Id > 0)
            {
                var loaded = await financial.FindAccountAsync(child.Id);
                if (loaded != null && Excel.NamesEqual(loaded.Name, clipped))
                    return loaded;
            }

            var listed = await financial.GetAccountsAsync(companyCode);
            return listed.First(a => Excel.NamesEqual(a.Name, clipped) && a.FatherAccount == parent.Id);
        }

        private static Account FindTitulo(IEnumerable<Account> chart, string name)
        {
            return chart.First(a =>
                Excel.NamesEqual(a.Name, name) && a.AccountType == AccountType.Cuenta_Titulo);
        }

        private static Account FindHija(IEnumerable<Account> chart, Account parent, string name)
        {
            return chart.FirstOrDefault(a =>
                Excel.NamesEqual(a.Name, name) && a.FatherAccount == parent.Id);
        }

        private static Account FindPorNombre(IEnumerable<Account> chart, string name)
        {
            return chart.FirstOrDefault(a =>
                       Excel.NamesEqual(a.Name, name) && a.AccountType == AccountType.Cuenta_De_Mayor)
                   ?? chart.FirstOrDefault(a => Excel.NamesEqual(a.Name, name));
        }

        private static void Replace(List<Account> chart, Account updated)
        {
            var index = chart.FindIndex(a => a.Id == updated.Id);
            if (index >= 0)
                chart[index] = updated;
        }
    }
}
