using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriesContador.Core.Models.Utils
{
    public static class ListExtension
    {
        public static PostingPeriod GetOlderAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderBy(x => x.Date).FirstOrDefault();

        public static PostingPeriod GetNewerAccountPeriod(this IEnumerable<PostingPeriod> postingP)
            => postingP.OrderByDescending(x => x.Date).FirstOrDefault();
    }
}
