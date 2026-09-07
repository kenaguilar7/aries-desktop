using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Utils
{
    public static class DateTimeExtensions
    {

        public static DateTime UpdateToFirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0); 
        }
    }
}
