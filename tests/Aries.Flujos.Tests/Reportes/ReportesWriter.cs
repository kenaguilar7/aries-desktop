using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using ClosedXML.Excel;

namespace Aries.Flujos.Tests.Reportes
{
    internal sealed class ReportesWriter
    {
        private readonly DesktopServices _services;
        private readonly Company _company;
        private readonly string _userName;

        public ReportesWriter(DesktopServices services, Company company, string userName)
        {
            _services = services;
            _company = company;
            _userName = userName;
        }

        public async Task SaveComprobacionAsync(string path, PostingPeriod from, PostingPeriod to)
        {
            var report = (await _services.Reports.BalanceComprobacionReportAsync(Param(from, to))).ToList();
            var treeDeep = report.Max(x => SplitPath(x.AccountPath).Length);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hoja1");
                for (var i = 0; i < report.Count; i++)
                {
                    var item = report[i];
                    var split = SplitPath(item.AccountPath);
                    worksheet.Cell(5 + i, split.Length).Value = split.LastOrDefault();
                    var amountStart = treeDeep + 1;
                    worksheet.Cell(5 + i, amountStart).Value = item.SaldoAnteriorDeb;
                    worksheet.Cell(5 + i, amountStart + 1).Value = item.SaldoAnteriorCred;
                    worksheet.Cell(5 + i, amountStart + 2).Value = item.SaldoMensualDeb;
                    worksheet.Cell(5 + i, amountStart + 3).Value = item.SaldoMensualCred;
                    worksheet.Cell(5 + i, amountStart + 4).Value = item.SaldoActualCuentaDeb;
                    worksheet.Cell(5 + i, amountStart + 5).Value = item.SaldoActualCuentaCred;
                }

                var range = worksheet.Range(worksheet.Cell(4, 1).Address, worksheet.Cell(4, treeDeep).Address);
                range.Value = "Cuentas";
                range.Merge();
                worksheet.Cell(4, treeDeep + 1).Value = "Saldo Anterior Débitos";
                worksheet.Cell(4, treeDeep + 2).Value = "Saldo Anterior Créditos";
                worksheet.Cell(4, treeDeep + 3).Value = "Saldo Mensual Débitos";
                worksheet.Cell(4, treeDeep + 4).Value = "Saldo Mensual Créditos";
                worksheet.Cell(4, treeDeep + 5).Value = "Saldo Cuenta Débitos";
                worksheet.Cell(4, treeDeep + 6).Value = "Saldo Cuenta Créditos";
                worksheet.Cell(1, 1).Value = _company.ToString();
                worksheet.Cell(2, 1).Value = string.Format(
                    CultureInfo.GetCultureInfo("es-CR"),
                    "Reporte de comprobación de {0} a {1}",
                    from.Date.ToString("Y", CultureInfo.GetCultureInfo("es-CR")),
                    to.Date.ToString("Y", CultureInfo.GetCultureInfo("es-CR")));
                worksheet.Cell(3, 1).Value = _userName;
                workbook.SaveAs(path);
            }
        }

