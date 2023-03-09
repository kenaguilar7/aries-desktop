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
    public class BalanceComprobacionReport
    {
        [XLColumn(Header = "Ruta de cuenta")]
        //[DisplayName("Ruta de cuenta")]
        [IgnoreDataMember]
        public string AccountPath { get; set; }
        [XLColumn(Header = "Cuenta")]
        //[DisplayName("Cuenta")]
        [IgnoreDataMember]
        public string Account { get; set; }
        [XLColumn(Header = "Saldo Anterior Debitos")]
        [DisplayName("Saldo Anterior Debitos")]
        public decimal SaldoAnteriorDeb { get; set; }
        [XLColumn(Header = "Saldo Anterior Creditos")]
        [DisplayName("Saldo Anterior Creditos")]
        public decimal SaldoAnteriorCred { get; set; }
        [XLColumn(Header = "Saldo Mensual Debitos")]
        [DisplayName("Saldo Mensual Debitos")]
        public decimal SaldoMensualDeb { get; set; }
        [XLColumn(Header = "Saldo Mensual Creditos")]
        [DisplayName("Saldo Mensual Creditos")]
        public decimal SaldoMensualCred { get; set; }
        [XLColumn(Header = "Saldo Cuenta Debitos")]
        [DisplayName("Saldo Cuenta Debitos")]
        public decimal SaldoActualCuentaDeb { get; set; }
        [XLColumn(Header = "Saldo Cuenta Creditos")]
        [DisplayName("Saldo Cuenta Creditos")]
        public decimal SaldoActualCuentaCred { get; set; }
    }
}
