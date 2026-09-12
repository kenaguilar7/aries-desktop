using Aries.Reporting.Entidades.Cuentas;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class EgresoSaldoMensualTests
    {
        [Fact]
        public void SaldoMensual_matches_1_1_15_credito_minus_debito()
        {
            var egreso = new Egreso();
            Assert.Equal(-40m, egreso.SaldoMensual(50m, 10m));
            Assert.Equal(25m, egreso.SaldoMensual(10m, 35m));
        }
    }
}
