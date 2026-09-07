using Aries.Reporting.Textos;
using Aries.Desktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;

namespace Aries.Desktop.FrameCuentas
{
    public partial class FrameAdministrarMeses : Form
    {
        private readonly IFinancialService _financialService;
        private readonly IFinancialReportService _financialReportService;
        private List<PostingPeriod> _postingPeriods = new List<PostingPeriod>(); 

        public FrameAdministrarMeses()
        {
            InitializeComponent();
        }

        public FrameAdministrarMeses(IFinancialService financialService, IFinancialReportService financialReportService)
            : this()
        {
            _financialService = financialService;
            _financialReportService = financialReportService; 
        }

        private async void FrameAdministrarMeses_Load(object sender, EventArgs e)
        {
            await LoadDropDownsAsync();
            await LoadDataGridsAsync();
            dtRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtRegistros.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dtGridClosingPeriodsReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridClosingPeriodsReport.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dtGridClosingPeriodsReport.Columns[nameof(ClosingPostingPeriodReport.Amount)].DefaultCellStyle.Format = "#,0.00";

        }

        private async Task LoadDataGridsAsync()
        {
            dtRegistros.DataSource = await _financialReportService.PostingPeriodInfoAsync(GlobalConfig.Company.Code);
            dtGridClosingPeriodsReport.DataSource =
                await _financialReportService.ClosingPostingPeriodReportAsync(GlobalConfig.Company.Code); 
        }

        private async Task LoadDropDownsAsync()
        {
            _postingPeriods = (await _financialService.GetPostingPeriodsAsync(GlobalConfig.Company.Code)).ToList();
            var olderPeriod = _postingPeriods.Where(x => !x.Closed)
                                             .OrderBy(x => x.Date)
                                             .ToList()
                                             .FirstOrDefault().DeepClone();

            this.lstFromPeriod.DataSource = new List<PostingPeriod>() { olderPeriod };

            var availablePostingPeriods =
                await _financialService.GetAvailablePostingPeriodsForBeCreatedAsync(GlobalConfig.Company.Code);
            lstAbrirMes.DataSource = availablePostingPeriods;
        }


        private async void Btn_Create_PostingPeriod(object sender, EventArgs e)
        {
            try
            {
                btnGuardar.Enabled = false;
                await CreateNewPostingPeriodAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                await ReloadFormAsync(); 
                btnGuardar.Enabled = true;
            }
        }

        private async Task CreateNewPostingPeriodAsync()
        {
            var selectedItem = lstAbrirMes.SelectedItem as PostingPeriod;
            var selectedDate = selectedItem.Date; 

            var postingPeriod = new PostingPeriod()
            {
                Date = new DateTime(selectedDate.Year, selectedDate.Month, 1, 0, 0, 0, 0), 
                Closed = false, 
                CompanyId = GlobalConfig.Company.Code, 
                UpdatedBy = GlobalConfig.Usuario.Id
            }; 

            await _financialService.CreatePostingPeriodAsync(postingPeriod);
        }

        private void lstFromPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var startMonth = lstFromPeriod.SelectedItem as PostingPeriod;
            lstToPeriod.DataSource = _postingPeriods.Where(x => !x.Closed).ToList()
                                                    .GetOlder(startMonth.Date);
        }

        private async void BtnCerrarMes_Click(object sender, EventArgs e)
        {
            var amount = await ReporteEstadoResultadoIntegralDataAsync();

            if (MessageBox.Show($@"Se creará un cierre contable por {String.Format("{0:n}", amount.Amount)}", TextoGeneral.NombreApp,
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                await ClosePeriodProcessAsync(amount.Amount);
            }
        }

        private async Task ClosePeriodProcessAsync(decimal amount)
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
                CompanyId = GlobalConfig.Company.Code,
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
                await UiBusy.Run(this, async () =>
                {
                    await _financialService.ClosePostingPeriodAsync(savedModel);
                    await CreateNewPostingPeriodAsync();
                });
                txtBoxUserNotes.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                await ReloadFormAsync();
                btnCerrarMes.Enabled = true;
            }

        }

        private async Task ReloadFormAsync()
        {
            await LoadDropDownsAsync();
            await LoadDataGridsAsync();
        }
        

        private async Task<ClosurePostingPeriodBalance> ReporteEstadoResultadoIntegralDataAsync()
        {
            var firstDate = lstFromPeriod.SelectedItem as PostingPeriod;
            var endDate = lstToPeriod.SelectedItem as PostingPeriod;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Code,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            return await _financialReportService.PreviousClosurePostingPeriodBalanceAsync(reportParamns);
        }


    }
}
