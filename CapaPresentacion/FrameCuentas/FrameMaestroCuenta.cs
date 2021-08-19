using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Textos;
using CapaPresentacion.cods;
using CapaPresentacion.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaPresentacion.Utils;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameMaestroCuenta : Form, ICallingForm
    {
        private List<PostingPeriod> PostingPeriods { get; set; } 
        private Account CurrentAccount { get; set; }
        private List<Account> CompanyAccounts { get; set; }
        private readonly IFinancialService _financialService;
        public FrameMaestroCuenta()
        {
            InitializeComponent();

            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
        }

        private void FrameMaestroCuenta_Load(object sender, EventArgs e)
        {
            LoadAccounts();
            CargarDatosAListas();
            //treeCuentas.ExpandAll(); 
        }


        public bool TransferirCuenta(Account cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, CompanyAccounts);
                return true;
            }
            return false;
        }

    
        private void LoadAccounts() // refactored
        {
            CompanyAccounts?.Clear();
            CompanyAccounts = _financialService.GetAccounts(GlobalConfig.Company.Codigo).ToList(); 
            treeCuentas.Nodes.AddRange(CompanyAccounts.BuildTreeView());
        }

        private void CargarDatosAListas()
        {
            AFechaFinal.SelectedIndexChanged -= this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged -= this.BFechaFinalSelectedIndexChanged;

            PostingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList(); 

            var lstBfchFnl = new List<PostingPeriod> { (from c1 in PostingPeriods select c1).OrderByDescending(x => x.Date).LastOrDefault() };
            AFechaInicio.DataSource = lstBfchFnl;
            AFechaFinal.DataSource = (from c1 in PostingPeriods select c1).ToList();
            AFechaFinal.SelectedIndex = -1;

            BFechaInicio.DataSource = (from c1 in PostingPeriods select c1).ToList();
            BFechaFinal.DataSource = (from c1 in PostingPeriods where c1.Date >= ((PostingPeriod)BFechaInicio.SelectedItem).Date select c1).ToList();
            BFechaFinal.SelectedIndex = -1;

            
            AFechaFinal.SelectedIndexChanged += this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged += this.BFechaFinalSelectedIndexChanged;

        }

        private void LoadAccountInfoToDashboard()// refactored
        {
            try
            {
                this.txtNombreInfo.Text = CurrentAccount.Name;
                this.txtTipoInfo.Text = CurrentAccount.AccountTag.ToString().Replace('_', ' ');
                this.txtIndicadorInfo.Text = CurrentAccount.AccountType.ToString().Replace('_', ' ');
                this.txtBoxDetalle.Text = CurrentAccount.Memo;
                this.infoPanel.Tag = CurrentAccount;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message ,TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildAccountBalanceInformationDashboard()
        {
            var startMonth = GetSeletedMonthStartDate(); 
            var endMonth = GetSelectedMonthEndDate();

            try
            {
                if (startMonth != null && endMonth != null && CurrentAccount != null)
                {
                    var accountWithAmount = _financialService.
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private PostingPeriod GetSeletedMonthStartDate()
        {
            if(gridDatosA.Visible) return AFechaInicio.SelectedItem as PostingPeriod ?? null;
            return BFechaInicio.SelectedItem as PostingPeriod ?? null;
        }

        private PostingPeriod GetSelectedMonthEndDate()
        {
            if (gridDatosA.Visible) return AFechaFinal.SelectedItem as PostingPeriod;
            return BFechaFinal.SelectedItem as PostingPeriod;
        }

        private void CargarDatosPanelA()
        {

            //if (AFechaFinal.SelectedIndex == -1)
            //{
            //    return;
            //}
            //#region Obtenemos las Fechas
            //DateTime fch1 = ((FechaTransaccion)AFechaInicio.SelectedItem).Fecha;
            //DateTime fch2 = ((FechaTransaccion)AFechaFinal.SelectedItem).Fecha;
            //DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            //DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            /////Ponemos el ultimo dia del mes en la fecha final
            //par2 = (par2.AddMonths(1)).AddDays(-1);
            //#endregion
            /////vamos a hacer un metodo que solo mande la cuenta con sus hijas y los datos de la cuenta que devulva 
            /////van a ser los utilizados

            ////Solo vamos a llenar las cuentas que se seleccione, de esta manera evitamos traer
            ////todos los datos t mejoramos rendimiento

            //// var lstcntshjs = TreeViewCuentas.GetCuentasHIjas(CuentaActual, _lstCuentas);

            ////_cuentaCL.LLenarConSaldos(par1, par2, _lstCuentas, GlobalConfig.Company);
            ////TreeCuentasAfterSelect(null, null);
            //CargarGridA();
        }
        private void CargarDatosPanelB()
        {
            //if (BFechaFinal.SelectedIndex == -1)
            //{
            //    return;
            //}
            //#region Obtenemos las Fechas
            //DateTime fch1 = ((FechaTransaccion)BFechaInicio.SelectedItem).Fecha;
            //DateTime fch2 = ((FechaTransaccion)BFechaFinal.SelectedItem).Fecha;
            //DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            //DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            /////Ponemos el ultimo dia del mes en la fecha final
            //par2 = (par2.AddMonths(1)).AddDays(-1);
            //#endregion
            //// var lstcntshjs = TreeViewCuentas.GetCuentasHIjas(CuentaActual, _lstCuentas);
            ////_cuentaCL.LLenarConSaldos(par1, par2, _lstCuentas, GlobalConfig.Company);

            //CargarGridB();

        }

        private void CargarGridA()
        {
            //if (CuentaActual is null)
            //{
            //    return;
            //}
            //gridDatosA.Rows.Clear();
            //DataGridViewRow row = new DataGridViewRow();
            //row.CreateCells(gridDatosA);
            //row.Cells[0].Value = CuentaActual.SaldoAnteriorColones;
            //row.Cells[1].Value = CuentaActual.DebitosColones;
            //row.Cells[2].Value = CuentaActual.CreditosColones;
            //row.Cells[3].Value = CuentaActual.SaldoActualColones;
            //gridDatosA.Rows.Add(row);
        }
        private void CargarGridB()
        {
            //if (CuentaActual is null)
            //{
            //    return;
            //}
            //gridDatosB.Rows.Clear();
            //DataGridViewRow row = new DataGridViewRow();
            //row.CreateCells(gridDatosB);

            //row.Cells[0].Value = CuentaActual.DebitosColones;
            //row.Cells[1].Value = CuentaActual.CreditosColones;
            //row.Cells[2].Value = CuentaActual.SaldoMensualColones;

            //gridDatosB.Rows.Add(row);
        }
    

        #region Eventos

        private void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.txtNombreInfo.ReadOnly = true;
            this.txtBoxDetalle.ReadOnly = true;
            this.btnGuardarNuevoNombre.Enabled = false;
            this.btnGuardarNuevoNombre.Visible = false;
            CurrentAccount = (Account)e.Node.Tag;
            LoadAccountInfoToDashboard();
            BuildAccountBalanceInformationDashboard(); 
            //if (tabControlGeneral.SelectedIndex == 0)
            //{
            //    CargarGridA();
            //}
            //else
            //{
            //    CargarGridB();
            //}
        }
        /// <summary>
        /// Actualiza el nombre a de la cuenta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarNuevoNombre(object sender, EventArgs e)
        {
            //try
            //{


            //    Cuenta cEdita = ((Cuenta)treeCuentas.SelectedNode.Tag);


            //    cEdita.Detalle = this.txtBoxDetalle.Text;

            //    if (_cuentaCL.Update(ref cEdita, GlobalConfig.Usuario, txtNombreInfo.Text, GlobalConfig.Company, txtBoxDetalle.Text, out String mensaje))
            //    {
            //        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        treeCuentas.SelectedNode.Text = cEdita.Nombre;

            //        this.btnGuardarNuevoNombre.Enabled = false;
            //        this.btnGuardarNuevoNombre.Visible = false;

            //        this.txtNombreInfo.ReadOnly = true;
            //        this.txtBoxDetalle.ReadOnly = true;

            //    }
            //    else
            //    {
            //        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        private void LstMesInicioSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 1)
            {

                BFechaFinal.DataSource = (from n in PostingPeriods where n.Date >= ((PostingPeriod)BFechaInicio.SelectedItem).Date select n).ToList<PostingPeriod>();
            }
        }

        private void TabControlGeneralSelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAccountBalanceInformationDashboard(); 
        }

        private void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAccountBalanceInformationDashboard(); 
            //CargarDatosPanelA();
        }

        private void BFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAccountBalanceInformationDashboard();
            //CargarDatosPanelB();
        }

        private void Eliminar_Click(object sender, EventArgs e)
        {
            //if (CuentaActual is null)
            //{
            //    MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            //    return;
            //}

            //if (MessageBox.Show("Esta acción no se puede deshacer ¿desea continuar de todos modos?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{

            //    if (_cuentaCL.Deleted(CuentaActual, GlobalConfig.Usuario, out String mensaje))
            //    {
            //        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        CompanyAccounts.Remove(CuentaActual);
            //        var padre = treeCuentas.SelectedNode.Parent;
            //        treeCuentas.Nodes.Remove(treeCuentas.SelectedNode);
            //        treeCuentas.SelectedNode = padre;
            //    }
            //    else
            //    {
            //        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //    }
            //}

        }

        #endregion

        #region Llamada a otras ventanas
        /// <summary>
        /// Abre la ventana para listar las cuentas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listar(object sender, EventArgs e)
        {
            ReporteCuenta reporte = new ReporteCuenta(GlobalConfig.Company, GlobalConfig.Usuario);
            reporte.MdiParent = this.MdiParent;
            reporte.Show();

        }
        private void CrearNuevaCuenta(object sender, EventArgs e) //Refactored
        {
            try
            {
                var selectedAccountOnNode = treeCuentas.SelectedNode?.Tag as Account;

                if (selectedAccountOnNode != null && selectedAccountOnNode.Editable)
                {
                    FrameNuevaCuenta nv = new FrameNuevaCuenta(this, selectedAccountOnNode);
                    nv.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Esta cuenta no puede ser editada.", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Evento que ocurre cuando se presiona el boton de editar una cuenta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditarCuenta(object sender, EventArgs e)
        {
            //try
            //{
            //    if (CuentaActual != null && CuentaActual.Editable)
            //    {
            //        // var cuenta = (Cuenta)infoPanel.Tag;
            //        this.txtNombreInfo.ReadOnly = false;
            //        this.txtNombreInfo.Focus();
            //        this.txtBoxDetalle.ReadOnly = false;
            //        VisualizarOpcionesDeEdicion();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Esta cuenta no puede ser editada", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
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
        /// <summary>
        /// Metodo para establecer los valores de los controles del panel editar cuenta
        /// </summary>
        /// <param name="tag"></param>
        private void VisualizarOpcionesDeEdicion()
        {

            btnGuardarNuevoNombre.Enabled = true;
            btnGuardarNuevoNombre.Visible = true;
        }
        /// <summary>
        /// Si el usuario presiona enter el sistema lo convierte en tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            ReporteMovimientosCuenta frame = new ReporteMovimientosCuenta();
            if (frame.TransferirCuenta(CurrentAccount))
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
