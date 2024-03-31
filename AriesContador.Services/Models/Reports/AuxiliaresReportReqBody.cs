using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using System.Collections.Generic;

namespace AriesContador.Services.Models.Reports
{
    public class AuxiliaresReportReqBody
    {
        public List<PostingPeriod> PostingPeriods { get; set; }
        public string CompanyId { get; set; }
        public CurrencyTypeCompany CurrencyType { get; set; }
        public ReportHeaderText ReportHeader { get; set; }
    }

    public class ReportHeaderText
    {
        public string CompanyName { get; set; }
        public string ReportName { get; set; }
        public string IssuerName { get; set; }
    }

    public class ReporteAuxiliarResponse 
    {
        public byte[] Report { get; set; }
        public int HeadersEntAt { get; set; }
        /// <summary>
        /// Numer of columns in base 0
        /// </summary>
        public int AccountNamesColumnLength { get; set; }

        public List<string> ColumnsBalanceHeaderText { get; set; } = new List<string>();
    }
}
