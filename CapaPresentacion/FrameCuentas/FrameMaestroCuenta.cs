using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Textos;
using CapaPresentacion.cods;
using CapaPresentacion.Reportes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ClosedXML.Report.Utils;

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
    
        private void LoadAccounts()
        {
            CompanyAccounts?.Clear();
            CompanyAccounts = _financialService.GetAccounts(GlobalConfig.Company.Codigo).ToList(); 
            treeCuentas.Nodes?.Clear();
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

        private void LoadAccountInfoToDashboard()
        {
            try
            {
                this.txtAccountName.Text = CurrentAccount.Name;
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

        #region Events
        private void AFechaInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(AFechaInicio.SelectedItem is PostingPeriod selectedPostingP)
                AFechaFinal.DataSource = PostingPeriods.GetOlder(selectedPostingP.Date); 
        }

        private void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.txtAccountName.ReadOnly = true;
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

        private void UpdateAccountName(object sender, EventArgs e)
        {
            try
            {
                var account = this.CurrentAccount.DeepClone();
                account.Name = this.txtAccountName.Text;
                account.Memo = this.txtBoxDetalle.Text;
                account.UpdatedBy = GlobalConfig.Usuario.Id;
                account.UpdateAt = DateTime.Now;

                if (ValidateChildren())
                {
                    _financialService.UpdateAccountName(account);
                    LoadAccounts();
                    this.treeCuentas.ExpandAll();
                    var selectedNode = treeCuentas.Nodes.Find(account.Name, true);
                    this.treeCuentas.SelectedNode = selectedNode.FirstOrDefault();
                    //this.txtAccountName.ReadOnly = true;
                    //this.txtBoxDetalle.ReadOnly = true;
                    ActivateEditAccountNameOptions(false);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            BuildAccountBalanceInformationDashboard();
            LoadAccountBalanceToDashBoard();
        }

        private void Eliminar_Click(object sender, EventArgs e)
        {
            if (CurrentAccount == null)
            {
                //Don't do nothing
            }
            else if (!CurrentAccount.Editable)
            {
                MessageBox.Show("Las cuentas primarias no se pueden eliminar", TextoGeneral.NombreApp,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    var fatherAccount = CompanyAccounts.Find(a => a.Id == CurrentAccount.FatherAccount);
                    _financialService.DeleteAccount(CurrentAccount);
                    LoadAccounts();
                    this.treeCuentas.ExpandAll();
                    var selectedNode = treeCuentas.Nodes.Find(fatherAccount.Name, true);
                    this.treeCuentas.SelectedNode = selectedNode.FirstOrDefault();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Llamada a otras ventanas
        private void Listar(object sender, EventArgs e)
        {
            ReporteCuenta reporte = new ReporteCuenta(GlobalConfig.Company, GlobalConfig.Usuario);
            reporte.MdiParent = this.MdiParent;
            reporte.Show();

        }

        private void CrearNuevaCuenta(object sender, EventArgs e)
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
            try
            {
                if (CurrentAccount != null && CurrentAccount.Editable)
                {
                    this.txtAccountName.Focus();
                    ActivateEditAccountNameOptions(true);
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

        private void ActivateEditAccountNameOptions(bool enable)
        {
            btnGuardarNuevoNombre.Enabled = enable;
            btnGuardarNuevoNombre.Visible = enable;

            this.txtAccountName.ReadOnly = !enable;
            this.txtBoxDetalle.ReadOnly = !enable;
        }
        
        private void UsuarioKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void BtnMovimientosCuenta_Click(object sender, EventArgs e)
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

        private void TxtAccountName_Validating(object sender, CancelEventArgs e)
        {
            var newAccountName = txtAccountName.Text; 
            var isRepetitive = CompanyAccounts.Exists(a =>
                a.AccountTag == CurrentAccount.AccountTag &&
                a.Name.Equals(newAccountName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase)); 

            if (string.IsNullOrEmpty(txtAccountName.Text))
            {
                e.Cancel = true;
                accountsErrorProviders.SetError(this.txtAccountName, "Agregue un nombre");
            }
            else if (isRepetitive && newAccountName != CurrentAccount.Name)
            {
                e.Cancel = true;
                accountsErrorProviders.SetError(this.txtAccountName, "Ya existe una cuenta con este nombre");
            }
            else
            {
                e.Cancel = false;
                accountsErrorProviders.SetError(this.txtAccountName, string.Empty);
            }
        }
    }
}
