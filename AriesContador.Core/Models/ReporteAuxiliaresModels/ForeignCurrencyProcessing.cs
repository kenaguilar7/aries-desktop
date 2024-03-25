using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    internal class ForeignCurrencyProcessing : MAinContext
    {
        public override void Process(ref IXLWorksheet worksheet, ref int column, List<Account> combinedAccounts, Dictionary<PostingPeriod, Account[]> tableReport, out List<string> Headers)
        {
            SetAccountsNames(ref worksheet, 6, ref column, combinedAccounts);
            LlenarSaldoCuentasColones(ref worksheet, 6, column, tableReport, out Headers);
        }

        private void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, Dictionary<PostingPeriod, Account[]> lstFechas, out List<string> headers)
        {
            var rowFechas = row;
            List<string> _headers = new List<string>();

            foreach (var fecha in lstFechas)
            {
                _headers.Add($"{fecha.Key.ToString()}-USD");
                worksheet.Cell(rowFechas - 1, column).Value = fecha.Key.ToString();
                var rowMonto = rowFechas;
                foreach (var cuenta in fecha.Value)
                {
                    worksheet.Cell(rowMonto++, column).Value = cuenta.MontlyBalanceForeign;
                }
                column++;
            }

            headers = _headers;
        }
    }
}
