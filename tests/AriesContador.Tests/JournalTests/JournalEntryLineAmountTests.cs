using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace AriesContador.Tests.JournalTests
{
    public class JournalEntryLineAmountTests
    {
        [Fact]
        public void Colones_uses_rate_one_and_entered_amount()
        {
            var line = new JournalEntryLine();
            JournalEntryLineAmount.Apply(line, Currency.colones, 1250.75m, 550m);

            Assert.Equal(Currency.colones, line.Currency);
            Assert.Equal(1.00m, line.RateAmount);
            Assert.Equal(1250.75m, line.Amount);
            Assert.Equal(0m, line.ForeignAmount);
        }

        [Fact]
        public void Dolares_truncates_colones_amount_to_two_decimals()
        {
            var line = new JournalEntryLine();
            JournalEntryLineAmount.Apply(line, Currency.dolares, 1.111m, 2.00m);

            Assert.Equal(Currency.dolares, line.Currency);
            Assert.Equal(2.00m, line.RateAmount);
            Assert.Equal(1.111m, line.ForeignAmount);
            Assert.Equal(2.22m, line.Amount);
        }

        [Fact]
        public void Truncate_does_not_round()
        {
            Assert.Equal(1.23m, JournalEntryLineAmount.TruncateToTwoDecimals(1.239m));
            Assert.Equal(1.23m, JournalEntryLineAmount.TruncateToTwoDecimals(1.231m));
        }
    }
}
