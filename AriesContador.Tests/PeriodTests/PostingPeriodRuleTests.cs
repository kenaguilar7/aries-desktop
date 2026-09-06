using System;
using System.Collections.Generic;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace AriesContador.Tests.PeriodTests
{
    public class PostingPeriodRuleTests
    {
        [Fact]
        public void PeriodExist_matches_year_and_month_ignoring_day()
        {
            var existing = new[]
            {
                new PostingPeriod { Date = new DateTime(2024, 6, 30) }
            };

            Assert.True(existing.PeriodExist(new PostingPeriod { Date = new DateTime(2024, 6, 1) }));
            Assert.False(existing.PeriodExist(new PostingPeriod { Date = new DateTime(2024, 7, 1) }));
        }

        [Fact]
        public void Creator_without_movements_offers_month_before_oldest_and_after_newest()
        {
            var periods = new List<PostingPeriod>
            {
                new PostingPeriod { Date = new DateTime(2024, 2, 1) },
                new PostingPeriod { Date = new DateTime(2024, 4, 1) }
            };

            var available = new PostingPeriodCreator(periods, existMovements: false)
                .GetAvailablePostingPeriodForBeCreated();

            Assert.Equal(2, available.Count);
            Assert.Contains(available, p => p.Date == new DateTime(2024, 1, 1));
            Assert.Contains(available, p => p.Date == new DateTime(2024, 5, 1));
        }

        [Fact]
        public void Creator_with_movements_offers_only_month_after_newest()
        {
            var periods = new List<PostingPeriod>
            {
                new PostingPeriod { Date = new DateTime(2024, 2, 1) },
                new PostingPeriod { Date = new DateTime(2024, 4, 1) }
            };

            var available = new PostingPeriodCreator(periods, existMovements: true)
                .GetAvailablePostingPeriodForBeCreated();

            var only = Assert.Single(available);
            Assert.Equal(new DateTime(2024, 5, 1), only.Date);
        }

        [Fact]
        public void New_company_period_is_first_day_of_current_month()
        {
            var created = new PostingPeriodCreator().CreatePostingPeriodForNewCompany();
            Assert.Equal(DateTime.Now.UpdateToFirstDayOfMonth(), created.Date);
        }
    }
}
