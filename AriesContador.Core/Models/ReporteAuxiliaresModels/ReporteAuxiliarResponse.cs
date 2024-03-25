using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
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
