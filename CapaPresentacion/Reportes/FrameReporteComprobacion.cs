using CapaEntidad.Entidades.Cuentas;
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Reports;
using CapaEntidad.Utils;
using ClosedXML.Excel;

namespace CapaPresentacion.Reportes
{
    public partial class FrameReporteComprobación : Form
    {
        private List<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();
        private readonly IFinancialReportService _financialReportService;
        private readonly IFinancialService _financialService;
        private int _accountTreeDeep = 0; 

        public FrameReporteComprobación()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialReportService = new FinancialReportService(unit);
            _financialService = new FinancialService(unit);
        }

        private void FrameReporteComprobacion_Load(object sender, EventArgs e)
        {
            var lstPostingPe = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo).ToList();

            this.PostingPeriods = lstPostingPe;
            var lstP = lstPostingPe.OrderBy(p => p.Date);
            this.ToPeriod.DataSource = (from mm in lstP select mm).ToArray();
            if (ToPeriod.Items.Count > 0)
            {
                ToPeriod.SelectedIndex = 0;
            }

            this.FromPeriod.DataSource = lstPostingPe.ToArray();
        }

        private void FromPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var report = JournalEntryReports();
            var dt = ToDataTable(report);
            GridDatos.DataSource = dt;
            foreach (DataGridViewColumn col in GridDatos.Columns)
            {
                if(dt.Columns[col.HeaderText] != null)
                    col.HeaderText = dt.Columns[col.HeaderText].Caption;
            }
            ConfigGridColumns();
        }

        public DataTable ToDataTable(IEnumerable<BalanceComprobacionReport> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(BalanceComprobacionReport));
            DataTable table = new DataTable();

            
            var deepTree = data.Max(x=> x.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries).Length);
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
                
                var accountSlip = item.AccountPath.Split(new char[] {'¡'}, StringSplitOptions.RemoveEmptyEntries);
                var lastName = accountSlip.LastOrDefault();
                row[$"AccountName{accountSlip.Length - 1}"] = lastName; 

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private void ConfigGridColumns()
        {
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoAnteriorDeb)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoAnteriorCred)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoMensualDeb)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoMensualCred)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoActualCuentaDeb)].DefaultCellStyle.Format = "#,0.00";
            GridDatos.Columns[nameof(BalanceComprobacionReport.SaldoActualCuentaCred)].DefaultCellStyle.Format = "#,0.00";

            GridDatos.Columns[nameof(BalanceComprobacionReport.AccountPath)].Visible = false;
            GridDatos.Columns[nameof(BalanceComprobacionReport.Account)].Visible = false;
        }

        private void ExportToExcel(string path)
        {
            var output = ToDataTable(JournalEntryReports());

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hoja1");
                worksheet.Cell(4, 1).InsertTable(output);
                RemoveColumnsExcelReport(worksheet);
                SetColumnsFormatExcelReport(worksheet);
                SetExcelSheetGeneralFormat(worksheet);
                ConfigSheetHeaders(worksheet); 
                AddHeadExcelReport(worksheet);
                workbook.SaveAs(path);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        }

        private void ConfigSheetHeaders(IXLWorksheet ws)
        {

            var range2 = ws.Range(ws.Cell(4, 1).Address, ws.Cell(4, _accountTreeDeep).Address);
            range2.Value = "Cuentas";
            range2.Merge();
            range2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(4, _accountTreeDeep + 1).Value = "Saldo Anterior Débitos";
            ws.Cell(4, _accountTreeDeep + 2).Value = "Saldo Anterior Créditos";
            ws.Cell(4, _accountTreeDeep + 3).Value = "Saldo Mensual Débitos";
            ws.Cell(4, _accountTreeDeep + 4).Value = "Saldo Mensual Créditos";
            ws.Cell(4, _accountTreeDeep + 5).Value = "Saldo Cuenta Débitos";
            ws.Cell(4, _accountTreeDeep + 6).Value = "Saldo Cuenta Créditos";

            for (int i = 1; i < _accountTreeDeep; i++)
            {
                ws.Column(i).Width = 3; 
            }
        }

        private void AddHeadExcelReport(IXLWorksheet worksheet)
        {
            var firstDate = ToPeriod.SelectedItem as PostingPeriod;
            var endDate = FromPeriod.SelectedItem as PostingPeriod;

            worksheet.Cell(1, 1).Value = GlobalConfig.Company.ToString();
            worksheet.Cell(2, 1).Value = $"Reporte de comprobación de {firstDate.Date.ToString("Y")} a {endDate.Date.ToString("Y")}";
            worksheet.Cell(3, 1).Value = GlobalConfig.Usuario.ToString();
        }

        private void SetExcelSheetGeneralFormat(IXLWorksheet worksheet)
        {
            worksheet.Tables.FirstOrDefault().Theme = XLTableTheme.None;
            worksheet.Tables.FirstOrDefault().ShowHeaderRow = false;
            worksheet.Tables.FirstOrDefault().ShowAutoFilter = false;
            worksheet.Columns().AdjustToContents();
        }

        private void SetColumnsFormatExcelReport(IXLWorksheet worksheet)
        {
            worksheet.Column(_accountTreeDeep + 1).Style.NumberFormat.Format = "₡#,##0.00";
            worksheet.Column(_accountTreeDeep + 2).Style.NumberFormat.Format = "₡#,##0.00";

            worksheet.Column(_accountTreeDeep + 3).Style.NumberFormat.Format = "₡#,##0.00";
            worksheet.Column(_accountTreeDeep + 4).Style.NumberFormat.Format = "₡#,##0.00";

            worksheet.Column(_accountTreeDeep + 5).Style.NumberFormat.Format = "₡#,##0.00";
            worksheet.Column(_accountTreeDeep + 6).Style.NumberFormat.Format = "₡#,##0.00";
        }

        private void RemoveColumnsExcelReport(IXLWorksheet worksheet)
        {
            worksheet.Column(_accountTreeDeep + 1).Delete();
            worksheet.Column(_accountTreeDeep + 1).Delete();
        }

        private IEnumerable<BalanceComprobacionReport> JournalEntryReports()
        {
            var firstDate = ToPeriod.SelectedItem as PostingPeriod;
            var endDate = FromPeriod.SelectedItem as PostingPeriod;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = _financialReportService.BalanceComprobacionReport(reportParamns);
            return output;
        }

        private void BtnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"Balance de comprobación {GlobalConfig.Company.ToString()}" })
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

        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
