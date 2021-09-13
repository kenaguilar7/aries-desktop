using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Utils
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns>A string date format YYYY MM</returns>
        public static string BuildDateToParts(this DateTime source)
        {
            return $"{source.Year}{string.Format("{0, 0:D2}", source.Month)}"; 
        }

    }
}
