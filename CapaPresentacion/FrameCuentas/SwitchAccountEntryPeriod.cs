using System;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;

namespace CapaPresentacion.FrameCuentas
{
    public partial class SwitchAccountEntryPeriod : Form
    {
        private readonly JournalEntry _journalEntry;
        private readonly PostingPeriod _postingPeriod;
        private readonly FinancialService _financialService;
        //public event EventHandler FinishProcess;
        public event Action<JournalEntry, PostingPeriod> FinishProcess; 



        public SwitchAccountEntryPeriod(JournalEntry journalEntry, PostingPeriod postingPeriod)
        {
            _journalEntry = journalEntry;
            _postingPeriod = postingPeriod;
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
        }

        private void SwitchAccountEntryPeriod_Load(object sender, EventArgs e)
        {
            var lst = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).OrderByDescending(p => p.Date).ToList();
            lst.RemoveAll(p => p.Id == _postingPeriod.Id || p.Closed); 
            this.lstMesesAbiertos.DataSource = lst; 
            this.txtAccountName.Text = _journalEntry.Number.ToString();
            this.txtCurrentPeriod.Text = _postingPeriod.ToString(); 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.btnSave.Enabled = false;
            try
            {
                var pp = lstMesesAbiertos.SelectedItem as PostingPeriod;
                _journalEntry.PostingPeriodId = pp.Id;
                _financialService.UpdatedJournalEntryPeriod(_journalEntry);
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
