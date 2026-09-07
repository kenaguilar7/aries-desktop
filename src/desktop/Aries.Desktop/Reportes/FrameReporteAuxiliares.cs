using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.FechaTransacciones;
using Aries.Reporting.Mappers;
using Aries.Reporting.Reportes;
using Aries.Desktop.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.Reportes
{
    public partial class FrameReporteAuxiliares : Form
    {

        private readonly IFinancialService _financialService;
        private List<FechaTransaccion> fechaTransaccions = new List<FechaTransaccion>();

        public FrameReporteAuxiliares(IFinancialService financialService)
        {
            _financialService = financialService;
            InitializeComponent();
            Load += FrameReporteAuxiliares_Load;
        }

        private async void FrameReporteAuxiliares_Load(object sender, EventArgs e)
        {
            await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            var lstMeses = CuentaMapper.ToFechaTransaccionList(
                await _financialService.GetPostingPeriodsAsync(GlobalConfig.Company.Code));
            fechaTransaccions = lstMeses;
            this.lstMesInicio.DataSource = lstMeses;
        }

        private void lstMesInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var meses = (from n in fechaTransaccions where n.Fecha >= ((FechaTransaccion)lstMesInicio.SelectedItem).Fecha select n).ToList<FechaTransaccion>();

            lstMesFinal.DataSource = meses;
        }

        private async void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            try
            {


                var lstCuentas = new Dictionary<FechaTransaccion, List<Cuenta>>();
                var cuentas = await ReportAccountLoader.LoadAsync(_financialService, GlobalConfig.Company);

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

                        await ReportAccountLoader.FillBalancesAsync(_financialService, cuentasClonadas, item.Fecha, item.Fecha);

                        cuentasClonadas = ReportAccountLoader.WithoutEmptyBalances(cuentasClonadas);

                        lstCuentas.Add(item, cuentasClonadas);
                    }
                }

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", Title = "Reporte auxiliares", FileName = $"REPORTE DE AUXILIARES {GlobalConfig.Company.ToString()} - {GlobalConfig.Company.IdNumber}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteAuxiliares.GenerarReporte(lstCuentas, GlobalConfig.Company, GlobalConfig.Usuario, GlobalConfig.Company.CurrencyType, sfd.FileName);
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
