using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    internal class LocalCurrencyProcessing : MAinContext
    {
        public override void Process(ref IXLWorksheet worksheet, ref int column, List<Account> combinedAccounts, Dictionary<PostingPeriod, Account[]> tableReport, out List<string> Headers)
        {
            SetAccountsNames(ref worksheet, 6, ref column, combinedAccounts);
            LlenarSaldoCuentasColones(ref worksheet, 6, column, tableReport, out Headers); 
        }

        private void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, Dictionary<PostingPeriod, Account[]> lstFechas, out List<string> Headers)
        {
            var rowFechas = row + 1;
            List<string> header = new List<string>();

            foreach (var fecha in lstFechas)
            {
                header.Add($"{fecha.Key.ToString()}-COL");
                worksheet.Cell(rowFechas - 1, column).Value = fecha.Key.ToString();
                worksheet.Cell(rowFechas - 1, column).Style.NumberFormat.Format = "MMM yyyy";

                var rowMonto = rowFechas;

                foreach (var cuenta in fecha.Value)
                {
                    var rowNumber = rowMonto++; 
                    worksheet.Cell(rowNumber, column).Value = cuenta.MontlyBalance;
                    worksheet.Cell(rowNumber, column).Style.NumberFormat.NumberFormatId = 4;
                }
                column++;
            }
            Headers = header;
        }
    }
}
