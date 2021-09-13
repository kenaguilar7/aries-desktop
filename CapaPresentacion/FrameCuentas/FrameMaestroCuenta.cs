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
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaPresentacion.Utils;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameMaestroCuenta : Form, ICallingForm
    {
        private List<PostingPeriod> PostingPeriods { get; set; } 
        private Account CurrentAccount { get; set; }
        private List<Account> CompanyAccounts { get; set; }
        private IEnumerable<Account> AccountsWithBalance { get; set;  }
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
            AFechaInicio.SelectedIndexChanged -= this.AFechaInicio_SelectedIndexChanged;
            AFechaFinal.SelectedIndexChanged -= this.AFechaFinalSelectedIndexChanged; 

            PostingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList(); 
            AFechaInicio.DataSource = PostingPeriods.DeepClone();
            AFechaFinal.DataSource = PostingPeriods.DeepClone();
            AFechaFinal.SelectedIndex = -1;

            AFechaInicio.SelectedIndexChanged += this.AFechaInicio_SelectedIndexChanged;
            AFechaFinal.SelectedIndexChanged += this.AFechaFinalSelectedIndexChanged;
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
            var startMonth = AFechaInicio.SelectedItem as PostingPeriod ?? null;
            var endMonth = AFechaFinal.SelectedItem as PostingPeriod ?? null;

            if (startMonth != null && endMonth != null && CurrentAccount != null)
            {
                var requestParam = new BasicReportParam()
                {
                    CompanyId = GlobalConfig.Company.Codigo,
                    FirstDate = startMonth.Date.BuildDateToParts(),
                    EndDate = endMonth.Date.BuildDateToParts(),
                };

                AccountsWithBalance = _financialService.GetAccountsBalance(requestParam);
            }
        }

        #region Eventos

        private void AFechaInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(AFechaInicio.SelectedItem is PostingPeriod selectedPostingP)
                AFechaFinal.DataSource = PostingPeriods.GetOlder(selectedPostingP.Date); 
        }

        private void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.txtNombreInfo.ReadOnly = true;
            this.txtBoxDetalle.ReadOnly = true;
            this.btnGuardarNuevoNombre.Enabled = false;
            this.btnGuardarNuevoNombre.Visible = false;
            CurrentAccount = (Account)e.Node.Tag;
            LoadAccountInfoToDashboard();
            LoadAccountBalanceToDashBoard();
        }

        private void LoadAccountBalanceToDashBoard()
        {
            var accountWithBalance = AccountsWithBalance?.FirstOrDefault(a => a.Id == CurrentAccount.Id);
            if (accountWithBalance != null)
            {
                gridDatosA.Rows.Clear();
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(gridDatosA);
                row.Cells[0].Value = accountWithBalance.PriorBalance;
                row.Cells[1].Value = accountWithBalance.DebitBalance;
                row.Cells[2].Value = accountWithBalance.CreditBalance;
                row.Cells[3].Value = accountWithBalance.MontlyBalance;
                row.Cells[4].Value = accountWithBalance.CurrentBalance;

                gridDatosA.Rows.Add(row);
            }
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



        private void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAccountBalanceInformationDashboard();
            LoadAccountBalanceToDashBoard();
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
