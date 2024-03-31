using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    public class MultiCurrencyColonesProcessing : MAinContext
    {
        public override void Process(ref IXLWorksheet worksheet, ref int column, List<Account> combinedAccounts, Dictionary<PostingPeriod, Account[]> tableReport, out List<string> Headers)
        {
            SetAccountsNames(ref worksheet, 6, ref column, combinedAccounts);
            LlenarSaldoCuentasColonesDolares(ref worksheet, 7, column, tableReport, out Headers);
        }

        private static void LlenarSaldoCuentasColonesDolares(
           ref IXLWorksheet worksheet,
           int row,
           int column,
           Dictionary<PostingPeriod, Account[]> lstFechas,
           out List<string> headers)
        {

            var rowFechas = row;
            var columnaQuiebre = column;
            List<string> _headers = new List<string>();

            foreach (var fecha in lstFechas)
            {
                _headers.Add($"{fecha.Key.ToString()}-COL");
                _headers.Add($"{fecha.Key.ToString()}-USD");


                ///Se escribe la fecha en el exel 
                ///
                worksheet.Cell(rowFechas - 2, column).Value = fecha.Key.ToString();
                worksheet.Cell(rowFechas - 2, column).Style.NumberFormat.Format = "MMM yyyy";

                worksheet.Cell(rowFechas - 2, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(rowFechas - 2, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(rowFechas - 2, column, rowFechas - 2, column + 1).Merge();

                worksheet.Cell(rowFechas - 1, column).Value = "COL";
                worksheet.Cell(rowFechas - 1, column + 1).Value = "USD";


                ///Ahora imprimos las cuentas
                var rowMonto = rowFechas;
                foreach (var cuenta in fecha.Value)
                {
                    worksheet.Cell(rowMonto, column).Value = cuenta.MontlyBalance;
                    worksheet.Cell(rowMonto, column).Style.NumberFormat.NumberFormatId = 4;
                    worksheet.Cell(rowMonto, column + 1).Value = cuenta.MontlyBalanceForeign;
                    worksheet.Cell(rowMonto, column + 1).Style.NumberFormat.NumberFormatId = 4;
                    rowMonto++;
                }
                column += 2;
            }
            headers = _headers;
        }
    }
}
