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

namespace CapaEntidad.Reportes
{
    public class ReporteAuxiliares
    {


        public static void GenerarReporte(Dictionary<FechaTransaccion, List<Cuenta>> lstCuentas, Compañia compañia, Usuario usuario,
                          TipoMonedaCompañia tipoMoneda, String direccion)
        {


            var nuevaLista = new List<Cuenta>();
            ///Esta list guarda todas las cuentas 
            ///osea guarda la cuenta aunque solo haya tenido movientos un solo mes. 

            foreach (var item in lstCuentas)
            {
                foreach (var item2 in item.Value)
                {
                    if (!nuevaLista.Exists(cc => cc.Id == item2.Id))
                    {
                        nuevaLista.Add(item2);
                    }
                }
            }

            nuevaLista = Ordernar(nuevaLista);


            var tablaCuentas = new Dictionary<FechaTransaccion, Cuenta[]>();

            foreach (var mes in lstCuentas)
            {
                ///Ya tenemos la fecha
                var m = mes.Key;
                Cuenta[] cuentasConSaldo = new Cuenta[nuevaLista.Count];

                ///nuevaLista.CopyTo(cuentasConSaldo);

                ((from c in nuevaLista
                  select new Cuenta(c.Nombre, c.Id, c.Padre)).ToList<Cuenta>()).CopyTo(cuentasConSaldo);



                ///Ya creamos la copia
                ///con diferente apuntadores de memoria

                ///Recorremos toda la lista que acabamos de crear 
                ///y buscamos en los elementos de este mes la cuenta que haga math

                foreach (var ccc in cuentasConSaldo)
                {
                    var cc2 = mes.Value.Find(cv => cv.Id == ccc.Id);
                    if (cc2 != null)
                    {
                        ///Copiamos todas la informacion
                        ///
                        ccc.TipoCuenta = cc2.TipoCuenta;
                        ccc.SaldoAnteriorColones = cc2.SaldoAnteriorColones;
                        ccc.SaldoAnteriorDolares = cc2.SaldoAnteriorDolares;
                        ccc.DebitosColones = cc2.DebitosColones;
                        ccc.CreditosColones = cc2.CreditosColones;
                        ccc.DebitosDolares = cc2.DebitosDolares;
                        ccc.CreditosDolares = cc2.CreditosDolares;

                    }
                    else
                    {
                        
                        ccc.TipoCuenta = Cuenta.GenerarTipoCuenta(1);
                        ccc.SaldoAnteriorColones = 0.00m;
                        ccc.SaldoAnteriorDolares = 0.00m;
                        ccc.DebitosColones = 0.00m;
                        ccc.CreditosColones = 0.00m;
                        ccc.DebitosDolares = 0.00m;
                        ccc.CreditosDolares = 0.00m;

                    }
                }

                ///Agregamos a la lista
                tablaCuentas.Add(mes.Key, cuentasConSaldo);
            }



            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;
                
                worksheet.Cell(row++, column).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(row++, column).Value = $"Balance Auxiliares";
                worksheet.Cell(row++, column).Value = usuario;
                switch (tipoMoneda)
                {
                    case TipoMonedaCompañia.Dolares_y_Colones:
                        LLenarNombreCuentas(ref worksheet, 6, ref column, nuevaLista);
                        //LlenarTitulosAmbasDivisas(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColonesDolares(ref worksheet, 7, column, tablaCuentas);
                        break;
                    case TipoMonedaCompañia.Solo_Colones:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, nuevaLista);
                        //LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        LlenarSaldoCuentasColones(ref worksheet, 6, column, tablaCuentas);

                        break;
                    case TipoMonedaCompañia.Solo_Dolares:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, nuevaLista);
                        //LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        //LlenarSaldoCuentasDolares(ref worksheet, 6, column, tablaCuentas);
                        LlenarSaldoCuentasColonesDolares(ref worksheet, 7, column, tablaCuentas);

