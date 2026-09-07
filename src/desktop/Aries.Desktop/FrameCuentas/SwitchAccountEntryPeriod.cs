using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;

namespace Aries.Desktop.FrameCuentas
{
    public partial class SwitchAccountEntryPeriod : Form
    {
        private readonly JournalEntry _journalEntry;
        private readonly PostingPeriod _postingPeriod;
        private readonly IFinancialService _financialService;
        public event Action<JournalEntry, PostingPeriod> FinishProcess;

        public SwitchAccountEntryPeriod(JournalEntry journalEntry, PostingPeriod postingPeriod, IFinancialService financialService)
        {
            _journalEntry = journalEntry;
            _postingPeriod = postingPeriod;
            InitializeComponent();
            _financialService = financialService;
        }

        private async void SwitchAccountEntryPeriod_Load(object sender, EventArgs e)
        {
            var lst = (await _financialService.GetPostingPeriodsAsync(GlobalConfig.Company.Code)).OrderByDescending(p => p.Date).ToList();
            lst.RemoveAll(p => p.Id == _postingPeriod.Id || p.Closed); 
            this.lstMesesAbiertos.DataSource = lst; 
            this.txtAccountName.Text = _journalEntry.Number.ToString();
            this.txtCurrentPeriod.Text = _postingPeriod.ToString(); 
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            this.btnSave.Enabled = false;
            try
            {
                var pp = lstMesesAbiertos.SelectedItem as PostingPeriod;
                _journalEntry.PostingPeriodId = pp.Id;
                await _financialService.UpdatedJournalEntryPeriodAsync(_journalEntry);
                FinishProcess.Invoke(_journalEntry,pp);
                this.Close();
            }
            catch (Exception exception)
            {
                this.btnSave.Enabled = true; 
                throw;
            }
        }
    }
}
