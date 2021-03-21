using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CapaEntidad.Reportes
{
    public class ReporteBalanceComprobacion
    {

        public static void GenerarReporte(List<Cuenta> list, Compañia compañia, Usuario usuario,
                                          TipoMonedaCompañia tipoMoneda, String direccion)
        {

            ///Generamos el encabezado

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;
                var tpmda = (compañia.TipoMoneda == TipoMonedaCompañia.Solo_Dolares) ? "Dolares y Colones" : "Colones";
                worksheet.Cell(row++, column).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(row++, column).Value = $"Balance Comprobación en {tpmda} al mes de";
                worksheet.Cell(row++, column).Value = usuario;

                switch (tipoMoneda)
                {
                    case TipoMonedaCompañia.Dolares_y_Colones:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, list);
                        LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColones(ref worksheet, 6, column, list);
                        break;
                    case TipoMonedaCompañia.Solo_Colones:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, list);
                        LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColones(ref worksheet, 6, column, list);
                        break;
                    case TipoMonedaCompañia.Solo_Dolares:
                        LLenarNombreCuentas(ref worksheet, 6, ref column, list);
                        LlenarTitulosAmbasDivisas(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColonesDolares(ref worksheet, 7, column, list);
                        break;
                    default:
                        break;
                }


                // var direccion = @"balance_comprobacion_" + compañia.MyNombre.Replace(' ', '_') + ".xlsx"; 
                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });


                /**
                 *      LLenarNombreCuentas(ref worksheet, 5, ref column, list);
                        LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColones(ref worksheet, 6, column, list);
                 */


            }

        }
        private static void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {
            for (int i = 0; i < list.Count; i++)
            {

                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoAnteriorColones.ToString() : "";
                worksheet.Cell(i + row, column + 1).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoAnteriorColones.ToString() : "";
                //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;


                //Movimiento Mensual
                worksheet.Cell(i + row, column + 2).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoMensualColones.ToString() : "";
                worksheet.Cell(i + row, column + 3).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoMensualColones.ToString() : "";
                // formato Saldo Moviento Mensual
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;

                //Saldo actual
                worksheet.Cell(i + row, column + 4).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoActualColones.ToString() : "";
                worksheet.Cell(i + row, column + 5).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoActualColones.ToString() : "";
                //Saldo Actual Formato
                worksheet.Cell(i + row, column + 4).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 5).Style.NumberFormat.NumberFormatId = 4;
            }
            ///Ponemos totales
            ///Ponemos titulo total
            worksheet.Cell(row + list.Count, 1).Value = "Total";
            worksheet.Range(row + list.Count, 1, row + list.Count, column - 1).Merge();

            ///Buscamos los totales de debito
            var totalAnteiorColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoAnteriorColones);

            var totalBalanceActualColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoMensualColones);

            var totalActualColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoActualColones);

            ///Buscamos los totales de credito
            var totalAnteiorColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                .Sum(c => c.SaldoAnteriorColones);

            var totalBalanceActualColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoMensualColones);

            var totalActualColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoActualColones);

            worksheet.Cell(row + list.Count, column).Value = totalAnteiorColonesDebito;
            worksheet.Cell(row + list.Count, column).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 1).Value = totalAnteiorColonesCredito;
            worksheet.Cell(row + list.Count, column + 1).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 2).Value = totalBalanceActualColonesDebito;
            worksheet.Cell(row + list.Count, column + 2).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 3).Value = totalBalanceActualColonesCredito;
            worksheet.Cell(row + list.Count, column + 3).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 4).Value = totalActualColonesDebito;
            worksheet.Cell(row + list.Count, column + 4).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 5).Value = totalActualColonesCredito;
            worksheet.Cell(row + list.Count, column + 5).Style.NumberFormat.NumberFormatId = 4;
        }
        private static void LlenarSaldoCuentasDolares(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {
            for (int i = 0; i < list.Count; i++)
            {

                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoAnteriorDolares.ToString() : "";
                worksheet.Cell(i + row, column + 1).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoAnteriorDolares.ToString() : "";
                //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;


                //Movimiento Mensual
                worksheet.Cell(i + row, column + 2).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoMensualDolares.ToString() : "";
                worksheet.Cell(i + row, column + 3).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoMensualDolares.ToString() : "";
                // formato Saldo Moviento Mensual
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;

                //Saldo actual
                worksheet.Cell(i + row, column + 4).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoActualDolares.ToString() : "";
                worksheet.Cell(i + row, column + 5).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoActualDolares.ToString() : "";
                //Saldo Actual Formato
                worksheet.Cell(i + row, column + 4).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 5).Style.NumberFormat.NumberFormatId = 4;
            }
            ///Ponemos totales
            ///Ponemos titulo total
            worksheet.Cell(row + list.Count, 1).Value = "Total";
            worksheet.Range(row + list.Count, 1, row + list.Count, column - 1).Merge();

            ///Buscamos los totales de debito
            var totalAnteiorDolaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoAnteriorDolares);

            var totalBalanceActualDalaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoMensualDolares);

            var totalActualDalaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoActualDolares);

            ///Buscamos los totales de credito
            var totalAnteiorDolaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                .Sum(c => c.SaldoAnteriorDolares);

            var totalBalanceActualDalaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoMensualDolares);

            var totalActualDalaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoActualDolares);

            worksheet.Cell(row + list.Count, column).Value = totalAnteiorDolaresDebito;
            worksheet.Cell(row + list.Count, column).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 1).Value = totalAnteiorDolaresCredito;
            worksheet.Cell(row + list.Count, column + 1).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 2).Value = totalBalanceActualDalaresDebito;
            worksheet.Cell(row + list.Count, column + 2).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 3).Value = totalBalanceActualDalaresCredito;
            worksheet.Cell(row + list.Count, column + 3).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 4).Value = totalActualDalaresDebito;
            worksheet.Cell(row + list.Count, column + 4).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 5).Value = totalActualDalaresCredito;
            worksheet.Cell(row + list.Count, column + 5).Style.NumberFormat.NumberFormatId = 4;


        }
        private static void LlenarSaldoCuentasColonesDolares(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {

            for (int i = 0; i < list.Count; i++)
            {

                //Saldo Anterior
                worksheet.Cell(i + row, column).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoAnteriorColones.ToString() : "";
                worksheet.Cell(i + row, column + 1).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoAnteriorColones.ToString() : "";
                worksheet.Cell(i + row, column + 2).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoAnteriorDolares.ToString() : "";
                worksheet.Cell(i + row, column + 3).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoAnteriorDolares.ToString() : "";

                //Saldo Anterior Formato
                worksheet.Cell(i + row, column).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 1).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 2).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 3).Style.NumberFormat.NumberFormatId = 4;




                ////Movimiento Mensual
                worksheet.Cell(i + row, column + 4).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoMensualColones.ToString() : "";
                worksheet.Cell(i + row, column + 5).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoMensualColones.ToString() : "";
                worksheet.Cell(i + row, column + 6).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoMensualDolares.ToString() : "";
                worksheet.Cell(i + row, column + 7).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoMensualDolares.ToString() : "";

                // formato Saldo Moviento Mensual
                worksheet.Cell(i + row, column + 4).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 5).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 6).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 7).Style.NumberFormat.NumberFormatId = 4;



                //Saldo actual
                worksheet.Cell(i + row, column + 8).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoActualColones.ToString() : "";
                worksheet.Cell(i + row, column + 9).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoActualColones.ToString() : "";
                worksheet.Cell(i + row, column + 10).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Debito) ? list[i].SaldoActualDolares.ToString() : "";
                worksheet.Cell(i + row, column + 11).Value = (list[i].TipoCuenta.Comportamiento == Comportamiento.Credito) ? list[i].SaldoActualDolares.ToString() : "";

                //Saldo Actual
                worksheet.Cell(i + row, column + 8).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 9).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 10).Style.NumberFormat.NumberFormatId = 4;
                worksheet.Cell(i + row, column + 11).Style.NumberFormat.NumberFormatId = 4;
            }

            ///Ponemos totales
            ///Ponemos titulo total
            worksheet.Cell(row + list.Count, 1).Value = "Total";
            worksheet.Range(row + list.Count, 1, row + list.Count, column - 1).Merge();

            ///Buscamos los totales de debito Colones
            ///
            var totalAnteiorColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoAnteriorColones);

            var totalBalanceActualColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoMensualColones);

            var totalActualColonesDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoActualColones);

            ///Buscamos los totales de credito Colones
            var totalAnteiorColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                .Sum(c => c.SaldoAnteriorColones);

            var totalBalanceActualColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoMensualColones);

            var totalActualColonesCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoActualColones);

            ///Buscamos los totales de debito Dolares
            var totalAnteiorDolaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoAnteriorDolares);

            var totalBalanceActualDalaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoMensualDolares);

            var totalActualDalaresDebito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Debito))
                      .Sum(c => c.SaldoActualDolares);

            ///Buscamos los totales de credito
            var totalAnteiorDolaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                .Sum(c => c.SaldoAnteriorDolares);

            var totalBalanceActualDalaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoMensualDolares);

            var totalActualDalaresCredito = list.Where(c => (c.Indicador == IndicadorCuenta.Cuenta_Auxiliar) && (c.TipoCuenta.Comportamiento == Comportamiento.Credito))
                      .Sum(c => c.SaldoActualDolares);

            ///Monstramos las sumatoriasd
            ///Saldo Anterior
            worksheet.Cell(row + list.Count, column).Value = totalAnteiorColonesDebito;
            worksheet.Cell(row + list.Count, column + 1).Value = totalAnteiorColonesCredito;
            worksheet.Cell(row + list.Count, column + 2).Value = totalAnteiorDolaresDebito;
            worksheet.Cell(row + list.Count, column + 3).Value = totalAnteiorDolaresCredito;

            worksheet.Cell(row + list.Count, column).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 1).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 2).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 3).Style.NumberFormat.NumberFormatId = 4;


            ///balance Actual 
            worksheet.Cell(row + list.Count, column + 4).Value = totalBalanceActualColonesDebito;
            worksheet.Cell(row + list.Count, column + 5).Value = totalBalanceActualColonesCredito;
            worksheet.Cell(row + list.Count, column + 6).Value = totalBalanceActualDalaresDebito;
            worksheet.Cell(row + list.Count, column + 7).Value = totalBalanceActualDalaresCredito;

            worksheet.Cell(row + list.Count, column + 4).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 5).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 6).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 7).Style.NumberFormat.NumberFormatId = 4;

            ///Saldo Actual
            worksheet.Cell(row + list.Count, column + 8).Value = totalActualColonesDebito;
            worksheet.Cell(row + list.Count, column + 9).Value = totalActualColonesCredito;
            worksheet.Cell(row + list.Count, column + 10).Value = totalActualDalaresDebito;
            worksheet.Cell(row + list.Count, column + 11).Value = totalActualDalaresCredito;

            worksheet.Cell(row + list.Count, column + 8).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 9).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 10).Style.NumberFormat.NumberFormatId = 4;
            worksheet.Cell(row + list.Count, column + 11).Style.NumberFormat.NumberFormatId = 4;

        }
        private static void LlenarTitulosAmbasDivisas(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {

            var quiebreNombreColumna = column;
            //Saldo Anterior
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldos Anteriores";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 3).Row(1).Merge();
            //Moneda Colones
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Row(1).Merge();
            //MOneda Dolares
            worksheet.Cell(5, quiebreNombreColumna + 2).Value = "Dolares";
            worksheet.Cell(5, quiebreNombreColumna + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna + 2, 5, quiebreNombreColumna + 3).Row(1).Merge();
            //Debitos y creditos
            worksheet.Cell(6, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 1).Value = "Creditos";
            worksheet.Cell(6, quiebreNombreColumna + 2).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 3).Value = "Creditos";


            quiebreNombreColumna = quiebreNombreColumna + 4;

            //Saldo Mensual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Movimiento Mensual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 3).Row(1).Merge();
            //Moneda Colones
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Row(1).Merge();
            //MOneda Dolares
            worksheet.Cell(5, quiebreNombreColumna + 2).Value = "Dolares";
            worksheet.Cell(5, quiebreNombreColumna + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna + 2, 5, quiebreNombreColumna + 3).Row(1).Merge();
            //Debitos y creditos
            worksheet.Cell(6, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 1).Value = "Creditos";
            worksheet.Cell(6, quiebreNombreColumna + 2).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 3).Value = "Creditos";

            quiebreNombreColumna = quiebreNombreColumna + 4;

            //Saldo Actual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Actual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 3).Row(1).Merge();
            //Moneda Colones
            worksheet.Cell(5, quiebreNombreColumna).Value = "Colones";
            worksheet.Cell(5, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna, 5, quiebreNombreColumna + 1).Row(1).Merge();
            //MOneda Dolares
            worksheet.Cell(5, quiebreNombreColumna + 2).Value = "Dolares";
            worksheet.Cell(5, quiebreNombreColumna + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(5, quiebreNombreColumna + 2, 5, quiebreNombreColumna + 3).Row(1).Merge();
            //Debitos y creditos
            worksheet.Cell(6, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 1).Value = "Creditos";
            worksheet.Cell(6, quiebreNombreColumna + 2).Value = "Debitos";
            worksheet.Cell(6, quiebreNombreColumna + 3).Value = "Creditos";

        }
        private static void LlenarTitulosUnaDivisa(ref IXLWorksheet worksheet, int row, int column, List<Cuenta> list)
        {

            var quiebreNombreColumna = column;
            //Saldo Anterior
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldos Anteriores";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();

            //Debitos y creditos
            worksheet.Cell(5, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Creditos";



            quiebreNombreColumna = quiebreNombreColumna + 2;

            //Movimiento Mensual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Movimiento Mensual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();

            //Debitos y creditos
            worksheet.Cell(5, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Creditos";



            quiebreNombreColumna = quiebreNombreColumna + 2;

            //Saldo Actual
            worksheet.Cell(4, quiebreNombreColumna).Value = "Saldo Actual";
            worksheet.Cell(4, quiebreNombreColumna).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(4, quiebreNombreColumna, 4, quiebreNombreColumna + 1).Row(1).Merge();

            //Debitos y creditos
            worksheet.Cell(5, quiebreNombreColumna).Value = "Debitos";
            worksheet.Cell(5, quiebreNombreColumna + 1).Value = "Creditos";




        }
        private static void LLenarNombreCuentas(ref IXLWorksheet worksheet, int row, ref int column, List<Cuenta> list)
        {
            //Le sumamos dos 
            //va a ser el rango para poner el titulo
            //De cuentas

            var quiebreNombreColumna = column;
            var quiebreRow = row;
            for (int i = 0; i < list.Count; i++)
            {
                var nombre = list[i].GetNombreParaExcel(list);

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