                        break;
                    default:
                        break;
                }


                // var direccion = @"balance_comprobacion_" + compañia.MyNombre.Replace(' ', '_') + ".xlsx"; 
                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }

        }
        private static void LlenarSaldoCuentasColones(ref IXLWorksheet worksheet, int row, int column, Dictionary<FechaTransaccion, Cuenta[]> lstFechas)
        {
            var rowFechas = row;
            var columnaQuiebre = column;

            foreach (var fecha in lstFechas)
            {
                ///Se escribe la fecha en el exel 
                ///
                worksheet.Cell(rowFechas - 1, column).Value = fecha.Key.ToString();
                ///worksheet.Cell(rowFechas - 1, column).Style.NumberFormat.Format = "MMM yyyy";
                ///Ahora imprimos las cuentas
                var rowMonto = rowFechas;
                foreach (var cuenta in fecha.Value)
                {
                    worksheet.Cell(rowMonto++, column).Value = cuenta.SaldoMensualColones;

                }
                column++;
            }

        }
        private static void LlenarSaldoCuentasDolares(ref IXLWorksheet worksheet, int row, int column, Dictionary<FechaTransaccion, Cuenta[]> lstFechas)
        {
            var rowFechas = row;
            var columnaQuiebre = column;

            foreach (var fecha in lstFechas)
            {
                ///Se escribe la fecha en el exel 
                ///
                worksheet.Cell(rowFechas - 1, column).Value = fecha.Key;
                worksheet.Cell(rowFechas - 1, column).Style.NumberFormat.Format = "MMM yyyy";
                ///Ahora imprimos las cuentas
                var rowMonto = rowFechas;
                foreach (var cuenta in fecha.Value)
                {
                    worksheet.Cell(rowMonto++, column).Value = cuenta.SaldoMensualDolares;
                    worksheet.Cell(rowMonto++, column).Style.NumberFormat.NumberFormatId = 4;

                }
                column++;
            }
        }
        private static void LlenarSaldoCuentasColonesDolares(ref IXLWorksheet worksheet, int row, int column, Dictionary<FechaTransaccion, Cuenta[]> lstFechas)
        {

            var rowFechas = row;
            var columnaQuiebre = column;

            foreach (var fecha in lstFechas)
            {
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
                    worksheet.Cell(rowMonto, column).Value = cuenta.SaldoMensualColones;
                    worksheet.Cell(rowMonto, column).Style.NumberFormat.NumberFormatId = 4;
                    worksheet.Cell(rowMonto, column + 1).Value = cuenta.SaldoMensualDolares;
                    worksheet.Cell(rowMonto, column + 1).Style.NumberFormat.NumberFormatId = 4;
                    rowMonto++;
                }
                column = column + 2;
            }
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

        private static List<Cuenta> Ordernar(List<Cuenta> lst)
        {

            List<Cuenta> retorno = new List<Cuenta>();

            ///Recorremos la lista para obtener solamente 
            ///Las cuentas guias
            foreach (Cuenta item in lst)
            {
                if (item.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    ///Por cada cuenta guia buscamos su hija
                    CargarNodos(item);
                }
            }

            ///buscamos las cuentas hijas 
            void CargarNodos(Cuenta cuenta)
            {
                ///toda cuenta que sea pasada por parametro sera agregada 
                ///de tal modo que siempre que se encuentre una hija
                ///esta sera agregada usara recursividas y si encuentra mas 
                ///el proceso se repetira
                retorno.Add(cuenta);

                var sql = from c in lst where c.Padre == cuenta.Id select c;

                ///devuelve todas las cuentas hijas
                var cueHijas = sql.ToArray<Cuenta>();

                ///recorremos todas
                foreach (Cuenta item in cueHijas)
                {
                    ///Usamos recursividad para buscar todas las cuentas hijas
                    CargarNodos(item);
                }
            }

            return retorno;
        }
    }
}
