using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AriesContador.Core.Models.PostingPeriods;
using Xunit;

namespace AriesContador.Tests.FinancialTests
{
    public class PostingPeriodsTests
    {

        [Fact]
        public void OrderPostingPeriodsStarListAndEndList()
        {
            string file = System.IO.File.ReadAllText("FinancialTests/Resources/postingPeriods.json");
            var postingPeriods = JsonSerializer.Deserialize<List<PostingPeriod>>(file);



            var asse1 = new PostingPeriod()
            {
                Date = new DateTime(2021,05,1)
            }; 




        }


    }
}