        public async Task SaveAsientosAsync(string path, PostingPeriod from, PostingPeriod to)
        {
            var output = (await _services.Reports.JournalEntryReportAsync(Param(from, to))).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hoja1");
                worksheet.Cell(4, 1).InsertTable(output);
                if (_company.CurrencyType == CurrencyTypeCompany.Solo_Colones)
                {
                    worksheet.Column(9).Delete();
                    worksheet.Column(10).Delete();
                    worksheet.Column(11).Delete();
                }

                worksheet.Tables.First().Theme = XLTableTheme.None;
                worksheet.Tables.First().ShowAutoFilter = false;
                worksheet.Cell(1, 1).Value = _company.ToString();
                worksheet.Cell(2, 1).Value = "Reporte Asientos";
                worksheet.Cell(3, 1).Value = _userName;
                workbook.SaveAs(path);
            }
        }

        public async Task SaveAuxiliaresAsync(string path, PostingPeriod from, PostingPeriod to)
        {
            var accounts = (await _services.Financial.GetAccountsAsync(_company.Code)).ToList();
            EnsureNature(accounts);
            var periods = (await _services.Financial.GetPostingPeriodsAsync(_company.Code))
                .Where(p => p.Date >= from.Date && p.Date <= to.Date)
                .OrderBy(p => p.Date)
                .ToList();

            var byMonth = new List<Tuple<DateTime, List<Account>>>();
            var union = new List<Account>();
            foreach (var period in periods)
            {
                var clone = accounts.Select(CloneAccount).ToList();
                await _services.Financial.FillAccountsWithBalancesAsync(clone, period.Date, period.Date);
                EnsureNature(clone);
                var filtered = AccountRules.RemoveAccountsWithoutBalances(clone).ToList();
                byMonth.Add(Tuple.Create(period.Date, filtered));
                foreach (var account in filtered)
                {
                    if (union.All(a => a.Id != account.Id))
                        union.Add(CloneAccount(account));
                }
            }

            var ordered = OrderTree(union);
            var nameWidth = Math.Max(1, ordered.Max(a => NombreParaExcel(a, ordered).Length));

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                worksheet.Cell(1, 1).Value = _company + " " + _company.Code;
                worksheet.Cell(2, 1).Value = "Balance Auxiliares";
                worksheet.Cell(3, 1).Value = _userName;
                worksheet.Cell(4, 1).Value = "Cuentas";
                worksheet.Range(4, 1, 6, nameWidth).Merge();

                for (var i = 0; i < ordered.Count; i++)
                {
                    var nombre = NombreParaExcel(ordered[i], ordered);
                    worksheet.Cell(7 + i, nombre.Length).Value = nombre[nombre.Length - 1];
                }

                var amountCol = nameWidth + 1;
                foreach (var month in byMonth)
                {
                    worksheet.Cell(5, amountCol).Value = MesEs(month.Item1);
                    worksheet.Range(5, amountCol, 5, amountCol + 1).Merge();
                    worksheet.Cell(6, amountCol).Value = "COL";
                    worksheet.Cell(6, amountCol + 1).Value = "USD";
                    for (var i = 0; i < ordered.Count; i++)
                    {
                        var current = month.Item2.Find(a => a.Id == ordered[i].Id) ?? ordered[i];
                        worksheet.Cell(7 + i, amountCol).Value = current.MontlyBalance;
                        worksheet.Cell(7 + i, amountCol + 1).Value = current.MontlyBalanceForeign;
                    }

                    amountCol += 2;
                }

                workbook.SaveAs(path);
            }
        }

        public async Task SaveBalanceSituacionAsync(string path, PostingPeriod from, PostingPeriod to)
        {
            var accounts = (await _services.Financial.GetAccountsAsync(_company.Code)).ToList();
            EnsureNature(accounts);
            await _services.Financial.FillAccountsWithBalancesAsync(accounts, from.Date, to.Date);
            EnsureNature(accounts);
            var rows = BuildSituacion(accounts);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte");
                worksheet.Cell(1, 1).Value = _company.ToString();
                worksheet.Cell(2, 1).Value = "BALANCE DE SITUACION AL MES " + MesEs(to.Date).ToUpperInvariant();
                worksheet.Cell(3, 1).Value = "EMITIDO POR " + _userName + " ";
                for (var r = 0; r < rows.Count; r++)
                {
                    for (var c = 0; c < rows[r].Length; c++)
                    {
                        var value = rows[r][c];
                        if (value == null || value.Equals(""))
                            continue;
                        worksheet.Cell(5 + r, 1 + c).Value = value;
                    }
                }

                workbook.SaveAs(path);
            }
        }

        public async Task SaveMovimientosAsync(string path, string accountName)
        {
            var accounts = (await _services.Financial.GetAccountsAsync(_company.Code)).ToList();
            var account = accounts.FirstOrDefault(a => Excel.NamesEqual(a.Name, accountName))
                          ?? throw new InvalidOperationException("No está la cuenta " + accountName);
            var auxiliar = account.AccountType == AccountType.Cuenta_Auxiliar;
            var table = await _services.Reports.GetAccountMovementReportAsync(account.Id, auxiliar);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                worksheet.Cell(1, 1).Value = _company + " " + _company.Code;
                worksheet.Cell(2, 1).Value = "Reporte movimientos de cuenta";
                worksheet.Cell(3, 1).Value = "Cuenta: " + account.Name;
                worksheet.Cell(4, 1).Value = _userName;
                worksheet.Cell(5, 1).Value = "Nombre Cuenta";
                worksheet.Cell(5, 2).Value = "Tipo Movimiento";
                worksheet.Cell(5, 3).Value = "Detalle";
                worksheet.Cell(5, 4).Value = "Referencia";
                worksheet.Cell(5, 5).Value = "Fecha Factura";
                worksheet.Cell(5, 6).Value = "Mes Contable";
                worksheet.Cell(5, 7).Value = "Número Asiento";
                worksheet.Cell(5, 8).Value = "Debito";
                worksheet.Cell(5, 9).Value = "Credito";
                worksheet.Cell(5, 10).Value = "Saldo Actual";
                worksheet.Cell(5, 11).Value = "Tipo Cambio";
                worksheet.Cell(5, 12).Value = "Monto Dolares";

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    var vs = table.Rows[i].ItemArray;
                    for (var c = 0; c < Math.Min(12, vs.Length); c++)
                    {
                        var value = vs[c];
                        if (value == null || value == DBNull.Value)
                            continue;
                        var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                        worksheet.Cell(6 + i, 1 + c).Value = text.Replace(",", string.Empty);
                    }
                }

                workbook.SaveAs(path);
            }
        }

        public async Task SaveMaestroAsync(string path, PostingPeriod from, PostingPeriod to)
        {
            var accounts = (await _services.Financial.GetAccountsAsync(_company.Code)).ToList();
            EnsureNature(accounts);
            await _services.Financial.FillAccountsWithBalancesAsync(accounts, from.Date, to.Date);
            EnsureNature(accounts);
            var filtered = AccountRules.RemoveAccountsWithoutBalances(accounts).ToList();
            var nameWidth = Math.Max(1, filtered.Max(a => NombreParaExcel(a, filtered).Length));

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                worksheet.Cell(1, 1).Value = _company.ToString();
                worksheet.Cell(2, 1).Value = "Maestro de Cuentas en Colones,  a " + MesEs(to.Date);
                worksheet.Cell(3, 1).Value = "usuario " + _userName;
                worksheet.Cell(4, 1).Value = "Cuentas";
                worksheet.Range(4, 1, 5, nameWidth).Merge();
                worksheet.Cell(4, nameWidth + 1).Value = "Saldo Anterior";
                worksheet.Cell(4, nameWidth + 2).Value = "Debitos";
                worksheet.Cell(4, nameWidth + 3).Value = "Creditos";
                worksheet.Cell(4, nameWidth + 4).Value = "Saldo Actual";

                for (var i = 0; i < filtered.Count; i++)
                {
                    var account = filtered[i];
                    var nombre = NombreParaExcel(account, filtered);
                    worksheet.Cell(6 + i, nombre.Length).Value = nombre[nombre.Length - 1];
                    worksheet.Cell(6 + i, nameWidth + 1).Value = account.PriorBalance;
                    worksheet.Cell(6 + i, nameWidth + 2).Value = account.DebitBalance;
                    worksheet.Cell(6 + i, nameWidth + 3).Value = account.CreditBalance;
                    worksheet.Cell(6 + i, nameWidth + 4).Value = account.CurrentBalance;
                }

                workbook.SaveAs(path);
            }
        }

        private BasicReportParam Param(PostingPeriod from, PostingPeriod to)
        {
            return new BasicReportParam
            {
                CompanyId = _company.Code,
                FirstDate = YyyyMm(from.Date),
                EndDate = YyyyMm(to.Date)
            };
        }

        private static string YyyyMm(DateTime date)
        {
            return date.Year.ToString(CultureInfo.InvariantCulture) + date.Month.ToString("00", CultureInfo.InvariantCulture);
        }

        private static string[] SplitPath(string path)
        {
            return (path ?? string.Empty).Split(new[] { '¡' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string MesEs(DateTime date)
        {
            var info = new CultureInfo("es-ES", false).DateTimeFormat;
            return info.GetMonthName(date.Month) + " " + date.Year;
        }

        private static void EnsureNature(IEnumerable<Account> accounts)
        {
            foreach (var account in accounts)
            {
                if (account.DebOrCred == DebOrCred.Debito || account.DebOrCred == DebOrCred.Credito)
                    continue;
                account.DebOrCred = DebitNature(account.AccountTag) ? DebOrCred.Debito : DebOrCred.Credito;
            }
        }

        private static bool DebitNature(AccountTag tag)
        {
            return tag == AccountTag.Activo || tag == AccountTag.CostoVenta || tag == AccountTag.Egreso;
        }

        private static Account CloneAccount(Account source)
        {
            return new Account
            {
                Id = source.Id,
                Name = source.Name,
                CompanyId = source.CompanyId,
                FatherAccount = source.FatherAccount,
                AccountType = source.AccountType,
                AccountTag = source.AccountTag,
                Editable = source.Editable,
                Active = source.Active,
                DebOrCred = source.DebOrCred,
                PathDirection = source.PathDirection,
                Memo = source.Memo,
                PriorBalance = source.PriorBalance,
                PriorBalanceForeign = source.PriorBalanceForeign,
                DebitBalance = source.DebitBalance,
                CreditBalance = source.CreditBalance,
                DebitBalanceForeign = source.DebitBalanceForeign,
                CreditBalanceForeign = source.CreditBalanceForeign
            };
        }

        private static string[] NombreParaExcel(Account account, IList<Account> all)
        {
            var depth = 1;
            var dummy = account;
            while (dummy != null)
            {
                dummy = all.FirstOrDefault(a => a.Id == dummy.FatherAccount);
                if (dummy != null)
                    depth++;
            }

            var names = new string[depth];
            for (var i = 0; i < names.Length; i++)
                names[i] = string.Empty;
            names[names.Length - 1] = account.Name;
            return names;
        }

        private static List<Account> OrderTree(List<Account> list)
        {
            var ordered = new List<Account>();
            foreach (var title in list.Where(a => a.AccountType == AccountType.Cuenta_Titulo))
                AddNode(title);
            return ordered;

            void AddNode(Account account)
            {
                ordered.Add(account);
                foreach (var child in list.Where(a => a.FatherAccount == account.Id))
                    AddNode(child);
            }
        }

        private static List<object[]> BuildSituacion(List<Account> accounts)
        {
            var perdida = accounts.Where(IsResultado).ToList();
            var situacion = accounts.Where(a => !IsResultado(a)).ToList();
            var depth = accounts.Max(a => NombreParaExcel(a, accounts).Length);
            var columnCount = depth * 2;
            var rows = new List<object[]>();
            var utilidad = TotalPerdida(perdida);

            foreach (var cuenta in situacion)
            {
                if (cuenta.AccountType != AccountType.Cuenta_Titulo)
                    continue;

                foreach (var aux in accounts)
                {
                    if (aux.AccountType == AccountType.Cuenta_Titulo)
                        continue;
                    if (aux.AccountTag != cuenta.AccountTag)
                        continue;

                    var name = NombreParaExcel(aux, accounts);
                    var row = NewRow(columnCount);
                    row[name.Length - 1] = name[name.Length - 1];
                    row[columnCount - name.Length] = aux.CurrentBalance;
                    rows.Add(row);
                }

                if (cuenta.AccountTag == AccountTag.Patrimonio)
                {
                    var utilidadRow = NewRow(columnCount);
                    utilidadRow[0] = "UTILIDAD/PERDIDA PERIODO";
                    utilidadRow[columnCount - 1] = utilidad;
                    rows.Add(utilidadRow);

                    var nameTotal = NombreParaExcel(cuenta, accounts);
                    var totalPat = NewRow(columnCount);
                    totalPat[nameTotal.Length - 1] = "TOTAL " + nameTotal[nameTotal.Length - 1];
                    totalPat[columnCount - nameTotal.Length] = utilidad + cuenta.CurrentBalance;
                    rows.Add(totalPat);
                }
                else
                {
                    var nameTotal = NombreParaExcel(cuenta, accounts);
                    var total = NewRow(columnCount);
                    total[nameTotal.Length - 1] = "TOTAL " + nameTotal[nameTotal.Length - 1];
                    total[columnCount - nameTotal.Length] = cuenta.CurrentBalance;
                    rows.Add(total);
                }
            }

            var pasivo = accounts.Find(a => a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == AccountTag.Pasivo);
            var patrimonio = accounts.Find(a => a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == AccountTag.Patrimonio);
            var totalPyP = NewRow(columnCount);
            totalPyP[0] = "TOTAL PASIVO Y PATRIMONIO";
            totalPyP[columnCount - 1] = utilidad + (pasivo?.CurrentBalance ?? 0m) + (patrimonio?.CurrentBalance ?? 0m);
            rows.Add(totalPyP);
            return rows;
        }

        private static bool IsResultado(Account account)
        {
            return account.AccountTag == AccountTag.Ingreso
                   || account.AccountTag == AccountTag.Egreso
                   || account.AccountTag == AccountTag.CostoVenta;
        }

        private static decimal TotalPerdida(List<Account> perdida)
        {
            var ingreso = perdida.Find(a => a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == AccountTag.Ingreso);
            var egreso = perdida.Find(a => a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == AccountTag.Egreso);
            var costo = perdida.Find(a => a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == AccountTag.CostoVenta);
            return (ingreso?.CurrentBalance ?? 0m) - (costo?.CurrentBalance ?? 0m) - (egreso?.CurrentBalance ?? 0m);
        }

        private static object[] NewRow(int columns)
        {
            var row = new object[columns];
            for (var i = 0; i < columns; i++)
                row[i] = "";
            return row;
        }
    }
}
