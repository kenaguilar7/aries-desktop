using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Reportes;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Utils;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteAsientos : Form
    {
        private List<JournalEntry> _listaDeAsientos;
        public List<PostingPeriod> fechaTransaccions { get; set; } = new List<PostingPeriod>();
        public void Commit(){ lstStarPeriod.DataSource = fechaTransaccions; }

        private readonly IFinancialReportService _financialReportService;
        private readonly IFinancialService _financialService;

        private List<JournalEntry> ListaDeAsientos
        {
            get { return _listaDeAsientos; }
            set { _listaDeAsientos = value; }
        }
        private AsientoCL _asientoCL = new AsientoCL();
        
        public ReporteAsientos()
        {
            InitializeComponent();

            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialReportService = new FinancialReportService(unit);
            _financialService = new FinancialService(unit);
            //if (GlobalConfig.Company.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            //{
            //    money_chance.Visible = false;
            //    balance_usd.Visible = false;
            //    money_type.Visible = false; 
            //}

        }
        private void ReporteAsientos_Load(object sender, EventArgs e)
        {
            var lstPostingPe = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();
            this.fechaTransaccions = lstPostingPe;

            var lstP = lstPostingPe.OrderBy(p => p.Date);
            var startP = lstP.FirstOrDefault();
            var endP = lstP.LastOrDefault();

            this.lstStarPeriod.DataSource = (from mm in lstP select mm).ToArray();
            //this.lstEndPeriod.DataSource = (from mm in lstP select mm).ToArray();
        }
        

        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            var starMonth = (PostingPeriod) lstStarPeriod.SelectedItem;
            lstEndPeriod.SelectedItem = this.lstEndPeriod.DataSource = (from mm in fechaTransaccions
                where mm.Year >= starMonth.Year && mm.Month >= starMonth.Month
                select mm).ToArray();
        }


        private void lstEndPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            var firstDate = (PostingPeriod)lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod)lstEndPeriod.SelectedItem; 

            var tes = new JournalEntryReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialReportService.JournalEntryReport(tes);
            var bindingList = new BindingList<JournalEntryReport>(output.ToList());
            var source = new BindingSource(bindingList, null);
            GridDatos.DataSource = source;
            GridDatos.Columns[nameof(JournalEntryReport.DebitAmount)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(JournalEntryReport.CreditAmount)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(JournalEntryReport.RateAmount)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(JournalEntryReport.ForeignAmount)].DefaultCellStyle.Format = "#,0.00";
        }

        private void CargarTabla()
        {

            GridDatos.Rows.Clear();
            foreach (var c in ListaDeAsientos)
            {
                
                foreach (var item in c.JournalEntryLines)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(GridDatos);

                    row.Cells[0].Value = c.PostingPeriodId;
                    row.Cells[1].Value = c.Number;
                    row.Cells[2].Value = item.AccountId;
                    row.Cells[3].Value = item.Reference;
                    row.Cells[4].Value = item.Memo;
                    row.Cells[5].Value = item.Date;
                    row.Cells[6].Value = (item.DebOrCred == DebOrCred.Debito) ? item.Monto : 0.00m;
                    row.Cells[7].Value = (item.DebOrCred == DebOrCred.Credito) ? item.Monto : 0.00m;
                    row.Cells[8].Value = item.Currency;
                    row.Cells[9].Value = item.RateAmount;
                    row.Cells[10].Value = (item.Currency == Currency.dolares) ? item.Monto / item.RateAmount : 0.00m;

                    GridDatos.Rows.Add(row);
                }

            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {

            try
            {
                
                //if (ListaDeAsientos == null)
                //{
                //    throw new Exception("La lista se encuentra vacia!");
                //}
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE DE ASIENTOS {GlobalConfig.Company.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteAsiento.GenerarReporte(ListaDeAsientos, GlobalConfig.Company, GlobalConfig.Usuario, GlobalConfig.Company.TipoMoneda, sfd.FileName, ((DataTable)GridDatos.DataSource));
                        Convert(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                lstStarPeriod.Enabled = false;
                //GridDatos.DataSource = GridDatos.DataSource = _asientoCL.ReporteAsientos(GlobalConfig.Compañia, (FechaTransaccion)lstMesesAbiertos.SelectedItem, true);
                //GridDatos.DataSource = _financialService.
            }
            else
            {
                lstStarPeriod.Enabled = true;
                //GridDatos.DataSource = _asientoCL.ReporteAsientos(GlobalConfig.Compañia, (FechaTransaccion)lstMesesAbiertos.SelectedItem, false);
            }
            
        }
        private void Convert() {

            foreach (DataRow item in ((DataTable)GridDatos.DataSource).Rows)
            {
                Object[] vs = item.ItemArray;
                
            }
        }

    }

}
