using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Cuentas;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Patterns.ActionsWorker;
using ClosedXML.Excel;
using Xunit;

namespace Aries.Flujos.Tests.Reportes
{
    [Collection("flujos-db")]
    public class ReportesFlujosTests
    {
        private readonly FlujosDbFixture _db;

        public ReportesFlujosTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Estado_resultado_diciembre_2020_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("eri");

            await new ReportResultadoIntegralActions(
                _db.Services.Reports,
                new ReportResultadoPameter
                {
                    CompanyId = ctx.Company.Code,
                    FirstDate = ctx.Periodo,
                    EndDate = ctx.Periodo,
                    UserName = FlujosDbFixture.AdminUserName
                },
                output).Execute();

            try
            {
                var generated = LeerSaldosEri(output);
                var expected = ctx.Plantilla;
                foreach (var row in expected.Where(r => r.IsTotal || r.IsPeriodResult))
                    AssertSaldo(generated, row.Name, row.Saldo);

                foreach (var row in expected.Where(r => !r.IsTotal && !r.IsPeriodResult && Math.Abs(r.Saldo) >= 0.01m))
                    AssertSaldo(generated, row.Name, row.Saldo);
            }
            finally
            {
                Delete(output);
            }
        }

        [DockerFact]
        public async Task Comprobacion_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("comprobacion");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveComprobacionAsync(output, ctx.Periodo, ctx.Periodo);
            try
            {
                AssertExcelCuentas(Excel.PlantillaComprobacion, output, 5, 6, "Balance de comprobación");
            }
            finally
            {
                Delete(output);
            }
        }

        [DockerFact]
        public async Task Asientos_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("asientos");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveAsientosAsync(output, ctx.Periodo, ctx.Periodo);
            try
            {
                var expected = ExcelCompare.LeerAsientos(Excel.FixturePath(Excel.FuenteAsientos));
                var generated = ExcelCompare.LeerAsientos(output);
                ExcelCompare.AssertNoDiff(ExcelCompare.DiffAsientos(expected, generated), "Reporte de asientos");
            }
            finally
            {
                Delete(output);
            }
        }

        [DockerFact]
        public async Task Auxiliares_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("auxiliares");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveAuxiliaresAsync(output, ctx.Periodo, ctx.Periodo);
            try
            {
                AssertExcelCuentas(Excel.PlantillaAuxiliares, output, 7, 2, "Balance de auxiliares");
            }
            finally
            {
                Delete(output);
            }
        }

        [DockerFact]
        public async Task Balance_situacion_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("situacion");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveBalanceSituacionAsync(output, ctx.Periodo, ctx.Periodo);
            try
            {
                AssertExcelCuentas(Excel.PlantillaBalanceSituacion, output, 5, 0, "Balance de situación");
            }
            finally
            {
                Delete(output);
            }
        }

        [DockerFact]
        public async Task Movimientos_auxiliar_coincide_con_plantilla_1_1_15()
        {
            await AssertMovimientosAsync(
                HopeMuestra.CuentaIngresosCirugias,
                Excel.PlantillaMovimientosAuxiliar,
                "Movimientos auxiliar INGRESOS POR CIRUGIAS");
        }

        [DockerFact]
        public async Task Movimientos_titulo_coincide_con_plantilla_1_1_15()
        {
            await AssertMovimientosAsync(
                Excel.CuentaMovimientosTitulo,
                Excel.PlantillaMovimientosTitulo,
                "Movimientos título INGRESOS MEDICOS-2024");
        }

        [DockerFact]
        public async Task Maestro_coincide_con_plantilla_1_1_15()
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("maestro");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveMaestroAsync(output, ctx.Periodo, ctx.Periodo);
            try
            {
                AssertExcelCuentas(Excel.PlantillaMaestro, output, 6, 4, "Maestro de cuentas");
            }
            finally
            {
                Delete(output);
            }
        }

        private async Task AssertMovimientosAsync(string accountName, string fixture, string titulo)
        {
            var ctx = await Semilla.CrearAsync(_db.Services);
            var output = TempPath("movimientos");
            await new ReportesWriter(_db.Services, ctx.Company, FlujosDbFixture.AdminUserName)
                .SaveMovimientosAsync(output, accountName);
            try
            {
                var expected = ExcelCompare.LeerMovimientos(Excel.FixturePath(fixture));
                var generated = ExcelCompare.LeerMovimientos(output);
                ExcelCompare.AssertNoDiff(ExcelCompare.DiffMovimientos(expected, generated), titulo);
            }
            finally
            {
                Delete(output);
            }
        }

        private static void AssertExcelCuentas(string fixture, string generatedPath, int startRow, int amountCount, string titulo)
        {
            var expected = ExcelCompare.LeerCuentas(Excel.FixturePath(fixture), startRow, amountCount);
            var generated = ExcelCompare.LeerCuentas(generatedPath, startRow, amountCount);
            ExcelCompare.AssertNoDiff(ExcelCompare.DiffCuentas(expected, generated), titulo);
        }

        private static void AssertSaldo(IReadOnlyList<EriFila> generated, string name, decimal expected)
        {
            var actual = generated.Where(r => Excel.NamesEqual(r.Name, name)).ToList();
            Assert.True(actual.Count > 0, "No salió en el Excel generado: " + name);
            Assert.Contains(actual, r => Math.Abs(r.Saldo - expected) < 0.02m);
        }

        private static List<EriFila> LeerSaldosEri(string path)
        {
            var rows = new List<EriFila>();
            using (var workbook = new XLWorkbook(path))
            {
                var sheet = workbook.Worksheet(1);
                var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
                var lastCol = sheet.LastColumnUsed()?.ColumnNumber() ?? 6;
                for (var r = 5; r <= lastRow; r++)
                {
                    var name = string.Empty;
                    for (var c = 1; c < lastCol; c++)
                    {
                        var text = Excel.Normalize(Excel.CellText(sheet.Cell(r, c)));
                        if (text.Length == 0)
                            continue;
                        name = text;
                        break;
                    }

                    if (name.Length == 0)
                        continue;
                    rows.Add(new EriFila
                    {
                        Name = Excel.ClipName(name),
                        Saldo = Excel.CellAmount(sheet.Cell(r, lastCol))
                    });
                }
            }

            return rows;
        }

        private static string TempPath(string name)
        {
            return Path.Combine(Path.GetTempPath(), "aries-flujos-" + name + "-" + Guid.NewGuid().ToString("N") + ".xlsx");
        }

        private static void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
