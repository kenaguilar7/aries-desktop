using System;
using System.Diagnostics;
using System.Windows.Forms;
using Aries.Reporting.Entidades.Ventanas;
using Aries.Reporting.Textos;
using Aries.Desktop.FrameCompañias;
using Aries.Desktop.FrameCuentas;
using Aries.Desktop.Seguridad;
using Aries.Desktop.Reportes;
using Aries.Desktop.AdminAsientos;
using Aries.Desktop.FrameUsuarios;
using Aries.Desktop.Restore;
using AriesContador.Core.Services;
using AriesContador.Core.Models;
using Aries.Desktop.Utils;

namespace Aries.Desktop
{
    public partial class FrameMenu : Form
    {
        private readonly IAdministrationService _administrationService;
        private readonly IFinancialService _financialService;
        private readonly IFinancialReportService _financialReportService;
        private readonly IPermissionService _permissionService;
        private readonly IEmailService _emailService;

        private readonly string _windowTitle;

        public Boolean comParametro { set { CargarCompañia(); } }
        public FrameMenu(
            IAdministrationService administrationService,
            IFinancialService financialService,
            IFinancialReportService financialReportService,
            IPermissionService permissionService,
            IEmailService emailService)
        {
            this._administrationService = administrationService;
            this._financialService = financialService;
            this._financialReportService = financialReportService;
            this._permissionService = permissionService;
            this._emailService = emailService;
            InitializeComponent();
            _windowTitle = this.Text;

            if (!TryLogin())
            {
                Application.Exit();
                return;
            }
            CargarDatos();
        }

        private bool TryLogin()
        {
            using (var login = new LoginForm(_administrationService))
                login.ShowDialog();
            return GlobalConfig.User != null;
        }
        private void CargarDatos()
        {
            if (GlobalConfig.Usuario != null)
            {
                this.txtUsuario.Text = GlobalConfig.Usuario.ToString();
                HideOptions();
                AddVersionNumber();
            }
        }
        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var beta = GlobalConfig.IsBeta ? " Beta" : string.Empty;
            this.Text = $"{_windowTitle} v.{versionInfo.FileVersion} [{GlobalConfig.EnvironmentName}{beta}] {GlobalConfig.MySqlDatabase}";
        }

        private void CargarCompañia()
        {
            this.txtCompaniaNombre.Text = GlobalConfig.Company.ToString();
        }

