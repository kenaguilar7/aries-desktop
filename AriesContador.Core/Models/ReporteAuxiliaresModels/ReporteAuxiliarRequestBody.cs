using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    public class ReporteAuxiliarRequestBody
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
}
