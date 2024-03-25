using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    public class ReporteAuxiliarRequestBody
    {
        public List<PostingPeriod> PostingPeriods { get; set; }
        public string CompanyId { get; set; }
        public CurrencyTypeCompany currencyTypeCompany { get; set; }
    }
}
