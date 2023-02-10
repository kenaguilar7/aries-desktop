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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using ClosedXML.Excel;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteAsientos : Form
    {
        public List<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();

        private readonly IFinancialReportService _financialReportService;
        private readonly IFinancialService _financialService;

        public ReporteAsientos()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialReportService = new FinancialReportService(unit);
            _financialService = new FinancialService(unit);
        }

        private void ReporteAsientos_Load(object sender, EventArgs e)
        {
            this.PostingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();
            this.lstStarPeriod.DataSource = this.PostingPeriods.DeepClone(); 
        }

        private void LstFirstPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var starMonth = (PostingPeriod) lstStarPeriod.SelectedItem;
            lstEndPeriod.DataSource = PostingPeriods.GetOlder(starMonth.Date); 
        }

        private void LstEndPostingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var output = JournalEntryReports(); 
            GridDatos.DataSource = output;
            ConfigGridColumns();
        }

        private void ConfigGridColumns()
        {
            var companyMoneyType = GlobalConfig.Company.TipoMoneda;
            if (companyMoneyType == TipoMonedaCompañia.Solo_Dolares)
            {
                GridDatos.Columns[nameof(JournalEntryReport.DebitAmount)].DefaultCellStyle.Format = "$#,0.00";
                GridDatos.Columns[nameof(JournalEntryReport.CreditAmount)].DefaultCellStyle.Format = "$#,0.00";
            }
            else
            {
                GridDatos.Columns[nameof(JournalEntryReport.DebitAmount)].DefaultCellStyle.Format = "₡#,0.00";
                GridDatos.Columns[nameof(JournalEntryReport.CreditAmount)].DefaultCellStyle.Format = "₡#,0.00";
            }

            GridDatos.Columns[nameof(JournalEntryReport.RateAmount)].DefaultCellStyle.Format = "₡#,0.00";
            GridDatos.Columns[nameof(JournalEntryReport.ForeignAmount)].DefaultCellStyle.Format = "$#,0.00";

            if (companyMoneyType == TipoMonedaCompañia.Solo_Colones)
            {
                GridDatos.Columns[nameof(JournalEntryReport.RateAmount)].Visible = false;
                GridDatos.Columns[nameof(JournalEntryReport.ForeignAmount)].Visible = false;
            }
        }

        private IEnumerable<JournalEntryReport> JournalEntryReports()
        {
            var firstDate = (PostingPeriod) lstStarPeriod.SelectedItem;
            var endDate = (PostingPeriod) lstEndPeriod.SelectedItem;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialReportService.JournalEntryReport(reportParamns);
            return output;
        }

        private void BtnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE DE ASIENTOS {GlobalConfig.Company.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ExportToExcel(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(string path)
        {
            var output = JournalEntryReports();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hoja1");
                worksheet.Cell(4, 1).InsertTable(output);
                SetColumnsFormatExcelReport(worksheet);
                RemoveColumnsExcelReport(worksheet);
                SetExcelSheetGeneralFormat(worksheet);
                AddHeadExcelReport(worksheet);
                workbook.SaveAs(path);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        }

        private void AddHeadExcelReport(IXLWorksheet worksheet)
        {
            worksheet.Cell(1,1).Value = GlobalConfig.Company.ToString();
            worksheet.Cell(2, 1).Value = "Reporte Asientos";
            worksheet.Cell(3, 1).Value = GlobalConfig.Usuario.ToString();
        }

        private void SetExcelSheetGeneralFormat(IXLWorksheet worksheet)
        {
            worksheet.Tables.FirstOrDefault().Theme = XLTableTheme.None;
            worksheet.Tables.FirstOrDefault().ShowAutoFilter = false;
            worksheet.Columns().AdjustToContents();
        }

        private void SetColumnsFormatExcelReport(IXLWorksheet worksheet)
        {
            var companyMoneyType = GlobalConfig.Company.TipoMoneda;
            if (companyMoneyType == TipoMonedaCompañia.Solo_Dolares)
            {
                worksheet.Column(8).Style.NumberFormat.Format = "$#,##0.00";
                worksheet.Column(7).Style.NumberFormat.Format = "$#,##0.00";
            }
            else
            {
                worksheet.Column(8).Style.NumberFormat.Format = "₡#,##0.00";
                worksheet.Column(7).Style.NumberFormat.Format = "₡#,##0.00";
            }

            // General text
            worksheet.Column(4).Style.NumberFormat.NumberFormatId = 0;
            worksheet.Column(4).Style.NumberFormat.NumberFormatId = 0;

            worksheet.Column(10).Style.NumberFormat.Format = "₡#,##0.00";
            worksheet.Column(11).Style.NumberFormat.Format = "$#,##0.00";
        }

        private void RemoveColumnsExcelReport(IXLWorksheet worksheet)
        {
            var companyMoneyType = GlobalConfig.Company.TipoMoneda;
            if (companyMoneyType == TipoMonedaCompañia.Solo_Colones)
            {
                worksheet.Column(9).Delete();
                worksheet.Column(10).Delete();
                worksheet.Column(11).Delete();
            }
        }

        private void CheckBoxShowAllPeriods_Checked(object sender, EventArgs e)
        {
            var userCanSelectMonths = !CheckBoxShowAllPeriods.Checked;

            lstStarPeriod.Enabled = userCanSelectMonths;
            lstEndPeriod.Enabled = userCanSelectMonths;

            if (!userCanSelectMonths && lstStarPeriod.Items?.Count > 0)
            {
                lstStarPeriod.SelectedIndex = 0;
                lstEndPeriod.SelectedIndex = lstEndPeriod.Items.Count - 1;
            }
        }

        private void CloseWindows(object sender, EventArgs e) => this.Close();
    }

}
