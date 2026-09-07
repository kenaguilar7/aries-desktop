using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Interfaces;
using Aries.Reporting.Textos;
using Aries.Desktop.cods;
using Aries.Desktop.FrameCuentas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
            CargarDatos();
        }

        private void CargarDatos()
        {
            _periodos = _financialService.GetPostingPeriods(GlobalConfig.Company.Code)
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

        private void btnCerrarPeriodo_Click(object sender, EventArgs e)
        {
            VerificarMesesPorCerrar();
        }

        private void VerificarMesesPorCerrar()
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
                CerrarMesesPendientes(meses);
            }
        }

        private void CerrarMesesPendientes(IEnumerable<PostingPeriod> meses)
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
                    var meses = ((IEnumerable<PostingPeriod>)e.Argument).OrderBy(x => x.Date).ToList();
                    var fromDate = meses.First();
                    var toDate = meses.Last();
                    foreach (var period in meses)
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
                    var amount = _financialReportService.PreviousClosurePostingPeriodBalance(reportParamns);

                    _financialService.ClosePostingPeriod(new PostingPeriodEndClosing
                    {
                        CompanyId = GlobalConfig.Company.Code,
                        FromPeriodId = fromDate.Id,
                        ToPeriodId = toDate.Id,
                        FromPeriod = fromDate.ToString(),
                        ToPeriod = toDate.ToString(),
                        Amount = amount.Amount,
                        PostingPeriods = meses,
                        UpdatedBy = GlobalConfig.User != null ? GlobalConfig.User.Id : 0
                    });
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
            CargarDatos();
            MessageBox.Show("Se ha cerrado el periodo exitosamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
