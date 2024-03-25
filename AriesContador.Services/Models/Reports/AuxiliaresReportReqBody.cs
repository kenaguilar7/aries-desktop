using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Services.Models.Reports
{
    public class AuxiliaresReportReqBody
    {
        public List<PostingPeriod> PostingPeriods { get; set; }
        public string CompanyId { get; set; }
        public CurrencyTypeCompany currencyTypeCompany { get; set; }
    }

    public class ReporteAuxiliarResponse 
    {
        public byte[] Report { get; set; }
        public int HeadersEntAt { get; set; }
        /// <summary>
        /// Numer of columns in base 0
        /// </summary>
        public int NumberOfColumns { get; set; }

        public List<string> ColumnsBalanceHeaderText { get; set; } = new List<string>();
    }
}
