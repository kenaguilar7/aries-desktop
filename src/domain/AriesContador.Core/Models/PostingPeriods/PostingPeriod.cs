using AriesContador.Core.Models.Companies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Models.JournalEntries;

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

    public class PostingPeriodEndClosing : BaseModel
    {
        public string CompanyId { get; set; }

        public int FromPeriodId { get; set;  }

        public int ToPeriodId { get; set; }

        public string FromPeriod { get; set; }

        public string ToPeriod { get; set; }

        public decimal Amount { get; set; }

        public string UserNotes { get; set; }

        public List<PostingPeriod> PostingPeriods { get; set; }
    }

    public class PostingPeriodInfo
    {
        public string PostingPeriodDateString { get; set; }
        public string Status { get; set; }
        public string CreatedDateString { get; set; }
        public string ClosedDateString { get; set; }
        public string UserName { get; set; }
    }

    public class PostingPeriodCreator
    {
        private List<PostingPeriod> _postingPeriods = new List<PostingPeriod>();
        private bool _existMovements; 
        public PostingPeriodCreator()
        {
            
        }

        public PostingPeriodCreator(List<PostingPeriod> actualOpenPeriods,
            bool existMovements)
        {
            _postingPeriods = actualOpenPeriods;
            _existMovements = existMovements;
        }

        public List<PostingPeriod> GetAvailablePostingPeriodForBeCreated()
        {
            var returnedList = new List<PostingPeriod>();

            //Create only future periods
            if (!_existMovements)
            {
                var olderPeriod = _postingPeriods.GetOlderAccountPeriod();
                var newerPeriod = _postingPeriods.GetNewerAccountPeriod();
                var olderDate = olderPeriod.Date.AddMonths(-1);
                var newerDate = newerPeriod.Date.AddMonths(1);

                returnedList.AddRange(new List<PostingPeriod>()
                {
                    new PostingPeriod(){Date = olderDate.UpdateToFirstDayOfMonth() },
                    new PostingPeriod(){Date = newerDate.UpdateToFirstDayOfMonth() }, 

                });

            }
            else
            {
                var newerPeriod = _postingPeriods.GetNewerAccountPeriod();
                var newerDate = newerPeriod.Date.AddMonths(1);

                returnedList.Add(new PostingPeriod()
                {
                    Date = newerDate.UpdateToFirstDayOfMonth()
                });
            }


            return returnedList;
        }

        public PostingPeriod CreatePostingPeriodForNewCompany()
        {
            var output = new PostingPeriod() {Date = DateTime.Now.UpdateToFirstDayOfMonth() };
            return output;
        }
    }

}
