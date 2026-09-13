using System;
using AriesContador.Core.Models.Utils;

namespace Aries.Flujos.Tests.Cuentas
{
    /// <summary>
    /// Datos tomados del reporte 1.1.15
    /// <c>REPORTE DE ASIENTOS GRUPO ONCOLOGICO HOPE S.A.-C151.xlsx</c>
    /// (asientos 2 y 3 de diciembre 2020).
    /// </summary>
    internal static class HopeMuestra
    {
        public const string CompaniaNombre = "GRUPO ONCOLOGICO HOPE S.A.";
        public const string CompaniaMail = "hope@aries.test";

        public static readonly DateTime MesContable = new DateTime(2020, 12, 1);
        public static readonly DateTime FechaAsiento2 = DateTime.FromOADate(44196);
        public static readonly DateTime FechaAsiento3 = DateTime.FromOADate(44196);

        public const string CuentaBacColones = "BAC 912612520 - COLONES";
        public const string CuentaCxcEfectivo = "CXC EFECTIVO";
        public const string CuentaRetencionTarjetas = "RETENCION 1.76% (RENTA TARJETAS)";
        public const string CuentaComisionesTarjetas = "COMISIONES TARJETAS DE CRÉDITO";
        public const string CuentaUsoDatafono = "USO DATAFONO CANCER";
        public const string CuentaIngresosCirugias = "INGRESOS POR CIRUGIAS";
        public const string CuentaBancos = "BANCOS";
        public const string CuentaCuentasPorCobrar = "CUENTAS POR COBRAR";
        public const string CuentaOtrosActivos = "OTROS ACTIVOS CORRIENTES";
        public const string CuentaPasivoCorto = "PASIVO CORTO PLAZO";
        public const string CuentaIngresoMayor = "INGRESO";

        public const string RefAsiento2 = "tef.425878203 cf. de noviembre consulta";
        public const string DetalleAsiento2 = "tef.425878203 cf. de noviembre consulta";
        public const decimal Asiento2BacDebito = 57444.00m;
        public const decimal Asiento2ComisionDebito = 3857.76m;
        public const decimal Asiento2RetencionDebito = 1098.24m;
        public const decimal Asiento2CxcCredito = 62400.00m;

        public const string RefAsiento3 = "ajuste ventas periodo";
        public const string DetalleAsiento3 = "ajuste ventas periodo";
        public const decimal Asiento3DatafonoDebito = 262352.00m;
        public const decimal Asiento3CirugiasCredito = 262352.00m;

        public static readonly Currency MonedaColones = Currency.colones;
    }
}
