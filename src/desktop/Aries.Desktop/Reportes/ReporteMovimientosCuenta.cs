using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using Aries.Reporting.Reportes;
using Aries.Reporting.Textos;
using Aries.Desktop.FrameCuentas;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.Reportes
{
    public partial class ReporteMovimientosCuenta : Form, ICallingForm
    {
        private readonly IFinancialReportService _financialReportService;
        public ReporteMovimientosCuenta(IFinancialReportService financialReportService)
        {
            _financialReportService = financialReportService;
            InitializeComponent();
            CargarDatos();
        }
        private void CargarDatos()
        {

            if (GlobalConfig.Company.CurrencyType == CurrencyTypeCompany.Solo_Colones)
            {
                chckBxMostarDolares.Checked = false;
                chckBxMostarDolares.Enabled = false;
                chckBxMostarDolares.Visible = false;
            }
            CargarDatosAlGrid(new Cuenta());


        }

        /// <summary>
        /// Metodo usado para traer la cuenta desde la ventana que las lista
        /// </summary>
        /// <param name="cuenta"></param>
        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta != null)
            {
                txtBoxCuentaSeleccionada.Text = cuenta.ToString();
                txtBoxCuentaSeleccionada.Tag = cuenta;
                LoadAccountGrid(cuenta);
                return true;
            }
            else
                return false;
        }

        private async void LoadAccountGrid(Cuenta cuenta)
        {
            await CargarDatosAlGridAsync(cuenta);
        }

        private void CargarDatosAlGrid(Cuenta cuenta)
        {
            LoadAccountGrid(cuenta);
        }

        private async Task CargarDatosAlGridAsync(Cuenta cuenta)
        {

            GridDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            var auxiliar = cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar;
            GridDatos.DataSource = cuenta.Id == 0
                ? new DataTable()
                : await _financialReportService.GetAccountMovementReportAsync(cuenta.Id, auxiliar);
            SetEstilosDataDrid();
            AjustarColumnaDolares();
            GridDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void MostarDolares(object sender, EventArgs e)
        {
            AjustarColumnaDolares();
        }

        private void SetEstilosDataDrid()
        {
            foreach (DataGridViewColumn item in GridDatos.Columns)
            {

                if (item.Name == "Monto" || item.Name == "Monto Dolares" || item.Name == "Saldo Actual" ||
                    item.Name == "Tipo Cambio" || item.Name == "Debito" || item.Name == "Credito")
                {
                    item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; //dataGridSpecialStyle

                }
                else if (item.Name == "Numero de Asiento")
                {
                    item.HeaderText = "Número de Asiento";
                    item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                }
            }
        }


        private void AjustarColumnaDolares()
        {
            foreach (DataGridViewColumn item in GridDatos.Columns)
            {
                if (item.Name == "Tipo Cambio" || item.Name == "Monto Dolares")
                {
                    item.Visible = chckBxMostarDolares.Checked;
                }
            }
        }

        private void CerrarFormulario(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GenerarExcel(object sender, EventArgs e)
        {
            try
            {


                if ((Cuenta)txtBoxCuentaSeleccionada.Tag != null)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE MOVIMIENTOS DE CUENTA {GlobalConfig.Company.ToString()}" })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            ReporteMovimientoCuenta.GenerarReporte(sfd.FileName, chckBxMostarDolares.Checked,
                                (DataTable)GridDatos.DataSource, GlobalConfig.Company, GlobalConfig.Usuario, (Cuenta)txtBoxCuentaSeleccionada.Tag);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FrameReporteMovimientosCuenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\u0013'))
            {
                ///CTR + S
                btnSeleccionarCuenta_Click(null, null);

            }
        }

        private void btnSeleccionarCuenta_Click(object sender, EventArgs e)
        {
            FrameSeleccionCuenta frame = new FrameSeleccionCuenta(this);
            frame.ShowDialog();
        }
    }
}
