using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Utils;

namespace CapaEntidad.Reportes
{
    public class ReporteAsiento
    {
        //public static void GenerarReporte(List<JournalEntry> list, Compañia compañia, Usuario usuario,
        //                          TipoMonedaCompañia tipoMoneda, String direccion, DataTable dataTable)
        //{

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Sample Sheet");
        //        ///Creamos el encabezado
        //        var column = 1;
        //        var row = 1;

        //        worksheet.Cell(row++, column).Value = $"{compañia} {compañia.Codigo}";
        //        worksheet.Cell(row++, column).Value = $"Reporte Asientos";
        //        worksheet.Cell(row++, column).Value = usuario;

        //        switch (tipoMoneda)
        //        {
        //            case TipoMonedaCompañia.Dolares_y_Colones:
        //                LlenarEncabezados(TipoMonedaCompañia.Dolares_y_Colones, ref worksheet, row, column);
        //                //LlenarSaldoAsientosColonesDolares(ref worksheet, 7, column, list);
        //                LlenarFromDataTable(dataTable, ref worksheet, row, column, compañia.TipoMoneda); 
        //                break;
        //            case TipoMonedaCompañia.Solo_Colones:
        //                LlenarEncabezados(TipoMonedaCompañia.Solo_Colones, ref worksheet, row, column);
        //                //LlenarSaldoCuentasColones(ref worksheet, 7, column, list);
        //               LlenarFromDataTable(dataTable, ref worksheet, row, column, compañia.TipoMoneda);
        //                break;
        //            case TipoMonedaCompañia.Solo_Dolares:
        //                LlenarEncabezados(TipoMonedaCompañia.Solo_Dolares, ref worksheet, row, column);
        //                //LlenarSaldoAsientosColonesDolares(ref worksheet, 7, column, list);
        //                LlenarFromDataTable(dataTable, ref worksheet, row, column, compañia.TipoMoneda);
        //                break;
        //            default:
        //                break;
        //        }

        //        workbook.SaveAs(direccion);
        //        ////Show report
        //        Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

        //    }

        //}
        //private static void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, List<JournalEntry> list)
        //{
        //    row = 5;
        //    foreach (var item in list)
        //    {
        //        for (int i = 0; i < item.JournalEntryLines.Count; i++)
        //        {

        //            //Fecha/mes
        //            worksheet.Cell(i + row, column).Value = item.PostingPeriodId;
        //            ///Número de asiento
        //            worksheet.Cell(i + row, column + 1).Value = item.Number;
        //            ///Cuenta 
        //            worksheet.Cell(i + row, column + 2).Value = item.JournalEntryLines[i].AccountId;
        //            ///Referencia
        //            worksheet.Cell(i + row, column + 3).Value = item.JournalEntryLines[i].Reference;
        //            ///Detalle
        //            worksheet.Cell(i + row, column + 4).Value = item.JournalEntryLines[i].Memo;
        //            ///fecha factura
        //            worksheet.Cell(i + row, column + 5).Value = item.JournalEntryLines[i].Date;
        //            ///debitos
        //            worksheet.Cell(i + row, column + 6).Value = (item.JournalEntryLines[i].DebOrCred == DebOrCred.Debito) ? item.JournalEntryLines[i].Monto : 0.00m;
        //            ///Creditos
        //            worksheet.Cell(i + row, column + 7).Value = (item.JournalEntryLines[i].DebOrCred == DebOrCred.Credito) ? item.JournalEntryLines[i].Monto : 0.00m;
        //            ///Como vamos a usar solo colones estos campos no apareceran
        //            //worksheet.Cell(i + row, column + 8).Value = item.Transaccions[i].TipoCambio.ToString();
        //            //worksheet.Cell(i + row, column + 9).Value = (item.Transaccions[i].TipoCambio == TipoCambio.Colones) ? " " : $"{item.Transaccions[i].MontoTipoCambio}";
        //            //worksheet.Cell(i + row, column + 10).Value = (item.Transaccions[i].TipoCambio == TipoCambio.Dolares) ? item.Transaccions[i].Monto / item.Transaccions[i].MontoTipoCambio : 0.00;


        //            worksheet.Cell(i + row, column).Style.NumberFormat.Format = "MMM yyyy";

        //            worksheet.Cell(i + row, column + 6).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(i + row, column + 7).Style.NumberFormat.Format = "₡#,##0.00";
        //            //worksheet.Cell(i + row, column + 9).Style.NumberFormat.Format = "₡#,##0.00";
        //            //worksheet.Cell(i + row, column + 10).Style.NumberFormat.Format = "$#,##0.00";
        //            //.Style.NumberFormat.Format = "MMM yyyy"; 

        //        }

        //        if (item.JournalEntryLines.Count != 0)
        //        {
        //            row += item.JournalEntryLines.Count();
        //            ///Totales
        //            worksheet.Cell(row, 1).Value = "Totales";
        //            worksheet.Cell(row, 7).Value = item.DebitosColones;
        //            worksheet.Cell(row, 8).Value = item.CreditosColones;
        //            //Formato
        //            worksheet.Cell(row, 7).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(row, 8).Style.NumberFormat.Format = "₡#,##0.00";

