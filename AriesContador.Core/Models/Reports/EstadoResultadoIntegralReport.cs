using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Attributes;

namespace AriesContador.Core.Models.Reports
{
    public class EstadoResultadoIntegralReport
    {
        [XLColumn(Header = "Ruta de cuenta")]
        //[DisplayName("Ruta de cuenta")]
        [IgnoreDataMember]
        public string AccountPath { get; set; }
        [XLColumn(Header = "Cuenta")]
        //[DisplayName("Cuenta")]
        [IgnoreDataMember]
        public string Account { get; set; }
        [XLColumn(Header = "IsMainAccount")]
        //[DisplayName("Cuenta")]
        [IgnoreDataMember]
        public bool IsMainAccount { get; set; }

        [XLColumn(Header = "Saldo")]
        [DisplayName("Saldo")]
        public decimal SaldoActual { get; set; }
    }

    public class ResultReportEstadoResultadoIntegral
    {
        public IEnumerable<EstadoResultadoIntegralReport> Results { get; set; }
        public decimal TotalPeridaGanancia { get; set; }
    }
}
