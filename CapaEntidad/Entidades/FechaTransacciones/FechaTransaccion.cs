using System;
using System.Globalization;

namespace CapaEntidad.Entidades.FechaTransacciones
{
    public class FechaTransaccion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Boolean Cerrada { get; set; }
        public FechaTransaccion(DateTime fecha, int id, bool cerrada = false)
        {
            Id = id;
            Fecha = fecha;
            Cerrada = cerrada;
        }

        public FechaTransaccion()
        {
            
        }
        public override string ToString()
        {
            return $"{MonthName(Fecha.Month)} {Fecha.Year}"; 
        }
        public string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }
    }
}
