using System;
using System.Globalization;

namespace AriesContador.Core.Models.PointOfSale
{
    public static class PosTax
    {
        public const decimal DefaultRate = 0.13m;

        public static (decimal Net, decimal Tax) SplitGross(decimal gross, decimal rate, bool exempt)
        {
            if (gross <= 0)
                return (0m, 0m);
            if (exempt || rate <= 0)
                return (gross, 0m);
            var net = Math.Round(gross / (1m + rate), 2, MidpointRounding.AwayFromZero);
            return (net, gross - net);
        }

        public static decimal LineCost(Product product, SaleLine line)
        {
            if (product == null || product.Cost <= 0)
                return 0m;
            if (product.SoldByWeight)
                return Math.Round(product.Cost * line.WeightGrams / 1000m, 2, MidpointRounding.AwayFromZero);
            return Math.Round(product.Cost * line.Quantity, 2, MidpointRounding.AwayFromZero);
        }

        public static string TotalsHash(
            decimal cash,
            decimal card,
            decimal transfer,
            decimal net,
            decimal tax,
            decimal cost,
            decimal difference)
        {
            return string.Join("|", new[]
            {
                cash.ToString("0.00", CultureInfo.InvariantCulture),
                card.ToString("0.00", CultureInfo.InvariantCulture),
                transfer.ToString("0.00", CultureInfo.InvariantCulture),
                net.ToString("0.00", CultureInfo.InvariantCulture),
                tax.ToString("0.00", CultureInfo.InvariantCulture),
                cost.ToString("0.00", CultureInfo.InvariantCulture),
                difference.ToString("0.00", CultureInfo.InvariantCulture)
            });
        }
    }
}
