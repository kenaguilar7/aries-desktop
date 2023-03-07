using CapaEntidad.Textos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;
using ClosedXML.Excel;
using AriesContador.Core.Models.Patterns.Factories;
using AriesContador.Core.Models.Patterns.ActionsWorker;
using System.Threading.Tasks;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteEstadoResultadoIntegral : Form
    {
        List<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();
        private readonly IFinancialReportService _financialReportService;
        private readonly IFinancialService _financialService;
        private int _accountTreeDeep = 0;

        public ReporteEstadoResultadoIntegral()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialReportService = new FinancialReportService(unit);
            _financialService = new FinancialService(unit);
        }

        private void ReporteEstadoResultadoIntegral_Load(object sender, EventArgs e)
        {
            this.PostingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();
            this.lstStarPeriod.DataSource = PostingPeriods.DeepClone(); 
        }

        private void LstFirstPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var starMonth = (PostingPeriod)lstStarPeriod.SelectedItem;
            lstEndPeriod.DataSource = PostingPeriods.GetOlder(starMonth.Date); 
        }

        private void lstEndPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var report = ReporteEstadoResultadoIntegralData();
            var dt = ToDataTable(report.Results, report.TotalPeridaGanancia);
            
            GridDatos.DataSource = dt;
            foreach (DataGridViewColumn col in GridDatos.Columns)
            {
                if (dt.Columns[col.HeaderText] != null)
                    col.HeaderText = dt.Columns[col.HeaderText].Caption;
            }
            ConfigGridColumns();
        }

        private ResultReportEstadoResultadoIntegral ReporteEstadoResultadoIntegralData()
        {
            var firstDate = lstStarPeriod.SelectedItem as PostingPeriod;
            var endDate = lstEndPeriod.SelectedItem as PostingPeriod;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialReportService.EstadoResultadoIntegral(reportParamns);
            return output;
        }

        public DataTable ToDataTable(IEnumerable<EstadoResultadoIntegralReport> data, decimal totalAmount)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(EstadoResultadoIntegralReport));
            DataTable table = new DataTable();


            var deepTree = data.Max(x => x.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries).Length);
            _accountTreeDeep = deepTree;
            for (int i = 0; i < deepTree; i++)
            {
                var col = new DataColumn();
                col.DataType = System.Type.GetType("System.String");
                col.Caption = string.Empty;
                col.ColumnName = $"AccountName{i}";
                table.Columns.Add(col);
            }


            foreach (PropertyDescriptor prop in properties)
            {
                var col = new DataColumn();
                col.DataType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                col.Caption = prop.DisplayName;
                col.ColumnName = prop.Name;
                table.Columns.Add(col);
            }
            foreach (var item in data)
            {
                DataRow row = table.NewRow();

                var accountSlip = item.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries);
                var lastName = (item.IsMainAccount)? $"TOTAL {accountSlip.LastOrDefault()}":accountSlip.LastOrDefault();
                row[$"AccountName{accountSlip.Length - 1}"] = lastName;

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            DataRow totalRow = table.NewRow();

            totalRow[0] = "UTILIDAD/PERDIDA PERIODO";
            totalRow[nameof(EstadoResultadoIntegralReport.SaldoActual)] = totalAmount; 
            table.Rows.Add(totalRow); 

            return table;
        }

        private void ConfigGridColumns()
        {
            //Order columns
            for (int i = 0; i < _accountTreeDeep; i++)
            {
                var accountColumnName = $"AccountName{i}"; 
                GridDatos.Columns[accountColumnName].DisplayIndex = i; 
            }

            GridDatos.Columns[nameof(EstadoResultadoIntegralReport.SaldoActual)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(EstadoResultadoIntegralReport.AccountPath)].Visible = false;
            GridDatos.Columns[nameof(EstadoResultadoIntegralReport.Account)].Visible = false;
            GridDatos.Columns[nameof(EstadoResultadoIntegralReport.IsMainAccount)].Visible = false;
        }

        #region

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"Estado Resultado Integral {GlobalConfig.Company.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var param = new ReportResultadoPameter()
                        {
                            CompanyId= GlobalConfig.Company.Codigo,
                            FirstDate = lstStarPeriod.SelectedItem as PostingPeriod,
                            EndDate = lstEndPeriod.SelectedItem as PostingPeriod,
                            UserName = GlobalConfig.Usuario.ToString()
                        };
                        var actionReport = new ReportResultadoIntegralActions(_financialReportService, param, sfd.FileName); 
                        Task.Run(async ()=> { await actionReport.Execute();  } );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void tbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }

}
