using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Usuarios;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CapaEntidad.Reportes
{
    public class ReporteCompañia
    {

        public static void GenerarReporte(List<Compañia> list, String direccion, Usuario usuario)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");

                worksheet.Cell(1, 1).Value = $"Reporte de Compañias";
                worksheet.Cell(2, 1).Value = $"{usuario.MyNombre} {usuario.MyApellidoPaterno}-{usuario.UserName}";

                worksheet.Cell(3, 1).Value = $"Código";
                worksheet.Cell(3, 2).Value = $"Tipo de identificación";
                worksheet.Cell(3, 3).Value = $"Número de identificacion";
                worksheet.Cell(3, 4).Value = $"Nombre";
                worksheet.Cell(3, 5).Value = $"op1";
                worksheet.Cell(3, 6).Value = $"op2";
                worksheet.Cell(3, 7).Value = $"Direccion";
                worksheet.Cell(3, 8).Value = $"Direccion Web";
                worksheet.Cell(3, 9).Value = $"Correo Electronico";
                worksheet.Cell(3, 10).Value = $"Teléfono";
                worksheet.Cell(3, 11).Value = $"Teléfono";
                worksheet.Cell(3, 12).Value = $"Observaciones";
                worksheet.Cell(3, 13).Value = $"Moneda";
                worksheet.Cell(3, 14).Value = $"Activo";

                ///Corregimos op
                if (list.Count != 0)
                {
                    var anclassOp = (list[0] is PersonaFisica) ?
                        new { op1 = "Primer Apellido", op2 = "Segundo Apellido" } :
                        new { op1 = "Representante Legal", op2 = "Identificación Representante Legal" };

                    worksheet.Cell(3, 5).Value = anclassOp.op1;
                    worksheet.Cell(3, 6).Value = anclassOp.op2;
                }

                var row = 4;
                foreach (var compan in list)
                {
                    var colum = 1;

                    worksheet.Cell(row, colum++).Value = compan.Codigo;
                    worksheet.Cell(row, colum++).Value = compan.TipoId.ToString().Replace('_', ' ');
                    ///Daba un problema con los numero de cédula NITE y DIMEX los convertia en numeros 
                    //worksheet.Cell(row, colum).DataType = XLDataType.Text; 
                    ///Error aun no resuelto
                    worksheet.Cell(row, colum++).Value = compan.NumeroCedula;
                    worksheet.Cell(row, colum++).Value = compan.Nombre;
                    worksheet.Cell(row, colum++).Value = (compan is PersonaFisica) ? ((PersonaFisica)compan).MyApellidoPaterno : ((PersonaJuridica)compan).MyRepresentanteLegal;
                    worksheet.Cell(row, colum++).Value = (compan is PersonaFisica) ? ((PersonaFisica)compan).MyApellidoMaterno : ((PersonaJuridica)compan).MyIDRepresentanteLegal;
                    worksheet.Cell(row, colum++).Value = compan.Direccion;
                    worksheet.Cell(row, colum++).Value = compan.Web;
                    worksheet.Cell(row, colum++).Value = compan.Correo;
                    worksheet.Cell(row, colum++).Value = compan.Telefono[0];
                    worksheet.Cell(row, colum++).Value = compan.Telefono[1];
                    worksheet.Cell(row, colum++).Value = compan.Observaciones;
                    worksheet.Cell(row, colum++).Value = compan.TipoMoneda.ToString().Replace('_', ' ');
                    worksheet.Cell(row, colum++).Value = (compan.Activo) ? "Activa" : "Desactiva";
                    row++;

                }

                workbook.SaveAs(direccion);
                ////Show report
                Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });

            }

        }
    }
}
