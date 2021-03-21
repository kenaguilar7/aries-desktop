using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Reportes
{
    public class ReporteMaestroCuenta
    {
        public static void GenerarReporte(List<Cuenta> list, String direccion, Compañia compañia, Usuario usuario,
                                          FechaTransaccion mesFinal, Boolean GenerarSaldo = false)
        {


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;
                var tpmda = (compañia.TipoMoneda == TipoMonedaCompañia.Solo_Dolares) ? "Dolares y Colones" : "Colones"; 
                worksheet.Cell(row++, column).Value = $"{compañia}";
                worksheet.Cell(row++, column).Value = $"Maestro de Cuentas en {tpmda},  a {mesFinal}";
                worksheet.Cell(row++, column).Value = $"usuario {usuario.ToString()}"; 


                LLenarNombreCuentas(ref worksheet, row, ref column, list);


                if (GenerarSaldo)
                {
                    switch (compañia.TipoMoneda)
                    {
                        case TipoMonedaCompañia.Dolares_y_Colones:
                            LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                            LlenarSaldoCuentasColones(ref worksheet, row, column, list);
                            break;
                        case TipoMonedaCompañia.Solo_Colones:
                            LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                            LlenarSaldoCuentasColones(ref worksheet, row, column, list);
                            break;
                        case TipoMonedaCompañia.Solo_Dolares:
                            LlenarTitulosAmbasDivisas(ref worksheet, row, column, list);
                            LlenarSaldoCuentasColonesDolares(ref worksheet, row, column, list);
                            break;
                        default:
                            break;
                    }
                }


                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }

        }
        public static void GenerarReporteCuentas(List<Cuenta> list, String direccion, Compañia compañia, Usuario usuario)
        {

            if (compañia.TipoMoneda == TipoMonedaCompañia.Dolares_y_Colones)
            {
                GenerarReporteAmbasDivisas(list, direccion, compañia, usuario);
                return;
            }

            char divisa = (compañia.TipoMoneda == TipoMonedaCompañia.Solo_Colones) ? '₡' : '$';
            divisa = (compañia.TipoMoneda == TipoMonedaCompañia.Solo_Dolares) ? '$' : divisa;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;

                worksheet.Cell(row++, column).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(row++, column).Value = $"Maestro de Cuentas";
                worksheet.Cell(row++, column).Value = usuario;
                worksheet.Cell(row++, column).Value = "Cuentas";

                row = 5;
                var quiebreNombreColumna = 3;///esta es la ultima fila
                var quiebreRow = 3;

                for (int i = 0; i < list.Count; i++)
                {
                    var nombre = list[i].GetNombreParaExcel(list);

                    for (int j = 0; j < nombre.Length; j++)
                    {
                        worksheet.Cell(i + row, j + column).Value = nombre[j];
                        //hacemos una sumatoria del punto de quiebre 
                        //para obtener el mayor rago
                        //solo sumanos si es mayor
                        quiebreNombreColumna = (quiebreNombreColumna < j + column) ? j + column : quiebreNombreColumna;
                    }

                    quiebreRow = (quiebreRow < i + row) ? i + row : quiebreRow;
                }
                ///Es mas uno porque el queda en la linea n y la siguiente linea es a escribir es en la que 
                ///quedo mas una 

                quiebreNombreColumna++;

                //Ya que tenemos la cantidad de cuentas 
                //Creamose el resto del encabezado
                worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Anterior";
                worksheet.Cell(4, quiebreNombreColumna + 1).Value = "Debitos";
                worksheet.Cell(4, quiebreNombreColumna + 2).Value = "Creditos";
                worksheet.Cell(4, quiebreNombreColumna + 3).Value = "Saldo Actual";

                column = quiebreNombreColumna;

                row = 5;

                for (int i = 0; i < list.Count; i++)
                {
                    worksheet.Cell(i + row, column).Value = $"{divisa}{list[i].SaldoAnteriorColones}"; //Saldo anterior
                    worksheet.Cell(i + row, column + 1).Value = $"{divisa}{list[i].DebitosColones}"; //debitos
                    worksheet.Cell(i + row, column + 2).Value = $"{divisa}{list[i].CreditosColones}"; //creditos
                    worksheet.Cell(i + row, column + 3).Value = $"{divisa}{list[i].SaldoActualColones}"; //Saldo Actual
                }


                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }


        }
        private static void GenerarReporteAmbasDivisas(List<Cuenta> list, String direccion, Compañia compañia, Usuario usuario)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;

                worksheet.Cell(row++, column).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(row++, column).Value = $"Maestro de Cuentas";
                worksheet.Cell(row++, column).Value = usuario;

                row = 6;
                var quiebreNombreColumna = 3;///esta es la ultima fila
                var quiebreRow = 3;



                for (int i = 0; i < list.Count; i++)
                {
                    var nombre = list[i].GetNombreParaExcel(list);

                    for (int j = 0; j < nombre.Length; j++)
                    {
                        worksheet.Cell(i + row, j + column).Value = nombre[j];
                        //hacemos una sumatoria del punto de quiebre 
                        //para obtener el mayor rago
                        //solo sumanos si es mayor
                        quiebreNombreColumna = (quiebreNombreColumna < j + column) ? j + column : quiebreNombreColumna;
                    }

                    quiebreRow = (quiebreRow < i + row) ? i + row : quiebreRow;
                }
                column = quiebreNombreColumna + 1;//guardamos para cuando vayamos a llenar los saldos

                //Creamos los titulos una vez que ya sepamos 
                //El ancho total
                //Cuentas
                worksheet.Cell("A4").Value = "Cuentas";
                worksheet.Range("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(4, 1, 5, quiebreNombreColumna).Merge();
                quiebreNombreColumna++;

                //Saldo Anterior
                worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Anterior";
                worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
                //Moneda 
                worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
                worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
                quiebreNombreColumna = quiebreNombreColumna + 2;


                //Debitos
                worksheet.Cell(4, quiebreNombreColumna).Value = "Debitos";
                worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
                //Monedas
                worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
                worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
                quiebreNombreColumna = quiebreNombreColumna + 2;


                //Creditos
                worksheet.Cell(4, quiebreNombreColumna).Value = "Creditos";
                worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
                //Monedas
                worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
                worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
                quiebreNombreColumna = quiebreNombreColumna + 2;


                //Saldo Actual
                worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Actual";
                worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
                //Monedas
                worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
                worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";


                ///Es mas uno porque el queda en la linea n y la siguiente linea es a escribir es en la que 
                ///quedo mas una 

                quiebreNombreColumna++;




                row = 6;

                LlenarSaldoCuentasColonesDolares(ref worksheet, row, column, list);

                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }


        }
        private static void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {
            row = 6;

            for (int i = 0; i < list.Count; i++)
            {
                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = $"{list[i].SaldoAnteriorColones}"; //Saldo anterior
                //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;

                //Debitos
                worksheet.Cell(i + row, column + 1).Value = $"{list[i].DebitosColones}"; //debitos Colones
                //Debitos Formato
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;
                
                //Creditos
                worksheet.Cell(i + row, column + 2).Value = $"{list[i].CreditosColones}"; //creditos Colones
                //Creditos Formato
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;

                //Saldo Actual
                worksheet.Cell(i + row, column + 3).Value = $"{list[i].SaldoActualColones}"; //Saldo Actual Colones
                //Saldo Acutual Formato
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;

            }
        }
        private static void LlenarSaldoCuentasDolares(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {
            row = 6;
            for (int i = 0; i < list.Count; i++)
            {
                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = $"{list[i].SaldoAnteriorDolares}"; //Saldo Anterior Dolres
                                                                                           //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;

                //Debitos
                worksheet.Cell(i + row, column + 1).Value = $"{list[i].DebitosDolares}"; //debitos Dolares
                //Debitos Formato
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;

                //Creditos
                worksheet.Cell(i + row, column + 2).Value = $"{list[i].DebitosDolares}"; //creditos Dolares
                //Creditos Formato
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;

                //Saldo Actual
                worksheet.Cell(i + row, column + 3).Value = $"{list[i].CreditosDolares}"; //Saldo Actual Dolares
                //Saldo Acutual Formato
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;

            }
        }
        private static void LlenarSaldoCuentasColonesDolares(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {
            row = 6;
            for (int i = 0; i < list.Count; i++)
            {
                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = $"{list[i].SaldoAnteriorColones}"; //Saldo anterior
                worksheet.Cell(i + row, column + 1).Value = $"{list[i].SaldoAnteriorDolares}"; //Saldo Anterior Dolres
                                                                                               //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;

                //Debitos
                worksheet.Cell(i + row, column + 2).Value = $"{list[i].DebitosColones}"; //debitos Colones
                worksheet.Cell(i + row, column + 3).Value = $"{list[i].DebitosDolares}"; //debitos Dolares
                                                                                         //Debitos Formato
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;

                //Creditos
                worksheet.Cell(i + row, column + 4).Value = $"{list[i].CreditosColones}"; //creditos Colones
                worksheet.Cell(i + row, column + 5).Value = $"{list[i].DebitosDolares}"; //creditos Dolares
                                                                                         //Creditos Formato
                worksheet.Cell(i + row, column + 4).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 5).Style.NumberFormat.NumberFormatId = 4;

                //Saldo Actual
                worksheet.Cell(i + row, column + 6).Value = $"{list[i].CreditosColones}"; //Saldo Actual Colones
                worksheet.Cell(i + row, column + 7).Value = $"{list[i].CreditosDolares}"; //Saldo Actual Dolares
                                                                                          //Saldo Acutual Formato
                worksheet.Cell(i + row, column + 6).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 7).Style.NumberFormat.NumberFormatId = 4;
            }
        }
        private static void LlenarTitulosAmbasDivisas(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {

            var quiebreNombreColumna = column;
            //Saldo Anterior
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Anterior";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
            //Moneda 
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
            quiebreNombreColumna = quiebreNombreColumna + 2;


            //Debitos
            worksheet.Cell(4, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
            //Monedas
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
            quiebreNombreColumna = quiebreNombreColumna + 2;


            //Creditos
            worksheet.Cell(4, quiebreNombreColumna).Value = "Creditos";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
            //Monedas
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";
            quiebreNombreColumna = quiebreNombreColumna + 2;


            //Saldo Actual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Actual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();
            //Monedas
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Dolares";

        }
        private static void LlenarTitulosUnaDivisa(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {

            var quiebreNombreColumna = column;
            //Saldo Anterior
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Anterior";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Column(1).Merge();
            quiebreNombreColumna++;
            //Debitos
            worksheet.Cell(4, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Column(1).Merge();
            quiebreNombreColumna++;
            //Creditos
            worksheet.Cell(4, quiebreNombreColumna).Value = "Creditos";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Column(1).Merge();
            quiebreNombreColumna++;
            //Saldo Actual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Actual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Column(1).Merge();

        }
        private static void LLenarNombreCuentas(ref IXLWorksheet worksheet, int row, ref int column, List<Cuenta> list)
        {
            //Le sumamos dos 
            //va a ser el rango para poner el titulo
            //De cuentas
            row = row + 2;
            var quiebreNombreColumna = column;
            var quiebreRow = row;
            for (int i = 0; i < list.Count; i++)
            {
                var nombre = list[i].GetNombreParaExcel(list);

                for (int j = 0; j < nombre.Length; j++)
                {
                    worksheet.Cell(i + row, j + column).Value = nombre[j];
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
            worksheet.Range(4, 1, 5, quiebreNombreColumna).Merge();
            column = quiebreNombreColumna + 1;
        }


    }
}
