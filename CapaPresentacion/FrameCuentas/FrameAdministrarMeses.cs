using AriesContador.Core;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAdministrarMeses : Form
    {
        private readonly IFinancialService _financialService;
        private readonly IFinancialReportService _financialReportService;
        private List<PostingPeriod> _postingPeriods = new List<PostingPeriod>(); 

        public FrameAdministrarMeses()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
            _financialReportService = new FinancialReportService(unit); 
        }

        private void FrameAdministrarMeses_Load(object sender, EventArgs e)
        {
            LoadDropDowns();
            LoadDataGrids();
            dtRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtRegistros.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dtGridClosingPeriodsReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridClosingPeriodsReport.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dtGridClosingPeriodsReport.Columns[nameof(ClosingPostingPeriodReport.Amount)].DefaultCellStyle.Format = "#,0.00";

        }

        private void LoadDataGrids()
        {
            dtRegistros.DataSource = _financialReportService.PostingPeriodInfo(GlobalConfig.Company.Codigo);
            dtGridClosingPeriodsReport.DataSource =
                _financialReportService.ClosingPostingPeriodReport(GlobalConfig.Company.Codigo); 
        }

        private void LoadDropDowns()
        {
            _postingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();
            var olderPeriod = _postingPeriods.Where(x => !x.Closed)
                                             .OrderBy(x => x.Date)
                                             .ToList()
                                             .FirstOrDefault().DeepClone();

            this.lstFromPeriod.DataSource = new List<PostingPeriod>() { olderPeriod };

            var availablePostingPeriods =
                _financialService.GetAvailablePostingPeriodsForBeCreated(GlobalConfig.Company.Codigo);
            lstAbrirMes.DataSource = availablePostingPeriods;
        }


        private void Btn_Create_PostingPeriod(object sender, EventArgs e)
        {
            try
            {
                btnGuardar.Enabled = false;
                CreateNewPostingPeriod();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                FrameAdministrarMeses_Load(null, null); 
                btnGuardar.Enabled = true;
            }
        }

        private void CreateNewPostingPeriod()
        {
            var selectedItem = lstAbrirMes.SelectedItem as PostingPeriod;
            var selectedDate = selectedItem.Date; 

            var postingPeriod = new PostingPeriod()
            {
                Date = new DateTime(selectedDate.Year, selectedDate.Month, 1, 0, 0, 0, 0), 
                Closed = false, 
                CompanyId = GlobalConfig.Company.Codigo, 
                UpdatedBy = GlobalConfig.Usuario.Id
            }; 

            _financialService.CreatePostingPeriod(postingPeriod);
        }

        private void lstFromPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var startMonth = lstFromPeriod.SelectedItem as PostingPeriod;
            lstToPeriod.DataSource = _postingPeriods.Where(x => !x.Closed).ToList()
                                                    .GetOlder(startMonth.Date);
        }

        private void BtnCerrarMes_Click(object sender, EventArgs e)
        {
            var amount = ReporteEstadoResultadoIntegralData();

            if (MessageBox.Show($@"Se creará un cierre contable por {String.Format("{0:n}", amount.Amount)}", TextoGeneral.NombreApp,
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                ClosePeriodProcess(amount.Amount);
            }
        }

        private void ClosePeriodProcess(decimal amount)
        {
            var fromDatePeriod = lstFromPeriod.SelectedItem as PostingPeriod;
            var toDatePeriod = lstToPeriod.SelectedItem as PostingPeriod;

            var postingPeriod = _postingPeriods.GetByRange(fromDatePeriod.Date, toDatePeriod.Date);

            foreach (var period in postingPeriod)
            {
                period.Closed = true;
                period.UpdatedBy = GlobalConfig.Usuario.Id; 
            }

            var savedModel = new PostingPeriodEndClosing()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FromPeriodId = fromDatePeriod.Id, 
                ToPeriodId = toDatePeriod.Id, 
                FromPeriod = fromDatePeriod.ToString(), 
                ToPeriod = toDatePeriod.ToString(), 
                Amount = amount, 
                UserNotes = txtBoxUserNotes.Text, 
                PostingPeriods = postingPeriod, 
                UpdatedBy = GlobalConfig.Usuario.Id
            }; 

            try
            {
                btnCerrarMes.Enabled = false;
                _financialService.ClosePostingPeriod(savedModel);
                CreateNewPostingPeriod();
                txtBoxUserNotes.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                FrameAdministrarMeses_Load(null, null);
                btnCerrarMes.Enabled = true;
            }

        }
        

        private ClosurePostingPeriodBalance ReporteEstadoResultadoIntegralData()
        {
            var firstDate = lstFromPeriod.SelectedItem as PostingPeriod;
            var endDate = lstToPeriod.SelectedItem as PostingPeriod;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            return  _financialReportService.PreviousClosurePostingPeriodBalance(reportParamns);
        }


    }
}
