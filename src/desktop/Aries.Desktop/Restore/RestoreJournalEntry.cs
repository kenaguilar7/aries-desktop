using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using Aries.Reporting.Textos;

namespace Aries.Desktop.Restore
{
    public partial class RestoreJournalEntry : Form
    {
        public List<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();
        private readonly IFinancialService _financialService;
        public RestoreJournalEntry()
        {
            InitializeComponent();
        }

        public RestoreJournalEntry(IFinancialService financialService)
            : this()
        {
            _financialService = financialService;
        }

        private async void RestoreJournalEntry_Load(object sender, EventArgs e)
        {
            ConfigGridColumns();
            await LoadStartPostingPeriodAsync();
        }

        private async Task LoadStartPostingPeriodAsync()
        {
            var lstPostingPe = (await _financialService.GetPostingPeriodsAsync(GlobalConfig.Company.Code)).ToList();
            this.PostingPeriods = lstPostingPe;
            var lstP = lstPostingPe.OrderBy(p => p.Date);
            this.lstStarPeriod.DataSource = (from mm in lstP select mm).ToArray();
        }

        private void LstFirstPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var starMonth = (PostingPeriod)lstStarPeriod.SelectedItem;
            lstEndPeriod.SelectedItem = this.lstEndPeriod.DataSource = (from mm in PostingPeriods
                where mm.Year >= starMonth.Year && mm.Month >= starMonth.Month
                select mm).ToArray();
        }

        private async void LstEndPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (RestoreJournalsTabControl.SelectedIndex == 0)
                {
                    var output = await JournalEntryLineReportsAsync();
                    JournalEntryLineGrid.DataSource = output;
                }
                else if (RestoreJournalsTabControl.SelectedIndex == 1)
                {
                    
                    var output = await JournalEntryReportsAsync();
                    JournalEntryGrid.DataSource = output;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void ConfigGridColumns()
        {

            JournalEntryLineGrid.DataSource = new List<JournalEntryLineDeletedReport>(); 
            this.JournalEntryLineGrid.Columns[nameof(JournalEntryLineDeletedReport.JournalEntryLineId)].Visible = false;
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            JournalEntryLineGrid.Columns.Add(btn);
            btn.HeaderText = "";
            btn.Text = "Recuperar";
            btn.Name = "btn";
            btn.UseColumnTextForButtonValue = true;
            JournalEntryLineGrid.Columns[nameof(JournalEntryLineDeletedReport.DebitAmount)].DefaultCellStyle.Format = "₡#,0.00";
            JournalEntryLineGrid.Columns[nameof(JournalEntryLineDeletedReport.CreditAmount)].DefaultCellStyle.Format = "₡#,0.00";
            JournalEntryLineGrid.Columns[nameof(JournalEntryLineDeletedReport.RateAmount)].DefaultCellStyle.Format = "₡#,0.00";
            JournalEntryLineGrid.Columns[nameof(JournalEntryLineDeletedReport.ForeignAmount)].DefaultCellStyle.Format = "$#,0.00";


            JournalEntryGrid.DataSource = new List<JournalEntryDeletedReport>();
            this.JournalEntryGrid.Columns[nameof(JournalEntryDeletedReport.JournalEntryId)].Visible = false;
            DataGridViewButtonColumn btnJe = new DataGridViewButtonColumn();
            JournalEntryGrid.Columns.Add(btnJe);
            btnJe.HeaderText = "";
            btnJe.Text = "Recuperar";
            btnJe.Name = "btn";
            btnJe.UseColumnTextForButtonValue = true;
            JournalEntryGrid.Columns[nameof(JournalEntryDeletedReport.DebitAmount)].DefaultCellStyle.Format = "₡#,0.00";
            JournalEntryGrid.Columns[nameof(JournalEntryDeletedReport.CreditAmount)].DefaultCellStyle.Format = "₡#,0.00";

        }

        private async Task<IEnumerable<JournalEntryLineDeletedReport>> JournalEntryLineReportsAsync()
        {
            var firstDate = (PostingPeriod)lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod)lstEndPeriod.SelectedItem;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Code,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = await _financialService.GetAllJournalEntryLineDeletedAsync(reportParamns);
            return output;
        }

        private async Task<IEnumerable<JournalEntryDeletedReport>> JournalEntryReportsAsync()
        {
            var firstDate = (PostingPeriod)lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod)lstEndPeriod.SelectedItem;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Code,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = await _financialService.GetAllJournalEntryDeletedAsync(reportParamns);
            return output;
        }

        private async void JournalEntryLineGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var selectedItem = JournalEntryLineGrid.SelectedRows[0].DataBoundItem; 
                if (selectedItem is JournalEntryLineDeletedReport journalEntryLine)
                {
                    await RestoreJournalEntryLineAsync(journalEntryLine);
                    var sms = $"Entrada de asiento número: {journalEntryLine.JournalEntryNumber} \nCuenta: {journalEntryLine.AccountName} \nPeriodo contable: {journalEntryLine.PostingPeriodName} \nRecuperada exitosamente"; 
                    MessageBox.Show(sms, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information); 
                    LstEndPostingPeriod_SelectedIndexChanged(null, null);
                }
            }
        }

        private async void JournalEntryGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var selectedItem = JournalEntryGrid.SelectedRows[0].DataBoundItem;
                if (selectedItem is JournalEntryDeletedReport journalEntry)
                {
                    await RestoreJournalEntryServiceAsync(journalEntry);
                    var sms = $"Asiento número: {journalEntry.JournalEntryNumber} \nPeriodo contable: {journalEntry.PostingPeriodName}  \nRecuperado exitosamente"; 
                    MessageBox.Show(sms, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LstEndPostingPeriod_SelectedIndexChanged(null, null);
                }
            }
        }

        private async Task RestoreJournalEntryServiceAsync(JournalEntryDeletedReport journalEntryLine)
        {
            var jEL = new JournalEntry()
            {
                Id = journalEntryLine.JournalEntryId,
                UpdatedBy = GlobalConfig.Usuario.Id
            };

            await _financialService.RestoreJournalEntryAsync(jEL);
        }

        private async Task RestoreJournalEntryLineAsync(JournalEntryLineDeletedReport journalEntryLine)
        {
            var jEL = new JournalEntryLine()
            {
                Id = journalEntryLine.JournalEntryLineId,
                UpdatedBy = GlobalConfig.Usuario.Id
            };

            await _financialService.RestoreJournalEntryLineAsync(jEL);
        }

        private void RestoreJournalsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            LstEndPostingPeriod_SelectedIndexChanged(null, null);
        }
    }
}
