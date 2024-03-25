using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace AriesContador.Core.Models.ReporteAuxiliaresModels
{
    public interface ICurrencyProcessing
    {
        void Process(ref IXLWorksheet worksheet,
            ref int column,
            List<Account> combinedAccounts,
            Dictionary<PostingPeriod, Account[]> tableReport,
            out List<string> Headers);
    }


    public abstract class MAinContext : ICurrencyProcessing
    {
        public abstract void Process(ref IXLWorksheet worksheet, ref int column, List<Account> combinedAccounts, Dictionary<PostingPeriod, Account[]> tableReport, out List<string> Headers);

        protected void SetAccountsNames(ref IXLWorksheet worksheet, int row, ref int column, List<Account> list)
        {
            var quiebreNombreColumna = column;
            var quiebreRow = row;
            for (int i = 0; i < list.Count; i++)
            {
                var nombre = list[i].GetPathName(list);

                for (int j = 0; j < nombre.Length; j++)
                {
                    worksheet.Cell(i + row + 1, j + column).Value = nombre[j];
                    //hacemos una sumatoria del punto de quiebre 
                    //para obtener el mayor rago
                    //solo sumanos si es mayor
                    quiebreNombreColumna = (quiebreNombreColumna < j + column) ? j + column : quiebreNombreColumna;
                }

                quiebreRow = (quiebreRow < i + row) ? i + row : quiebreRow;
            }
            worksheet.Cell("A4").Value = "Cuentas";
            worksheet.Range("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, 1, row, quiebreNombreColumna).Merge();
            column = quiebreNombreColumna + 1;
        }
    }

}
