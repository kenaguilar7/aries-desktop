using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.FechaTransacciones;
using Aries.Reporting.Entidades.Usuarios;
using Aries.Reporting.Enumeradores;
using Aries.Reporting.Reportes;
using Aries.Reporting.Utils;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using ClosedXML.Excel;
using Xunit;

namespace Aries.Desktop.Tests.Reportes
{
    public class ReporteAuxiliaresMonedaTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void Auxiliares_moneda_no_reconocida_escribe_cuentas_en_colones(int tipoMoneda)
        {
            var path = GenerarAuxiliares((CurrencyTypeCompany)tipoMoneda);
            try
            {
                using (var workbook = new XLWorkbook(path))
                {
                    var sheet = workbook.Worksheet(1);
                    Assert.Equal("Cuentas", Texto(sheet.Cell("A4")));
                    Assert.Contains(Textos(sheet), t => t == "CAJA");
                    Assert.Contains(Textos(sheet), t => t == "ACTIVOS");
                    Assert.DoesNotContain(Textos(sheet), t => t == "USD");
                    Assert.Contains(Montos(sheet), m => Math.Abs(m - 250.50m) < 0.01m);
                }
            }
            finally
            {
                Borrar(path);
            }
        }

        [Theory]
        [InlineData(CurrencyTypeCompany.Dolares_y_Colones, true)]
        [InlineData(CurrencyTypeCompany.Solo_Colones, false)]
        [InlineData(CurrencyTypeCompany.Solo_Dolares, true)]
        public void Auxiliares_monedas_conocidas_siguen_emitiendo_cuentas(CurrencyTypeCompany tipo, bool ambasDivisas)
        {
            var path = GenerarAuxiliares(tipo);
            try
            {
                using (var workbook = new XLWorkbook(path))
                {
                    var sheet = workbook.Worksheet(1);
                    Assert.Equal("Cuentas", Texto(sheet.Cell("A4")));
                    Assert.Contains(Textos(sheet), t => t == "CAJA");
                    Assert.Equal(ambasDivisas, Textos(sheet).Any(t => t == "USD"));
                }
            }
            finally
            {
                Borrar(path);
            }
        }

        [Fact]
        public void Maestro_moneda_no_reconocida_escribe_saldos_en_colones()
        {
            ExcelShell.OpenAfterSave = false;
            var path = Path.Combine(Path.GetTempPath(), "aries-maestro-" + Guid.NewGuid().ToString("N") + ".xlsx");
            var company = new Company
            {
                Code = "C233",
                CompanyName = "PROPIEDADES E INVERSIONES CAMARO",
                CurrencyType = (CurrencyTypeCompany)0
            };
            var usuario = new Usuario { MyNombre = "MIRIAM", MyApellidoPaterno = "H" };
            var cuentas = Plan();

            ReporteMaestroCuenta.GenerarReporte(
                cuentas,
                path,
                company,
                usuario,
                new FechaTransaccion(new DateTime(2026, 1, 1), 1),
                GenerarSaldo: true);

            try
            {
                using (var workbook = new XLWorkbook(path))
                {
                    var sheet = workbook.Worksheet(1);
                    Assert.Equal("Cuentas", Texto(sheet.Cell("A4")));
                    Assert.Contains(Textos(sheet), t => t == "CAJA");
                    Assert.Contains(Textos(sheet), t => t.StartsWith("Saldo Anterior", StringComparison.OrdinalIgnoreCase));
                    Assert.Contains(Montos(sheet), m => Math.Abs(m - 250.50m) < 0.01m);
                }
            }
            finally
            {
                Borrar(path);
            }
        }

        private static string GenerarAuxiliares(CurrencyTypeCompany tipoMoneda)
        {
            ExcelShell.OpenAfterSave = false;
            var path = Path.Combine(Path.GetTempPath(), "aries-auxiliares-" + Guid.NewGuid().ToString("N") + ".xlsx");
            var company = new Company
            {
                Code = "C233",
                CompanyName = "PROPIEDADES E INVERSIONES CAMARO SOCIEDAD ANONIMA"
            };
            var usuario = new Usuario { MyNombre = "MIRIAM", MyApellidoPaterno = "H" };
            var cuentas = Plan();
            var mes = new FechaTransaccion(new DateTime(2026, 1, 1), 1);
            var porMes = new Dictionary<FechaTransaccion, List<Cuenta>>
            {
                { mes, cuentas }
            };

            ReporteAuxiliares.GenerarReporte(porMes, company, usuario, tipoMoneda, path);
            return path;
        }

        private static List<Cuenta> Plan()
        {
            var titulo = new Cuenta("ACTIVOS", 1, 0, IndicadorCuenta.Cuenta_Titulo)
            {
                TipoCuenta = Cuenta.GenerarTipoCuenta(1)
            };
            var auxiliar = new Cuenta("CAJA", 2, 1, IndicadorCuenta.Cuenta_Auxiliar)
            {
                TipoCuenta = Cuenta.GenerarTipoCuenta(1),
                DebitosColones = 250.50m
            };
            return new List<Cuenta> { titulo, auxiliar };
        }

        private static string Texto(IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
                return string.Empty;
            return Convert.ToString(cell.Value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static IEnumerable<string> Textos(IXLWorksheet sheet)
        {
            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
            var lastCol = sheet.LastColumnUsed()?.ColumnNumber() ?? 0;
            for (var r = 1; r <= lastRow; r++)
            {
                for (var c = 1; c <= lastCol; c++)
                {
                    var text = Texto(sheet.Cell(r, c)).Trim();
                    if (text.Length > 0)
                        yield return text;
                }
            }
        }

        private static IEnumerable<decimal> Montos(IXLWorksheet sheet)
        {
            foreach (var text in Textos(sheet))
            {
                if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var inv))
                    yield return inv;
                if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("es-CR"), out var cr))
                    yield return cr;
            }
        }

        private static void Borrar(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