        private void MaestroDeCompañiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameMaestroCompañia n = new FrameMaestroCompañia(_administrationService);
            n.MdiParent = this;
            n.Show();
        }
        private void MaestroDeCuentasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.Company != null)
            {
                FrameMaestroCuenta n = new FrameMaestroCuenta(_financialService);
                n.MdiParent = this;
                n.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void AsientosContablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Company != null)
                {
                    if (!CheckForDuplicate(VentanaInfo.FormAsientos))
                    {
                        FrameAsientos n = new FrameAsientos(_financialService, _financialReportService);
                        n.MdiParent = this;
                        n.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void MaestroDeUsaurio(object sender, EventArgs e)
        {
            FrameMaestroUsuario n = new FrameMaestroUsuario(_administrationService);
            n.MdiParent = this;
            n.Show();
        }
        private void SeleccioneCompañiaParaTrabajar(object sender, EventArgs e)
        {
            try
            {
                FrameSeleccionCompañia n = new FrameSeleccionCompañia(this, _administrationService);
                n.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MaestroDeMeses(object sender, EventArgs e)
        {

            if (GlobalConfig.Company != null)
            {
                FrameAdministrarMeses n = new FrameAdministrarMeses(_financialService, _financialReportService);
                n.MdiParent = this;
                n.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void MaestroDeCompañia(object sender, EventArgs e)
        {
            try
            {
                FrameSeleccionCompañia n = new FrameSeleccionCompañia(this, _administrationService);
                n.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void balanceDeComprobaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Company != null)
                {
                    FrameReporteComprobación n = new FrameReporteComprobación(_financialService, _financialReportService);
                    n.MdiParent = this;
                    n.Show();

                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void balanceDeAuxiliaresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Company != null)
                {
                    FrameReporteAuxiliares frame = new FrameReporteAuxiliares(_financialService);
                    frame.MdiParent = this;
                    frame.Show();
                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void HideOptions()
        {
            ResetMenuVisibility();

            if (GlobalConfig.Usuario.TipoUsuario == Aries.Reporting.Enumeradores.TipoUsuario.Usuario)
            {

                elementosEliminadosToolStripMenuItem.Enabled = false; 

                if ((GlobalConfig.Usuario.Modulos.Find(x => x.Codigo == 1) is var mConta) && mConta == null || !mConta.TienePermiso)
                {
                    contableToolStripMenuItem.Enabled = false;
                    contableToolStripMenuItem.Visible = false;
                }
                else
                {
                    ///Empezamos por ventanas
                    maestroDeCuentasToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroCuenta)).TienePermiso;
                    asientosContablesToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormAsientos)).TienePermiso;
                    administrarMesesToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormAdminMeses)).TienePermiso;
                }

                if ((GlobalConfig.Usuario.Modulos.Find(x => x.Codigo == 2) is var MCompanias) && MCompanias == null || !MCompanias.TienePermiso)
                {
                    maestroDeCompañiasToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    maestroDeCompañiasToolStripMenuItem.Enabled = (MCompanias.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroCompanias)).TienePermiso;
                }

                if ((GlobalConfig.Usuario.Modulos.Find(x => x.Codigo == 3) is var mSeguridad) && mSeguridad == null || !mSeguridad.TienePermiso)
                {
                    sistemaToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    PermisosDeUsuarioToolStripMenuItem.Enabled = (mSeguridad.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormPermisoUsuario)).TienePermiso;

                }

                if ((GlobalConfig.Usuario.Modulos.Find(x => x.Codigo == 4) is var mUsuario) && mUsuario == null || !mUsuario.TienePermiso)
                {
                    usuariosToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    MaestroDeUsuariotoolStripMenuItem.Enabled = (mUsuario.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroUsuario)).TienePermiso;

                }

            }

        }

        private void ResetMenuVisibility()
        {
            contableToolStripMenuItem.Enabled = true;
            contableToolStripMenuItem.Visible = true;
            maestroDeCuentasToolStripMenuItem.Visible = true;
            asientosContablesToolStripMenuItem.Visible = true;
            administrarMesesToolStripMenuItem.Visible = true;
            maestroDeCompañiasToolStripMenuItem.Enabled = true;
            sistemaToolStripMenuItem.Enabled = true;
            PermisosDeUsuarioToolStripMenuItem.Enabled = true;
            usuariosToolStripMenuItem.Enabled = true;
            MaestroDeUsuariotoolStripMenuItem.Enabled = true;
            elementosEliminadosToolStripMenuItem.Enabled = true;
        }

        private bool CloseChildrenIfClean()
        {
            foreach (Form form in MdiChildren)
            {
                var needsCheck = form as INeedValidatedForClose;
                if (needsCheck != null && !needsCheck.IsAvalibleToClose())
                    return false;
            }

            for (int i = MdiChildren.Length - 1; i >= 0; i--)
                MdiChildren[i].Close();
            return true;
        }

        private async void CerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CloseChildrenIfClean())
                return;

            await GlobalConfig.ClearSessionAsync();
            txtUsuario.Text = string.Empty;
            txtCompaniaNombre.Text = string.Empty;
            this.Text = _windowTitle;

            if (!TryLogin())
            {
                Application.Exit();
                return;
            }
            CargarDatos();
        }

        private void gestorDeVentanasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPermisoUsuario form = new FormPermisoUsuario(_administrationService, _permissionService)
            {
                MdiParent = this
            };
            form.Show();
        }
        private void movimientosDeCuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (GlobalConfig.Company != null)
            {
                ReporteMovimientosCuenta form = new ReporteMovimientosCuenta(_financialReportService)
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void perdiasYGananciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.Company != null)
            {
                ReporteEstadoResultadoIntegral form = new ReporteEstadoResultadoIntegral(_financialService, _financialReportService)
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void balanceDeSituaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (GlobalConfig.Company != null)
            {
                ReporteBalanceSituacion form = new ReporteBalanceSituacion(_financialService)
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private bool CheckForDuplicate(VentanaInfo ventana)
        {
            bool bValue = false;
            foreach (Form fm in this.MdiChildren)
            {
                if (fm.Name == "FrameAsientos")
                {
                    fm.Activate();
                    fm.WindowState = FormWindowState.Normal;
                    bValue = true;
                }
            }
            return bValue;
        }
        private void cierreDePeriodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.Company != null)
            {
                FrameAsientoCierre form = new FrameAsientoCierre(_financialService, _financialReportService)
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public Boolean Bar
        {
            set
            {
                ProgressBar.Visible = value;
                ProgressBar.Value = 80;
                
            }
        }

        private void gestionDeCorreosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Correo n = new Correo(_emailService);
            n.MdiParent = this;
            n.Show(); 
        }

        private void elementosEliminadosToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (GlobalConfig.Company != null)
            {
                var n = new RestoreJournalEntry(_financialService);
                n.MdiParent = this;
                n.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void actualizacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FrameActualizaciones())
                form.ShowDialog(this);
        }

        private void tokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var scriptInfo =
                "Ambiente: " + GlobalConfig.EnvironmentName
                + Environment.NewLine
                + "MySQL: " + GlobalConfig.MySqlServer
                + Environment.NewLine
                + "Database: " + GlobalConfig.MySqlDatabase
                + Environment.NewLine
                + "Login / maestros / asientos: in-process (no necesitan API)"
                + Environment.NewLine
                + "Token: "
                + (EnvironmentVariable.ApiToken?.Token ?? "(ninguno)")
                + Environment.NewLine
                + "API (si la arrancas): "
                + EnvironmentVariable.ApiUrl
                + Environment.NewLine
                + "Update: "
                + (string.IsNullOrWhiteSpace(GlobalConfig.UpdateUrl) ? "(ninguno)" : GlobalConfig.UpdateUrl)
                + Environment.NewLine
                + "Log: "
                + StartupLog.FilePath; 
            MessageBox.Show(scriptInfo, "Token info",MessageBoxButtons.OK, MessageBoxIcon.Information); 
        }
    }
}
