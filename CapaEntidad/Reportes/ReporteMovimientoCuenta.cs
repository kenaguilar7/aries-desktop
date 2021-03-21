using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Usuarios;
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
    public class ReporteMovimientoCuenta
    {

        public static void GenerarReporte(String direccion, Boolean MostrarDolares, DataTable dataTable, Compañia compañia, Usuario usuario, Cuenta cuenta)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");

                worksheet.Cell(1, 1).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(2, 1).Value = $"Reporte movimientos de cuenta";
                worksheet.Cell(3, 1).Value = $"Cuenta: { cuenta.ToString()}";
                worksheet.Cell(4, 1).Value = usuario;

                ///Ponemos los titulos


                //worksheet.Cell(6, 1).Value = dataTable;

                worksheet.Cell(4, 1).Value = usuario;

                worksheet.Cell(5, 1).Value = "Nombre Cuenta";
                worksheet.Cell(5, 2).Value = "Tipo Movimiento";
                worksheet.Cell(5, 3).Value = "Detalle";
                worksheet.Cell(5, 4).Value = "Referencia";
                worksheet.Cell(5, 5).Value = "Fecha Factura";
                worksheet.Cell(5, 6).Value = "Mes Contable";
                worksheet.Cell(5, 7).Value = "Número Asiento";
                worksheet.Cell(5, 8).Value = "Debito";
                worksheet.Cell(5, 9).Value = "Credito";
                worksheet.Cell(5, 10).Value = "Saldo Actual";


                var indicecol = 11;
                if (MostrarDolares)
                {
                    worksheet.Cell(5, indicecol++).Value = "Tipo Cambio";
                    worksheet.Cell(5, indicecol++).Value = "Monto Dolares";

                }
                worksheet.Cell(5, indicecol++).Value = "Usuario";
                worksheet.Cell(5, indicecol++).Value = "Fecha Registro";

                var rngTitle = worksheet.Range("A5:P5");
                rngTitle.Style.Font.Bold = true;

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    //worksheet.Cell(6, 1).InsertData(Configurar(dataTable));

                    object[] vs = dataTable.Rows[i].ItemArray;
                    worksheet.Cell(6 + i, 1).Value = vs[0];
                    worksheet.Cell(6 + i, 2).Value = vs[1];
                    worksheet.Cell(6 + i, 3).Value = vs[2];
                    worksheet.Cell(6 + i, 4).Value = vs[3];
                    ///Fecha factura
                    worksheet.Cell(6 + i, 5).Value = vs[4];
                    worksheet.Cell(6 + i, 5).Style.NumberFormat.NumberFormatId = 14; 
                    ///mescontable
                    worksheet.Cell(6 + i, 6).Value = vs[5];
                    worksheet.Cell(6 + i, 6).Style.NumberFormat.NumberFormatId = 17;
                    //#asiento, debitos, creditos, saldo actual , tipo cambio, dolares
                    worksheet.Cell(6 + i, 7).Value = vs[6].ToString().Replace(",", string.Empty); ;
                    worksheet.Cell(6 + i, 8).Value = vs[7].ToString().Replace(",", string.Empty); ;
                    worksheet.Cell(6 + i, 9).Value = vs[8].ToString().Replace(",", string.Empty); ;
                    worksheet.Cell(6 + i, 10).Value = vs[9].ToString().Replace(",", string.Empty);

                    indicecol = 11;
                    if (MostrarDolares)
                    {

                        worksheet.Cell(6 + i, indicecol++).Value = vs[10].ToString().Replace(",", string.Empty); ;
                        worksheet.Cell(6 + i, indicecol++).Value = vs[11].ToString().Replace(",", string.Empty); ;

                    }
                    worksheet.Cell(6 + i, indicecol++).Value = vs[12];
                    worksheet.Cell(6 + i, indicecol++).Value = vs[13];

                    //var range5 = worksheet.Range(6 + i, 7, 6 + i, indicecol - 1);
                    //range5.Style.NumberFormat.NumberFormatId = 4;



                }
                var range = worksheet.Range(6, 8, dataTable.Rows.Count+6, (MostrarDolares)? 12: 10);
                range.Style.NumberFormat.Format = "#,##0.00";

                workbook.SaveAs(direccion);
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }

        }


    }
}
