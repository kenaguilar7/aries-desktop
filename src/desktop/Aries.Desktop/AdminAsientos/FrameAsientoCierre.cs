using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Interfaces;
using Aries.Reporting.Textos;
using Aries.Desktop.FrameCuentas;
using Aries.Desktop.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.AdminAsientos
{
    public partial class FrameAsientoCierre : Form, ICallingForm
    {
        private readonly IFinancialService _financialService;
        private readonly IFinancialReportService _financialReportService;
        private Cuenta _CuentaFinal;
        private List<PostingPeriod> _periodos = new List<PostingPeriod>();
        private PostingPeriod UltimoMes;

        public FrameAsientoCierre(IFinancialService financialService, IFinancialReportService financialReportService)
        {
            _financialService = financialService;
            _financialReportService = financialReportService;
            InitializeComponent();
            Load += FrameAsientoCierre_Load;
        }

        private async void FrameAsientoCierre_Load(object sender, EventArgs e)
        {
            await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            _periodos = (await _financialService.GetPostingPeriodsAsync(GlobalConfig.Company.Code))
                .Where(p => !p.Closed)
                .OrderBy(p => p.Date)
                .ToList();
            dtRegistros.DataSource = ToPeriodTable(_periodos);
            lstAbrirMes.DataSource = _periodos.ToList();
        }

        private static DataTable ToPeriodTable(IEnumerable<PostingPeriod> periods)
        {
            var table = new DataTable();
            table.Columns.Add("Periodo");
            table.Columns.Add("Cerrado");
            foreach (var period in periods)
                table.Rows.Add(period.ToString(), period.Closed ? "Sí" : "No");
            return table;
        }

        private async void btnCerrarPeriodo_Click(object sender, EventArgs e)
        {
            await VerificarMesesPorCerrarAsync();
        }

        private async Task VerificarMesesPorCerrarAsync()
        {
            var fechaCierre = lstAbrirMes.SelectedItem as PostingPeriod;
            if (fechaCierre == null)
            {
                MessageBox.Show("Seleccione un periodo", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var meses = _periodos.Where(fch => fch.Date.Date <= fechaCierre.Date.Date).ToList();
            UltimoMes = meses.FirstOrDefault();
            if (meses.Count == 0)
                return;

            if (MessageBox.Show($"Se realizara cierre de los siguientes meses: {string.Join(", ", meses)}, ¿Desea continuar?",
                  TextoGeneral.NombreApp, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await CerrarMesesPendientesAsync(meses);
            }
        }

        public int cont = 0;
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async Task CerrarMesesPendientesAsync(IEnumerable<PostingPeriod> meses)
        {
            var f = (FrameMenu)this.MdiParent;
            if (f != null)
                f.Bar = true;
            btnCerrarPeriodo.Enabled = false;
            try
            {
                await UiBusy.Run(this, async () =>
                {
                    var ordered = meses.OrderBy(x => x.Date).ToList();
                    var fromDate = ordered.First();
                    var toDate = ordered.Last();
                    foreach (var period in ordered)
                    {
                        period.Closed = true;
                        period.UpdatedBy = GlobalConfig.User != null ? GlobalConfig.User.Id : 0;
                    }

                    var reportParamns = new BasicReportParam
                    {
                        CompanyId = GlobalConfig.Company.Code,
                        FirstDate = $"{fromDate.Date.Year}{string.Format("{0, 0:D2}", fromDate.Date.Month)}",
                        EndDate = $"{toDate.Date.Year}{string.Format("{0, 0:D2}", toDate.Date.Month)}"
                    };
                    var amount = await _financialReportService.PreviousClosurePostingPeriodBalanceAsync(reportParamns);

                    await _financialService.ClosePostingPeriodAsync(new PostingPeriodEndClosing
                    {
                        CompanyId = GlobalConfig.Company.Code,
                        FromPeriodId = fromDate.Id,
                        ToPeriodId = toDate.Id,
                        FromPeriod = fromDate.ToString(),
                        ToPeriod = toDate.ToString(),
                        Amount = amount.Amount,
                        PostingPeriods = ordered,
                        UpdatedBy = GlobalConfig.User != null ? GlobalConfig.User.Id : 0
                    });
                });

                await CargarDatosAsync();
                MessageBox.Show("Se ha cerrado el periodo exitosamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCerrarPeriodo.Enabled = true;
                if (f != null)
                    f.Bar = false;
            }
        }

        private void btnSeleccionarCuenta_Click(object sender, EventArgs e)
        {
            FrameSeleccionCuenta Frame = new FrameSeleccionCuenta(this);
            Frame.ShowDialog();
        }

        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta.Indicador != Aries.Reporting.Enumeradores.IndicadorCuenta.Cuenta_Auxiliar)
            {
                return false;
            }

            txtBoxNombreCuenta.Text = cuenta.Nombre;
            _CuentaFinal = cuenta;
            return true;
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
        }
    }
}
