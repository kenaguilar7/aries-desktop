using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Cuentas;
using System.Collections.Generic;
using CapaEntidad.Enumeradores;
using System.Windows.Forms;
using CapaEntidad.Textos;
using System.Data;
using System.Linq;
using CapaLogica;
using System;
using CapaEntidad.Reportes;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteBalanceSituacion : Form
    {
        private CuentaCL CuentaCL { get; } = new CuentaCL();
        private List<Cuenta> ListaCuentas { get; set; }
        private IEnumerable<Cuenta> ListaCuentasBalancePerdida { get; set; }
        private IEnumerable<Cuenta> ListaCuentasBalanceSitucion { get; set; }
        public ReporteBalanceSituacion()
        {
            InitializeComponent();
            CargarDatos();
        }


        private void CargarDatos()
        {
            ListaCuentas = CuentaCL.GetAll(GlobalConfig.Company);
            var lstDts = new FechaTransaccionCL().GetAllActive(GlobalConfig.Company, GlobalConfig.Usuario);

            AFechaFinal.DataSource = lstDts;


            var lstBfchFnl = new List<FechaTransaccion> { (from c1 in lstDts select c1).OrderByDescending(x => x.Fecha).LastOrDefault() };
            AFechaInicio.DataSource = lstBfchFnl;

            SetCuentasEnLista();

            CrearColumnasParaNombre();
        }

        private void SetCuentasEnLista()
        {
            ListaCuentasBalancePerdida = from c in ListaCuentas where c.TipoCuenta.TipoCuenta == TipoCuenta.Ingreso || c.TipoCuenta.TipoCuenta == TipoCuenta.Egreso || c.TipoCuenta.TipoCuenta == TipoCuenta.Costo_Venta select c;
            ListaCuentasBalanceSitucion = from c in ListaCuentas where c.TipoCuenta.TipoCuenta != TipoCuenta.Ingreso && c.TipoCuenta.TipoCuenta != TipoCuenta.Egreso && c.TipoCuenta.TipoCuenta != TipoCuenta.Costo_Venta select c;

        }
        private void BtnCalcular(object sender, EventArgs e)
        {
            DateTime fch1 = ((FechaTransaccion)AFechaInicio.SelectedItem).Fecha;
            DateTime fch2 = ((FechaTransaccion)AFechaFinal.SelectedItem).Fecha;



            //var cuentasSitucaion = ListaCuentas.;
            //var cuentasPerdida = lstPerdidas;
            CuentaCL.LLenarConSaldos(fch1, fch2, ListaCuentas, GlobalConfig.Company);
            //CuentaCL.LLenarConSaldoB(par1, par2, cuentasPerdida, GlobalConfig.Compañia);
            //if (!checkCuentasConSaldo.Checked)
            //{
            //    ListaCuentas =  new CuentaCL().QuitarCuentasSinSaldos(ListaCuentas);
            //}


            CargarFormulario();

        }

        private void CargarFormulario()
        {
            GridDatos.Rows.Clear();

            foreach (var cuenta in ListaCuentasBalanceSitucion)
            {
                if (cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {

                    foreach (var cntAux in ListaCuentas)
                    {
                        if (cntAux.Indicador != IndicadorCuenta.Cuenta_Titulo && cntAux.TipoCuenta.TipoCuenta == cuenta.TipoCuenta.TipoCuenta)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(GridDatos);
                            var name = cntAux.GetNombreParaExcel(ListaCuentas.ToList());
                            row.Cells[name.Length - 1].Value = name.Last();
                            row.Cells[GridDatos.Columns.Count - (name.Length)].Value = cntAux.SaldoActualColones;
                            GridDatos.Rows.Add(row);
                        }
                    }

                    if (cuenta.TipoCuenta.TipoCuenta == TipoCuenta.Patrimonio)
                    {

                        DataGridViewRow rowTotalPerdida = new DataGridViewRow();
                        rowTotalPerdida.CreateCells(GridDatos);

                        rowTotalPerdida.Cells[0].Value = $"UTILIDAD/PERDIDA PERIODO";
                        rowTotalPerdida.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        rowTotalPerdida.Cells[GridDatos.Columns.Count - 1].Value = TotalPerdida;

                        //GridDatos.Rows.Add(Perdida);
                        GridDatos.Rows.Add(rowTotalPerdida);

                        //patrimonio


                        DataGridViewRow rowTotalpatrimonio = new DataGridViewRow();
                        rowTotalpatrimonio.CreateCells(GridDatos);
                        var nameTotal = cuenta.GetNombreParaExcel(ListaCuentas.ToList());
                        rowTotalpatrimonio.Cells[nameTotal.Length - 1].Value = $"TOTAL {nameTotal.Last()}";
                        rowTotalpatrimonio.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        rowTotalpatrimonio.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = TotalPerdida + cuenta.SaldoActualColones;

                        //GridDatos.Rows.Add(Perdida);
                        GridDatos.Rows.Add(rowTotalpatrimonio);


                    }
                    else
                    {

                        DataGridViewRow rowTotal = new DataGridViewRow();
                        rowTotal.CreateCells(GridDatos);
                        var nameTotal = cuenta.GetNombreParaExcel(ListaCuentas.ToList());
                        rowTotal.Cells[nameTotal.Length - 1].Value = $"TOTAL {nameTotal.Last()}";
                        rowTotal.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                         rowTotal.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = cuenta.SaldoActualColones;

                        //GridDatos.Rows.Add(Perdida);
                        GridDatos.Rows.Add(rowTotal);
                    }
                }
            }
            DataGridViewRow rowTotalPasivoPatrimonio = new DataGridViewRow();
            rowTotalPasivoPatrimonio.CreateCells(GridDatos);

            rowTotalPasivoPatrimonio.Cells[0].Value = $"TOTAL PASIVO Y PATRIMONIO";
            rowTotalPasivoPatrimonio.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rowTotalPasivoPatrimonio.Cells[GridDatos.Columns.Count-1].Value = TotalPerdida + TotalSituacion;

            //GridDatos.Rows.Add(Perdida);
            GridDatos.Rows.Add(rowTotalPasivoPatrimonio);
            
        }
        private void CrearColumnasParaNombre()
        {

            var maxValue = (from c in ListaCuentas orderby c.GetNombreParaExcel(ListaCuentasBalanceSitucion.ToList()).Length select new { top = c.GetNombreParaExcel(ListaCuentas.ToList()).Length }).LastOrDefault();

            for (int i = 0; i < maxValue.top * 2; i++)
            {
                DataGridViewTextBoxColumn column1P = new DataGridViewTextBoxColumn
                {
                    HeaderText = "",
                    Name = $"columnN{i}",
                    ReadOnly = true
                };

                if (((maxValue.top * 2) / 2) > i)
                {
                    column1P.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else
                {
                    column1P.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column1P.DefaultCellStyle.Format = "###,##0.00";
                }
                GridDatos.Columns.Insert(i, column1P);
            }
            //dataGridViewCellStyle7.Format = "$### ###.00";
            //if (intrCont < cont)
            //{
            //    for (int i = 0; i < (cont - intrCont); i++)
            //    {
            //        GridDatos.Columns.Remove(GridDatos.Columns[i]);
            //    }

            //}
            //cont = intrCont;

        }

        private void tbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE BALANCE DE SITUACIÓN {GlobalConfig.Company.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteExcel.ReporteUtilidadPerdida(ConverRowsToExcel(), Encabezado(), sfd.FileName);
                        // CapaEntidad.Reportes.ReporteBalanceSituacion.GenerarReporte(sfd.FileName, ListaCuentasBalanceSitucion.ToList(),TotalPerdida,TotalSituacion,  GlobalConfig.Compañia, GlobalConfig.Usuario);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal TotalSituacion {
            get {
                ///Situcion
                //var activo = ListaCuentas.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Activo);
                var pasivo = ListaCuentas.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Pasivo);
                var patrimonio = ListaCuentas.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Patrimonio);

                return Decimal.Add( pasivo.SaldoActualColones , patrimonio.SaldoActualColones) ;  }
        }
        private decimal TotalPerdida {
            get {
                var ingreso = ListaCuentasBalancePerdida.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Ingreso);
                var Egreso = ListaCuentasBalancePerdida.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Egreso);
                var costoVenta = ListaCuentasBalancePerdida.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == TipoCuenta.Costo_Venta);
                
                return ingreso.SaldoActualColones - costoVenta.SaldoActualColones - Egreso.SaldoActualColones;
                

            }
        }

        public List<Object[]> ConverRowsToExcel()
        {

            var retorno = new List<object[]>();
            var dd = GridDatos.Rows;

            foreach (DataGridViewRow item in dd)
            {

                Object[] line = new object[item.Cells.Count];
                var cont = 0;

                foreach (DataGridViewCell cell in item.Cells)
                {
                    line[cont] = cell.Value ?? "";
                    cont++;
                }
                retorno.Add(line);

            }
            return retorno;
        }

        public String[] Encabezado()
        {

            return new string[]{

                $"{GlobalConfig.Company}",
                $"BALANCE DE SITUACION AL MES {(((FechaTransaccion)AFechaFinal.SelectedItem).ToString().ToUpper())}",
                $"EMITIDO POR {GlobalConfig.Usuario} ",
            };

        }

        private void checkCuentasConSaldo_CheckedChanged(object sender, EventArgs e)
        {
            CargarFormulario(); 
        }
    }
}
