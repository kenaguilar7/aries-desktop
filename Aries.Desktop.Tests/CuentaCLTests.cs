using System.Collections.Generic;
using System.Linq;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaLogica;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class CuentaCLTests
    {
        private readonly CuentaCL _cl = new CuentaCL();

        [Fact]
        public void Ordernar_is_preorder_title_then_children()
        {
            var titulo = new Cuenta("Activo", 1, 0, IndicadorCuenta.Cuenta_Titulo);
            var mayor = new Cuenta("Circulante", 2, 1, IndicadorCuenta.Cuenta_De_Mayor);
            var aux = new Cuenta("Caja", 3, 2, IndicadorCuenta.Cuenta_Auxiliar);
            var pasivo = new Cuenta("Pasivo", 10, 0, IndicadorCuenta.Cuenta_Titulo);

            var ordered = _cl.Ordernar(new List<Cuenta> { aux, pasivo, mayor, titulo })
                .Select(c => c.Id)
                .ToArray();

            Assert.Equal(new[] { 10, 1, 2, 3 }, ordered);
        }

        [Fact]
        public void AplicarRollUpHaciaPadres_adds_auxiliar_debits_credits_to_parents()
        {
            var titulo = new Cuenta("Activo", 1, 0, IndicadorCuenta.Cuenta_Titulo);
            var mayor = new Cuenta("Circulante", 2, 1, IndicadorCuenta.Cuenta_De_Mayor);
            var aux = new Cuenta("Caja", 3, 2, IndicadorCuenta.Cuenta_Auxiliar)
            {
                DebitosColones = 80,
                CreditosColones = 5,
                DebitosDolares = 2,
                CreditosDolares = 1
            };

            _cl.AplicarRollUpHaciaPadres(new List<Cuenta> { titulo, mayor, aux });

            Assert.Equal(80, mayor.DebitosColones);
            Assert.Equal(5, mayor.CreditosColones);
            Assert.Equal(80, titulo.DebitosColones);
            Assert.Equal(2, titulo.DebitosDolares);
            Assert.Equal(80, aux.DebitosColones);
        }

        [Fact]
        public void QuitarCuentasSinSaldos_keeps_titles_without_movement()
        {
            var titulo = new Cuenta("Activo", 1, 0, IndicadorCuenta.Cuenta_Titulo);
            var mayorVacio = new Cuenta("Mayor vacio", 2, 1, IndicadorCuenta.Cuenta_De_Mayor);
            var auxConSaldo = new Cuenta("Caja", 3, 2, IndicadorCuenta.Cuenta_Auxiliar)
            {
                DebitosColones = 10
            };

            var filtered = _cl.QuitarCuentasSinSaldos(new List<Cuenta> { titulo, mayorVacio, auxConSaldo });

            Assert.Contains(filtered, c => c.Id == 1);
            Assert.Contains(filtered, c => c.Id == 3);
            Assert.DoesNotContain(filtered, c => c.Id == 2);
        }

        [Fact]
        public void Deleted_rejects_system_accounts()
        {
            var cuenta = new Cuenta("Sistema", 1, 0, IndicadorCuenta.Cuenta_Auxiliar) { Editable = false };
            var ok = _cl.Deleted(cuenta, null, out var mensaje);

            Assert.False(ok);
            Assert.Contains("sistema", mensaje.ToLowerInvariant());
        }

        [Fact]
        public void Deleted_rejects_non_auxiliar()
        {
            var cuenta = new Cuenta("Titulo", 1, 0, IndicadorCuenta.Cuenta_Titulo) { Editable = true };
            var ok = _cl.Deleted(cuenta, null, out var mensaje);

            Assert.False(ok);
            Assert.Contains("No pueden ser eliminadas", mensaje);
        }

        [Fact]
        public void HeredarSaldosSiPadreEsAuxiliar_copies_balances()
        {
            var padre = new Cuenta("Padre aux", 2, 1, IndicadorCuenta.Cuenta_Auxiliar)
            {
                SaldoAnteriorColones = 40,
                DebitosColones = 10,
                CreditosColones = 3,
                DebitosDolares = 1
            };
            var nueva = new Cuenta("Hija", 3, 2, IndicadorCuenta.Cuenta_Auxiliar);

            _cl.HeredarSaldosSiPadreEsAuxiliar(nueva, padre);

            Assert.Equal(40, nueva.SaldoAnteriorColones);
            Assert.Equal(10, nueva.DebitosColones);
            Assert.Equal(3, nueva.CreditosColones);
            Assert.Equal(1, nueva.DebitosDolares);
        }

        [Fact]
        public void HeredarSaldosSiPadreEsAuxiliar_does_nothing_when_parent_is_mayor()
        {
            var padre = new Cuenta("Mayor", 2, 1, IndicadorCuenta.Cuenta_De_Mayor)
            {
                DebitosColones = 99
            };
            var nueva = new Cuenta("Hija", 3, 2, IndicadorCuenta.Cuenta_Auxiliar);

            _cl.HeredarSaldosSiPadreEsAuxiliar(nueva, padre);

            Assert.Equal(0, nueva.DebitosColones);
        }

        [Fact]
        public void VerificarNombre_rejects_blank()
        {
            var cuenta = new Cuenta("x", 1, 0);
            var ok = _cl.VerificarNombre(cuenta, "   ", out var mensaje, null);

            Assert.False(ok);
            Assert.Equal("INGRESE UN NOMBRE VALIDO", mensaje);
        }
    }
}
