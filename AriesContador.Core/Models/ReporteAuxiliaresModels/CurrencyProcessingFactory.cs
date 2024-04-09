using AriesContador.Core.Models.Utils;
using System;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    public static class CurrencyProcessingFactory
    {
        private static readonly IDictionary<CurrencyTypeCompany, ICurrencyProcessing> ProcessingStrategies =
                new Dictionary<CurrencyTypeCompany, ICurrencyProcessing>
                {
                        { CurrencyTypeCompany.Dolares_y_Colones, new MultiCurrencyColonesProcessing() },
                        { CurrencyTypeCompany.Solo_Colones, new LocalCurrencyProcessing() },
                        { CurrencyTypeCompany.Solo_Dolares, new ForeignCurrencyProcessing() }
                };

        public static ICurrencyProcessing GetCurrencyProcessing(CurrencyTypeCompany currencyType)
        {
            if (ProcessingStrategies.TryGetValue(currencyType, out var strategy))
            {
                return strategy;
            }

            throw new ArgumentException("Invalid Currency Type");
        }
    }
}
