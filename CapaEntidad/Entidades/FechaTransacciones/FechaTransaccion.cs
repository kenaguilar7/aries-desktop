using CapaEntidad.Entidades.Asientos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.FechaTransacciones
{
    public class FechaTransaccion
    {
        public decimal Id { get; set; }
        public DateTime Fecha { get; set; }
        public Boolean Cerrada { get; set; }
        public List<Asiento> Asientos {get; set; }
        public FechaTransaccion(DateTime fecha, bool cerrada = false, decimal id = 0)
        {
            Id = id;
            Fecha = fecha;
            Cerrada = cerrada;
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
