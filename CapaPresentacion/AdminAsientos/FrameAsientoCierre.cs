using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.cods;
using CapaPresentacion.FrameCuentas;
using CapaPresentacion.Reportes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.AdminAsientos
{
    public partial class FrameAsientoCierre : Form, ICallingForm
    {
        private Cuenta _CuentaFinal;
        private FechaTransaccionCL fechaCL = new FechaTransaccionCL();
        private List<FechaTransaccion> fechaTransaccions = new List<FechaTransaccion>();
        private FechaTransaccion UltimoMes;
        private IEnumerable<Cuenta> cuentasFinales;
        private IEnumerable<Cuenta> _lstFinal
        {
            set
            {
                cuentasFinales = value;
            }
            get
            {
                return cuentasFinales;
            }
        }
        public FrameAsientoCierre()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            //List<FechaTransaccion> lst = fechaCL.GetAll(compania, usuario);
            DataTable dt = fechaCL.GetDataTable(GlobalConfig.Company, GlobalConfig.Usuario);
            dtRegistros.DataSource = dt;
            fechaTransaccions = fechaCL.GetAllActive(GlobalConfig.Company, GlobalConfig.Usuario);
            lstAbrirMes.DataSource = fechaTransaccions;
        }

        private void btnCerrarPeriodo_Click(object sender, EventArgs e)
        {
            if (_CuentaFinal is null)
            {
                MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                VerificarMesesPorCerrar();
        }

        /// <summary>
        /// Mover al objeto maestro de cuentas
        /// </summary>
        /// <returns></returns>
        private void VerificarMesesPorCerrar()
        {

            /// verificar que no hayan meses atrar por cerrar
            /// 
            var fechaCierre = (FechaTransaccion)lstAbrirMes.SelectedItem;
            var meses = from fch in fechaTransaccions where fch.Fecha.Date <= fechaCierre.Fecha.Date select fch;
            UltimoMes = meses.FirstOrDefault();
            if (meses.Count() > 0)
            {
                if (MessageBox.Show($"Se realizara cierre de los siguientes meses: {String.Join(", ", meses)}, ¿Desea continuar?",
                      TextoGeneral.NombreApp, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CerrarMesesPendientes(meses);
                }
            }
        }

        private void CerrarMesesPendientes(IEnumerable<FechaTransaccion> meses)

        {

            if (!backgroundWorker.IsBusy)
            {
                var f = (FrameMenu)this.MdiParent;
                f.Bar = true;
                backgroundWorker.RunWorkerAsync(meses);

            }



        }
        public int cont = 0;
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();


        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                using (new CursorWait(applicationCursor: true, appStarting: true))
                {
                    var meses = ((IEnumerable<FechaTransaccion>)e.Argument);
                    _lstFinal = fechaCL.AsientoDeCierre(GlobalConfig.Company, GlobalConfig.Usuario, meses, _CuentaFinal);
               }
            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Se ha cerrado el periodo exitosamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSeleccionarCuenta_Click(object sender, EventArgs e)
        {
            FrameSeleccionCuenta Frame = new FrameSeleccionCuenta(this);
            Frame.ShowDialog();

        }

        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta.Indicador != CapaEntidad.Enumeradores.IndicadorCuenta.Cuenta_Auxiliar)
            {
                return false;
            }
            else
                txtBoxNombreCuenta.Text = cuenta.Nombre;
            _CuentaFinal = cuenta;
            return true;
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            //FrameReporteComprobacion n = new FrameReporteComprobacion(_lstFinal, UltimoMes);
            //n.MdiParent = this.MdiParent;
            //n.Show(); 
        }
    }
}
