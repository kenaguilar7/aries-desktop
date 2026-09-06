using System.Linq;
using AriesContador.Core.Models.Patterns.ActionsWorker;
using AriesContador.Core.Models.Reports;
using Xunit;

namespace AriesContador.Tests.ReportTests
{
    public class EstadoResultadoIntegralTableTests
    {
        [Fact]
        public void ToDataTable_puts_tree_names_total_prefix_and_period_result_row()
        {
            var data = new[]
            {
                new EstadoResultadoIntegralReport
                {
                    AccountPath = "Ingresos",
                    IsMainAccount = true,
                    SaldoActual = 1000
                },
                new EstadoResultadoIntegralReport
                {
                    AccountPath = "Ingresos¡Ventas",
                    IsMainAccount = false,
                    SaldoActual = 1000
                }
            };

            var table = new ToDataTable(data, 750m).Execute();

            Assert.True(table.Columns.Contains("AccountName0"));
            Assert.True(table.Columns.Contains("AccountName1"));
            Assert.Equal("TOTAL Ingresos", table.Rows[0]["AccountName0"]);
            Assert.Equal("Ventas", table.Rows[1]["AccountName1"]);

            var total = table.Rows[table.Rows.Count - 1];
            Assert.Equal("UTILIDAD/PERDIDA PERIODO", total[0]);
            Assert.Equal(750m, (decimal)total["SaldoActual"]);
        }
    }
}
