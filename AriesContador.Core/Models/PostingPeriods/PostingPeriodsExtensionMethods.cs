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
            var output = postingPeriods.DeepClone().Where(p => p.Date >= cutPeriod);
            return output.ToList(); 
        }

        public static PostingPeriod GetOlderAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderBy(x => x.Date).FirstOrDefault();

        public static PostingPeriod GetNewerAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderByDescending(x => x.Date).FirstOrDefault();
    }
}
