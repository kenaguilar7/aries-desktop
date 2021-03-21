using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Financial; 

namespace CapaEntidad.Entidades.Asientos
{
    public class Asiento
    {
        public int Id { get; set; }
        public int NumeroAsiento { get; set; }
        public List<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
        public DateTime FechaRegistro { get; set; }
        public FechaTransaccion FechaAsiento { get; set; }
        public Boolean Convalidado { get; set; }
        public DateTime FechaConvalidacion { get; set; }
        public Compañia Compania { get; set; }
        public EstadoAsiento Estado { get; set; }


        public Asiento(int numeroAsiento, List<Transaccion> transaccions, DateTime fechaRegistro, FechaTransaccion fechaAiento,
                       Compañia compania, EstadoAsiento estado = EstadoAsiento.Proceso, int Id = 0)
        {

            this.NumeroAsiento = numeroAsiento;
            this.Transaccions = transaccions;
            this.FechaRegistro = fechaRegistro;
            this.FechaAsiento = fechaAiento;
            this.Convalidado = Convalidado;
            this.Compania = compania;
            this.Id = Id;
        }

        public Asiento(int numeroAsiento, Compañia compania)
        {
            NumeroAsiento = numeroAsiento;
            Compania = compania;
        }

        public Asiento()
        {
        }

        public override string ToString()
        {
            return Convert.ToString(NumeroAsiento);
        }
        public decimal DebitosColones
        {
            get
            {
                return GetMontoTransaccion(Comportamiento.Debito);
            }
        }


        public decimal CreditosColones
        {
            get
            {
                return GetMontoTransaccion(Comportamiento.Credito);
            }
        }

        public Boolean Cuadrado
        {
            get
            {
                return (DebitosColones == CreditosColones) ? true : false;
            }
        }

        private decimal GetMontoTransaccion(Comportamiento comportamiento)
        {
            return Transaccions.FindAll(x => x.ComportamientoCuenta == comportamiento).Sum(x => x.Monto);
        }
    }
}
