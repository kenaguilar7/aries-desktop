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
        //{ CurrencyTypeCompany.Solo_Colones, new SoloColonesProcessing() },
        //{ CurrencyTypeCompany.Solo_Dolares, new SoloDolaresProcessing() }
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

    //public class SoloColonesProcessing : ICurrencyProcessing
    //{
    //    public void Process(ref Worksheet worksheet, int startColumn, ref int column, List<Account> combinedAccounts, DataTable tableReport, out List<string> Headers)
    //    {
    //        // Lógica específica para Solo_Colones
    //    }
    //}

    //public class SoloDolaresProcessing : ICurrencyProcessing
    //{
    //    public void Process(ref Worksheet worksheet, int startColumn, ref int column, List<Account> combinedAccounts, DataTable tableReport, out List<string> Headers)
    //    {
    //        // Lógica específica para Solo_Dolares
    //    }
    //}

}
