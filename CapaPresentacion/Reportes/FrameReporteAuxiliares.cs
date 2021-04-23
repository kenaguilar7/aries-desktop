using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Reportes;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class FrameReporteAuxiliares : Form
    {

        FechaTransaccionCL fechaTransaccionCL = new FechaTransaccionCL();
        CuentaCL cuentaCL = new CuentaCL();
        private List<FechaTransaccion> fechaTransaccions = new List<FechaTransaccion>();

        public FrameReporteAuxiliares()
        {
            InitializeComponent();
            CargarDatos();
        }
        private void CargarDatos()
        {

            var lstMeses = fechaTransaccionCL.GetAll(GlobalConfig.Company, GlobalConfig.Usuario);
            fechaTransaccions = lstMeses;
            this.lstMesInicio.DataSource = lstMeses;
        }

        private void lstMesInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var meses = (from n in fechaTransaccions where n.Fecha >= ((FechaTransaccion)lstMesInicio.SelectedItem).Fecha select n).ToList<FechaTransaccion>();

            lstMesFinal.DataSource = meses;
        }

        private void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            try
            {


                var lstCuentas = new Dictionary<FechaTransaccion, List<Cuenta>>();
                var cuentas = cuentaCL.GetAll(GlobalConfig.Company);

                //cuentaCL.LLenarConSaldoB(((FechaTransaccion)lstMesInicio.SelectedItem).Fecha, ((FechaTransaccion)lstMesInicio.SelectedItem).Fecha, cuentas, GlobalConfig.Compañia); 
                foreach (var item in fechaTransaccions)
                {
                    if (item.Fecha >= ((FechaTransaccion)lstMesInicio.SelectedItem).Fecha && item.Fecha <= ((FechaTransaccion)lstMesFinal.SelectedItem).Fecha)
                    {
                        var cuentasClonadas = new List<Cuenta>(cuentas.Count);

                        cuentas.ForEach((Cuenta) =>
                        {
                            cuentasClonadas.Add(Cuenta.DeepCopy());
                        });

                        new CuentaCL().LLenarConSaldos(item.Fecha, item.Fecha, cuentasClonadas, GlobalConfig.Company);

                        cuentasClonadas = cuentaCL.QuitarCuentasSinSaldos(cuentasClonadas);

                        lstCuentas.Add(item, cuentasClonadas);
                    }
                }

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", Title = "Reporte auxiliares", FileName = $"REPORTE DE AUXILIARES {GlobalConfig.Company.ToString()} - {GlobalConfig.Company.NumeroCedula}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteAuxiliares.GenerarReporte(lstCuentas, GlobalConfig.Company, GlobalConfig.Usuario, GlobalConfig.Company.TipoMoneda, sfd.FileName);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }

        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
