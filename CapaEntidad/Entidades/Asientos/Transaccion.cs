using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using System;

namespace CapaEntidad.Entidades.Asientos
{
    public class Transaccion
    {
        public int Id { get; set; }
        public Cuenta CuentaDeAsiento { get; set; }
        public String Referencia { get; set; }
        public String Detalle { get; set; }
        public DateTime FechaFactura { get; set; }
        public TipoCambio TipoCambio { get; set; }

        private decimal _monto;
        public decimal Monto {
            get { return _monto; }
            ///Todo número se almacenara siempre con dos decimales
            set { _monto = Decimal.Truncate(value * 100) / 100; ; }
        }
        public Comportamiento ComportamientoCuenta { get; set; }
        public decimal MontoTipoCambio { get; set; } = 1.00m; 
        public Transaccion(Cuenta cuentaDeAsiento, string referencia, string detalle, DateTime fechaFactura,
                           decimal balance,TipoCambio tipoCambio, Comportamiento comportamientoCuenta = Comportamiento.Credito, int id = 0, decimal montoTipoCambio = 1.00m)
        {
            this.CuentaDeAsiento = cuentaDeAsiento;
            this.Referencia = referencia;
            this.Detalle = detalle;
            this.FechaFactura = fechaFactura;
            this.Monto = balance;
            this.TipoCambio = tipoCambio; 
            this.ComportamientoCuenta = comportamientoCuenta;
            this.Id = id;
            this.MontoTipoCambio = montoTipoCambio;
        }
        public Transaccion() {
            
        }

        
    }
}
