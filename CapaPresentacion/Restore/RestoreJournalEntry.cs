using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Textos;

namespace CapaPresentacion.Restore
{
    public partial class RestoreJournalEntry : Form
    {
        public List<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();
        private readonly IFinancialService _financialService;
        public RestoreJournalEntry()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
        }

        private void RestoreJournalEntry_Load(object sender, EventArgs e)
        {
            ConfigGridColumns();
            LoadStartPostingPeriod();
        }

        private void LoadStartPostingPeriod()
        {
            var lstPostingPe = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();
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

        private void LstEndPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (RestoreJournalsTabControl.SelectedIndex == 0)
                {
                    var output = JournalEntryLineReports();
                    JournalEntryLineGrid.DataSource = output;
                }
                else if (RestoreJournalsTabControl.SelectedIndex == 1)
                {
                    
                    var output = JournalEntryReports();
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

        private IEnumerable<JournalEntryLineDeletedReport> JournalEntryLineReports()
        {
            var firstDate = (PostingPeriod)lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod)lstEndPeriod.SelectedItem;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialService.GetAllJournalEntryLineDeleted(reportParamns);
            return output;
        }

        private IEnumerable<JournalEntryDeletedReport> JournalEntryReports()
        {
            var firstDate = (PostingPeriod)lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod)lstEndPeriod.SelectedItem;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialService.GetAllJournalEntryDeleted(reportParamns);
            return output;
        }

        private void JournalEntryLineGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var selectedItem = JournalEntryLineGrid.SelectedRows[0].DataBoundItem; 
                if (selectedItem is JournalEntryLineDeletedReport journalEntryLine)
                {
                    RestoreJournalEntryLine(journalEntryLine);
                    var sms = $"Entrada de asiento número: {journalEntryLine.JournalEntryNumber} \nCuenta: {journalEntryLine.AccountName} \nPeriodo contable: {journalEntryLine.PostingPeriodName} \nRecuperada exitosamente"; 
                    MessageBox.Show(sms, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information); 
                    LstEndPostingPeriod_SelectedIndexChanged(null, null);
                }
            }
        }

        private void JournalEntryGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var selectedItem = JournalEntryGrid.SelectedRows[0].DataBoundItem;
                if (selectedItem is JournalEntryDeletedReport journalEntry)
                {
                    RestoreJournalEntryService(journalEntry);
                    var sms = $"Asiento número: {journalEntry.JournalEntryNumber} \nPeriodo contable: {journalEntry.PostingPeriodName}  \nRecuperado exitosamente"; 
                    MessageBox.Show(sms, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LstEndPostingPeriod_SelectedIndexChanged(null, null);
                }
            }
        }

        private void RestoreJournalEntryService(JournalEntryDeletedReport journalEntryLine)
        {
            var jEL = new JournalEntry()
            {
                Id = journalEntryLine.JournalEntryId,
                UpdatedBy = GlobalConfig.Usuario.Id
            };

            _financialService.RestoreJournalEntry(jEL);
        }

        private void RestoreJournalEntryLine(JournalEntryLineDeletedReport journalEntryLine)
        {
            var jEL = new JournalEntryLine()
            {
                Id = journalEntryLine.JournalEntryLineId,
                UpdatedBy = GlobalConfig.Usuario.Id
            };

            _financialService.RestoreJournalEntryLine(jEL);
        }

        private void RestoreJournalsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            LstEndPostingPeriod_SelectedIndexChanged(null, null);
        }
    }
}
