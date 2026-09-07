using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Mappers;
using CapaEntidad.Textos;
using CapaPresentacion.cods;
using CapaPresentacion.Reportes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameMaestroCuenta : Form, ICallingForm
    {
        private readonly IFinancialService _financialService;
        private List<Cuenta> _lstCuentas { get; set; } = new List<Cuenta>();
        private List<FechaTransaccion> _lstFechas { get; set; } = new List<FechaTransaccion>();
        private Cuenta CuentaActual { get; set; }

        public FrameMaestroCuenta(IFinancialService financialService)
        {
            _financialService = financialService;
            InitializeComponent();
            CargarDatos();
            CargarDatosAListas();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitToWorkingArea();
            BeginInvoke(new Action(AdjustSplitter));
        }

        private void FitToWorkingArea()
        {
            Size available;
            if (MdiParent != null)
            {
                var mdiClient = MdiParent.Controls.OfType<MdiClient>().FirstOrDefault();
                available = mdiClient != null ? mdiClient.ClientSize : MdiParent.ClientSize;
            }
            else
            {
                available = Screen.FromControl(this).WorkingArea.Size;
            }

            if (Width > available.Width || Height > available.Height)
                WindowState = FormWindowState.Maximized;
        }

        private void AdjustSplitter()
        {
            var width = splitContainer1.Width;
            if (width <= 0)
                return;

            var minLeft = splitContainer1.Panel1MinSize;
            var minRight = splitContainer1.Panel2MinSize;
            var maxLeft = width - minRight - splitContainer1.SplitterWidth;
            if (maxLeft < minLeft)
                return;

            var desired = (int)(width * 0.55);
            splitContainer1.SplitterDistance = Math.Max(minLeft, Math.Min(desired, maxLeft));
        }

        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, _lstCuentas);
                return true;
            }
            else { return false; }
        }

        #region Carga de datos
        private void CargarDatos()
        {
            _lstCuentas.Clear();
            _lstCuentas = CuentaMapper.ToCuentaList(
                _financialService.GetAccounts(GlobalConfig.Company.Code),
                GlobalConfig.Company);
            treeCuentas.Nodes.AddRange(TreeViewCuentas.CrearTreeView(_lstCuentas));
        }

        private void CargarDatosAListas()
        {
            AFechaFinal.SelectedIndexChanged -= this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged -= this.BFechaFinalSelectedIndexChanged;

            _lstFechas = CuentaMapper.ToFechaTransaccionList(
                _financialService.GetPostingPeriods(GlobalConfig.Company.Code));
            var lstBfchFnl = new List<FechaTransaccion> { (from c1 in _lstFechas select c1).OrderByDescending(x => x.Fecha).LastOrDefault() };
            AFechaInicio.DataSource = lstBfchFnl;
            AFechaFinal.DataSource = (from c1 in _lstFechas select c1).ToList();
            AFechaFinal.SelectedIndex = -1;

            BFechaInicio.DataSource = (from c1 in _lstFechas select c1).ToList();
            if (BFechaInicio.SelectedItem is FechaTransaccion inicio)
            {
                BFechaFinal.DataSource = (from c1 in _lstFechas where c1.Fecha >= inicio.Fecha select c1).ToList();
            }
            BFechaFinal.SelectedIndex = -1;

            AFechaFinal.SelectedIndexChanged += this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged += this.BFechaFinalSelectedIndexChanged;
        }

        private void CargarDatosAlPanelDeInformacion()
        {
            try
            {
                this.txtNombreInfo.Text = CuentaActual.Nombre;
                this.txtTipoInfo.Text = CuentaActual.TipoCuenta.TipoCuenta.ToString().Replace('_', ' ');
                this.txtIndicadorInfo.Text = CuentaActual.Indicador.ToString().Replace('_', ' ');
                this.txtBoxDetalle.Text = CuentaActual.Detalle;
                this.infoPanel.Tag = CuentaActual;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void FillBalances(DateTime par1, DateTime par2)
        {
            var accounts = _lstCuentas.Select(CuentaMapper.ToAccount).ToList();
            _financialService.FillAccountsWithBalances(accounts, par1, par2);
            CuentaMapper.CopyBalancesToCuentas(accounts, _lstCuentas);
        }

        private void CargarDatosPanelA()
        {
            if (AFechaFinal.SelectedIndex == -1)
            {
                return;
            }
            DateTime fch1 = ((FechaTransaccion)AFechaInicio.SelectedItem).Fecha;
            DateTime fch2 = ((FechaTransaccion)AFechaFinal.SelectedItem).Fecha;
            DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            par2 = (par2.AddMonths(1)).AddDays(-1);
            FillBalances(par1, par2);
            CargarGridA();
        }

        private void CargarDatosPanelB()
        {
            if (BFechaFinal.SelectedIndex == -1)
            {
                return;
            }
            DateTime fch1 = ((FechaTransaccion)BFechaInicio.SelectedItem).Fecha;
            DateTime fch2 = ((FechaTransaccion)BFechaFinal.SelectedItem).Fecha;
            DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            par2 = (par2.AddMonths(1)).AddDays(-1);
            FillBalances(par1, par2);
            CargarGridB();
        }
        #endregion

        #region CargarDatosAlGrid
        private void CargarGridA()
        {
            if (CuentaActual is null)
            {
                return;
            }
            gridDatosA.Rows.Clear();
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(gridDatosA);
            row.Cells[0].Value = CuentaActual.SaldoAnteriorColones;
            row.Cells[1].Value = CuentaActual.DebitosColones;
            row.Cells[2].Value = CuentaActual.CreditosColones;
            row.Cells[3].Value = CuentaActual.SaldoActualColones;
            gridDatosA.Rows.Add(row);
        }
        private void CargarGridB()
        {
            if (CuentaActual is null)
            {
                return;
            }
            gridDatosB.Rows.Clear();
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(gridDatosB);

            row.Cells[0].Value = CuentaActual.DebitosColones;
            row.Cells[1].Value = CuentaActual.CreditosColones;
            row.Cells[2].Value = CuentaActual.SaldoMensualColones;

            gridDatosB.Rows.Add(row);
        }
        #endregion

        #region Eventos
        private void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.txtNombreInfo.ReadOnly = true;
            this.txtBoxDetalle.ReadOnly = true;
            this.btnGuardarNuevoNombre.Enabled = false;
            this.btnGuardarNuevoNombre.Visible = false;
            CuentaActual = (Cuenta)e.Node.Tag;
            CargarDatosAlPanelDeInformacion();

            if (tabControlGeneral.SelectedIndex == 0)
            {
                CargarGridA();
            }
            else
            {
                CargarGridB();
            }
        }

        private void GuardarNuevoNombre(object sender, EventArgs e)
        {
            try
            {
                Cuenta cEdita = ((Cuenta)treeCuentas.SelectedNode.Tag);
                cEdita.Detalle = this.txtBoxDetalle.Text;
                cEdita.Nombre = txtNombreInfo.Text;
                var account = CuentaMapper.ToAccount(cEdita);
                account.UpdatedBy = GlobalConfig.Usuario.Id;
                _financialService.UpdateAccount(account);
                MessageBox.Show(AccountRules.UpdateSuccessMessage, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                treeCuentas.SelectedNode.Text = cEdita.Nombre;

                this.btnGuardarNuevoNombre.Enabled = false;
                this.btnGuardarNuevoNombre.Visible = false;

                this.txtNombreInfo.ReadOnly = true;
                this.txtBoxDetalle.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void LstMesInicioSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 1 && BFechaInicio.SelectedItem is FechaTransaccion inicio)
            {
                BFechaFinal.DataSource = (from n in _lstFechas where n.Fecha >= inicio.Fecha select n).ToList<FechaTransaccion>();
            }
        }
        private void TabControlGeneralSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 0)
            {
                CargarDatosPanelA();
            }
            else
            {
                CargarDatosPanelB();
            }
        }
        private void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            CargarDatosPanelA();
        }
        private void BFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            CargarDatosPanelB();
        }
        private void Eliminar_Click(object sender, EventArgs e)
        {
            if (CuentaActual is null)
            {
                MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (MessageBox.Show("Esta acción no se puede deshacer ¿desea continuar de todos modos?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var account = CuentaMapper.ToAccount(CuentaActual);
                    account.UpdatedBy = GlobalConfig.Usuario.Id;
                    _financialService.DeleteAccount(account);
                    MessageBox.Show(AccountRules.DeleteSuccessMessage, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _lstCuentas.Remove(CuentaActual);
                    var padre = treeCuentas.SelectedNode.Parent;
                    treeCuentas.Nodes.Remove(treeCuentas.SelectedNode);
                    treeCuentas.SelectedNode = padre;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        #endregion

        #region Llamada a otras ventanas
        private void Listar(object sender, EventArgs e)
        {
            ReporteCuenta reporte = new ReporteCuenta(GlobalConfig.Company, GlobalConfig.Usuario, _financialService);
            reporte.MdiParent = this.MdiParent;
            reporte.Show();
        }
        private void CrearNuevaCuenta(object sender, EventArgs e)
        {
            try
            {
                if (treeCuentas.SelectedNode is null)
                {
                    MessageBox.Show("Seleccione una cuenta ", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (!(treeCuentas.SelectedNode.Tag is Cuenta cuenta) || cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    MessageBox.Show("No se puede crear cuentas en este nivel", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    FrameNuevaCuenta nv = new FrameNuevaCuenta(this, cuenta, _financialService);
                    nv.lstCuentas = _lstCuentas;
                    nv.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditarCuenta(object sender, EventArgs e)
        {
            try
            {
                if (CuentaActual != null && CuentaActual.Editable)
                {
                    this.txtNombreInfo.ReadOnly = false;
                    this.txtNombreInfo.Focus();
                    this.txtBoxDetalle.ReadOnly = false;
                    VisualizarOpcionesDeEdicion();
                }
                else
                {
                    MessageBox.Show("Esta cuenta no puede ser editada", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Metodos poco importante
        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ExpandirArbol(object sender, EventArgs e)
        {
            treeCuentas.ExpandAll();
        }
        private void ColapsarArbol(object sender, EventArgs e)
        {
            treeCuentas.CollapseAll();
        }
        private void VisualizarOpcionesDeEdicion()
        {
            btnGuardarNuevoNombre.Enabled = true;
            btnGuardarNuevoNombre.Visible = true;
        }
        private void UsuarioKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        private void btnMovimientosCuenta_Click(object sender, EventArgs e)
        {
            ReporteMovimientosCuenta frame = new ReporteMovimientosCuenta(
                GlobalConfig.Services.GetRequiredService<IFinancialReportService>());
            if (frame.TransferirCuenta(CuentaActual))
            {
                frame.MdiParent = this.MdiParent;
                frame.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una cuenta valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        #endregion
    }
}
