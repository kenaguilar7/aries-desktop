using AriesContador.Core.Models.Companies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CapaEntidad.Entidades.JournalEntries;

namespace AriesContador.Core.Models.PostingPeriods
{
    public class PostingPeriod : BaseModel
    {
        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

        public string CompanyId { get; set; }

        public DateTime Date { get; set; }

        public Boolean Closed { get; set; }

        public int ClosedMySQL
        {
            get { return Convert.ToInt32(this.Closed); }
            set { this.Closed = Convert.ToBoolean(value); }
        }

        public int Year { get; set; }

        public int Month { get; set; }

        public override string ToString()
        {
            return $"{MonthName(Date.Month)} {Date.Year}";
        }

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }
    }
}
