using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Reportes;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.FrameCuentas;
using System;
using System.Data;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteMovimientosCuenta : Form, ICallingForm
    {
        private CuentaCL _cuentaCL { get; } = new CuentaCL();
        public ReporteMovimientosCuenta()
        {
            InitializeComponent();
            CargarDatos();
        }
        private void CargarDatos()
        {

            if (GlobalConfig.Company.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
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
                CargarDatosAlGrid(cuenta);
                return true;
            }
            else
                return false;
        }
        private void CargarDatosAlGrid(Cuenta cuenta)
        {

            GridDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            GridDatos.DataSource = _cuentaCL.GetInfoCompleta(cuenta);
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
