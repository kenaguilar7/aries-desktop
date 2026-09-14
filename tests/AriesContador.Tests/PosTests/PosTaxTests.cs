using AriesContador.Core.Models.PointOfSale;
using Xunit;

namespace AriesContador.Tests.PosTests
{
    public class PosTaxTests
    {
        [Fact]
        public void SplitGross_13_percent_inclusive()
        {
            var split = PosTax.SplitGross(113m, 0.13m, false);
            Assert.Equal(100m, split.Net);
            Assert.Equal(13m, split.Tax);
        }

        [Fact]
        public void SplitGross_exempt_keeps_gross_as_net()
        {
            var split = PosTax.SplitGross(50m, 0.13m, true);
            Assert.Equal(50m, split.Net);
            Assert.Equal(0m, split.Tax);
        }

        [Fact]
        public void LineCost_weight_uses_kilo()
        {
            var product = new Product { Cost = 1000m, SoldByWeight = true };
            var line = new SaleLine { WeightGrams = 500m };
            Assert.Equal(500m, PosTax.LineCost(product, line));
        }
    }
}
