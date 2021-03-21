using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
   
namespace CapaEntidad.Reportes
{
    public class ReporteExcel
    {
        public static void ReporteUtilidadPerdida(List<object[]> lista, String[] encabezado, String padf) {

            var workbook = new XLWorkbook();     //creates the workbook
            var ws = workbook.AddWorksheet("Reporte"); //creates the worksheet with sheetname 'data'

            
            ws.Cell(1, 1).InsertData(encabezado);
            ws.Cell(encabezado.Length+2, 1).InsertData(lista);
            //var ss = wsDetailedData.Tables;


            //workbook.Table("Table1").Theme = XLTableTheme.None; 


            workbook.SaveAs(padf); //saves the workbook
                                   ////Show report
            Process.Start(new ProcessStartInfo(padf) { UseShellExecute = true });

        }

    }
}
