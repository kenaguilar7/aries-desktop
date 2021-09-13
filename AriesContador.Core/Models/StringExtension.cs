using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models
{
    public static class StringExtension
    {
        public static DateTime ParseToDateTimeWithFromMimFormat(this string dateString)
        {
            var year = Convert.ToInt32(dateString.Substring(0, 4));
            var month = Convert.ToInt32(dateString.Substring(4, 2));

            return new DateTime(year, month, 1); 
        }

        public static string ParseToYearMonthStringFromDate(this DateTime dateTime)
        {
            return $"{dateTime.Date.Year}{dateTime.Date.Month,0:D2}"; 
        }

    }
}