        //            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            worksheet.Range(row, 1, row, 6).Row(1).Merge();
        //            row++;
        //        }
        //    }
        //}
        //private static void LlenarSaldoAsientosColonesDolares(ref IXLWorksheet worksheet, int row, int column, List<JournalEntry> list)
        //{
        //    row = 5;
        //    foreach (var item in list)
        //    {
        //        for (int i = 0; i < item.JournalEntryLines.Count; i++)
        //        {

        //            //Saldo Anterior
        //            worksheet.Cell(i + row, column).Value = item.PostingPeriodId;
        //            worksheet.Cell(i + row, column + 1).Value = item.Number;
        //            worksheet.Cell(i + row, column + 2).Value = item.JournalEntryLines[i].AccountId;
        //            worksheet.Cell(i + row, column + 3).Value = item.JournalEntryLines[i].Reference;
        //            worksheet.Cell(i + row, column + 4).Value = item.JournalEntryLines[i].Memo;
        //            worksheet.Cell(i + row, column + 5).Value = item.JournalEntryLines[i].Date;
        //            worksheet.Cell(i + row, column + 6).Value = (item.JournalEntryLines[i].DebOrCred == DebOrCred.Debito) ? item.JournalEntryLines[i].Monto : 0.00m;
        //            worksheet.Cell(i + row, column + 7).Value = (item.JournalEntryLines[i].DebOrCred == DebOrCred.Credito) ? item.JournalEntryLines[i].Monto : 0.00m;
        //            worksheet.Cell(i + row, column + 8).Value = item.JournalEntryLines[i].Currency.ToString();
        //            worksheet.Cell(i + row, column + 9).Value = (item.JournalEntryLines[i].Currency == Currency.colones) ? " " : $"{item.JournalEntryLines[i].RateAmount}";
        //            worksheet.Cell(i + row, column + 10).Value = (item.JournalEntryLines[i].Currency == Currency.dolares) ? item.JournalEntryLines[i].Monto / item.JournalEntryLines[i].RateAmount : 0.00m;


        //            worksheet.Cell(i + row, column).Style.NumberFormat.Format = "MMM yyyy";

        //            worksheet.Cell(i + row, column + 6).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(i + row, column + 7).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(i + row, column + 9).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(i + row, column + 10).Style.NumberFormat.Format = "$#,##0.00";
        //            //.Style.NumberFormat.Format = "MMM yyyy"; 

        //        }

        //        if (item.JournalEntryLines.Count != 0)
        //        {
        //            row += item.JournalEntryLines.Count();
        //            ///Totales
        //            worksheet.Cell(row, 1).Value = "Totales";
        //            worksheet.Cell(row, 7).Value = item.DebitosColones;
        //            worksheet.Cell(row, 8).Value = item.CreditosColones;
        //            //Formato
        //            worksheet.Cell(row, 7).Style.NumberFormat.Format = "₡#,##0.00";
        //            worksheet.Cell(row, 8).Style.NumberFormat.Format = "₡#,##0.00";

        //            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            worksheet.Range(row, 1, row, 6).Row(1).Merge();
        //            row++;
        //        }
        //    }


        //}
        //private static void LlenarEncabezados(TipoMonedaCompañia tipoCambio, ref IXLWorksheet worksheet, int row, int column)
        //{

        //    var quiebreNombreColumna = 1;
        //    //Mes
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Mes";
        //    //Moneda Colones
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Numero Asiento";
        //    //Cuenta       
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Cuenta";
        //    //Referencia   
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Referencia";
        //    //Detalle      
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Detalle";
        //    //Fecha factura                       
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Fecha de factura";
        //    //Debitos                             
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Debitos";
        //    //Credito                             
        //    worksheet.Cell(4, quiebreNombreColumna++).Value = "Creditos";

        //    if (tipoCambio != TipoMonedaCompañia.Solo_Colones)
        //    {
        //        //Moneda                              
        //        worksheet.Cell(4, quiebreNombreColumna++).Value = "Moneda";
        //        //Tipo Cambio                         
        //        worksheet.Cell(4, quiebreNombreColumna++).Value = "Tipo Cambio";
        //        //Monto dolares                       
        //        worksheet.Cell(4, quiebreNombreColumna++).Value = "Monto Dolares";
        //    }
        //}

        //private static void LlenarFromDataTable(DataTable dataTable, ref IXLWorksheet worksheet, int row, int column, TipoMonedaCompañia monedaCompañia)
        //{
        //    row++; 
        //    foreach (DataRow item in dataTable.Rows)
        //    {
        //        Object[] vs = item.ItemArray;
        //        worksheet.Cell(row, 1).Value = vs[0];
        //        worksheet.Cell(row, 2).Value = vs[1];
        //        worksheet.Cell(row, 3).Value = vs[2];
        //        worksheet.Cell(row, 4).Value = vs[3];
        //        worksheet.Cell(row, 5).Value = vs[4];
        //        worksheet.Cell(row, 6).Value = vs[5];
        //        worksheet.Cell(row, 7).Value = vs[6];
        //        worksheet.Cell(row, 8).Value = vs[7];
        //        if (monedaCompañia != TipoMonedaCompañia.Solo_Colones)
        //        {
        //            worksheet.Cell(row, 9).Value = vs[8];
        //            worksheet.Cell(row, 10).Value = vs[9];
        //            worksheet.Cell(row, 11).Value = vs[10];
        //        }
        //        row++; 
        //    }

        //}
    }
}
