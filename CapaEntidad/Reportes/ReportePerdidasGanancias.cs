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

namespace CapaEntidad.Reportes
{
    public class ReportePerdidasGanancias
    {
        public static void GenerarReporte(String direccion, List<Cuenta> _lstExcel, Compañia compañia, Usuario usuario)
        {
            var maxValue = (from c in _lstExcel orderby c.GetNombreParaExcel(_lstExcel.ToList()).Length select new { top = c.GetNombreParaExcel(_lstExcel.ToList()).Length }).LastOrDefault();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");

                worksheet.Cell(1, 1).Value = $"{compañia} {compañia.Codigo}";
                worksheet.Cell(2, 1).Value = $"Reporte perdidas y ganacias";
                worksheet.Cell(4, 1).Value = usuario;

                ///Ponemos los titulos
                //worksheet.Cell(6, 1).Value = dataTable;
                worksheet.Cell(4, 1).Value = usuario;

                
                int cont = 5; 
                foreach (var cuenta in _lstExcel)
                {
                    if (cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                    {

                        foreach (var cntAux in _lstExcel)
                        {
                            if (cntAux.Indicador != IndicadorCuenta.Cuenta_Titulo && cntAux.TipoCuenta.TipoCuenta == cuenta.TipoCuenta.TipoCuenta)
                            {
                                
                                var name = cntAux.GetNombreParaExcel(_lstExcel.ToList());
                                worksheet.Cell(cont, name.Length).Value = cntAux.Nombre;
                                worksheet.Cell(cont, (maxValue.top*2)-1 - name.Length+1).Value = cntAux.SaldoActualColones;
                                cont++; 
                            }
                        }

                       
                        var nameTotal = cuenta.GetNombreParaExcel(_lstExcel.ToList());
                        worksheet.Cell(cont, nameTotal.Length).Value = $"TOTAL {cuenta.Nombre}"; 
                        worksheet.Cell(cont, (maxValue.top*2)-1).Value = cuenta.SaldoActualColones;
                        cont++; 
                        //rowTotal.Cells[nameTotal.Length - 1].Value = $"Total {nameTotal.Last()}";
                        //rowTotal.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //rowTotal.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = cuenta.SaldoActualColones;
                        //GridDatos.Rows.Add(rowTotal);
                    }
                }
                cont++;
                var ingreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Ingreso);
                var Egreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Egreso);
                var costoVenta = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Costo_Venta);

                worksheet.Cell(cont, 1).Value = "Total";
                worksheet.Cell(cont, (maxValue.top*2)-1).Value = (ingreso.SaldoActualColones-Egreso.SaldoActualColones);

                var range = worksheet.Range(5, maxValue.top + 1, _lstExcel.Count + 7, maxValue.top * 2);
                range.Style.NumberFormat.NumberFormatId = 4;

                workbook.SaveAs(direccion);
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });
            }

        }
    }
}
