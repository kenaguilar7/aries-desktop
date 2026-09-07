using System;
using System.Collections.Generic;
using System.Linq;
using AriesContador.Core.Models.Utils;

namespace AriesContador.Core.Models.PostingPeriods
{
    public static class PostingPeriodsExtensionMethods
    {

        public static List<PostingPeriod> GetOlder(this List<PostingPeriod> postingPeriods, DateTime cutPeriod)
        {
            var output = postingPeriods.Where(p => p.Date >= cutPeriod);
            return output.ToList().DeepClone(); 
        }

        public static List<PostingPeriod> GetByRange(this List<PostingPeriod> postingPeriods, DateTime startDateTime,
            DateTime endDateTime)
        {
            var output = postingPeriods.Where(p => p.Date >= startDateTime && p.Date <= endDateTime.Date);
            return output.ToList().DeepClone();
        }

        public static PostingPeriod GetOlderAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderBy(x => x.Date).FirstOrDefault();

        public static PostingPeriod GetNewerAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderByDescending(x => x.Date).FirstOrDefault();

        public static bool PeriodExist(this IEnumerable<PostingPeriod> postingPeriods, PostingPeriod postingPeriod)
        {
            var exist = postingPeriods
                .Any(x => x.Date.Year == postingPeriod.Date.Year &&
                          x.Date.Month == postingPeriod.Date.Month);
            return exist; 
        }
    }
}
