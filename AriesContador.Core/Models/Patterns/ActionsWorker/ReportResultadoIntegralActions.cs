using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Patterns.Command;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Services;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static AriesContador.Core.Models.Patterns.ActionsWorker.ConfigSheetHeaders;

namespace AriesContador.Core.Models.Patterns.ActionsWorker
{
    public class ReportResultadoIntegralActions : IActionClass
    {
        private readonly IFinancialReportService financialReportService;
        private readonly ReportResultadoPameter reportResultadoPameter;
        private readonly string path;

        public ReportResultadoIntegralActions(IFinancialReportService financialReportService,
            ReportResultadoPameter reportResultadoPameter, string path)
        {
            this.financialReportService = financialReportService;
            this.reportResultadoPameter = reportResultadoPameter;
            this.path = path;
        }
        public async Task Execute()
        {
            CommandWorker worker = new CommandWorker();
            var lst = new ReportEstadoResultadoIntegralData(financialReportService, reportResultadoPameter).Execute();
            var output = new ToDataTable(lst.Results, lst.TotalPeridaGanancia).Execute();
            var _accountTreeDeep = lst.Results.Max(x => x.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries).Length);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hoja1");
                worksheet.Cell(4, 1).InsertTable(output);
                worker.SetCommand(new RemoveColumnsExcelReport(worksheet, _accountTreeDeep));
                worker.SetCommand(new SetColumnsFormatExcelReport(worksheet, _accountTreeDeep));
                worker.SetCommand(new SetExcelSheetGeneralFormat(worksheet, _accountTreeDeep));
                worker.SetCommand(new ConfigSheetHeaders(worksheet, _accountTreeDeep));
                worker.SetCommand(new AddHeadExcelReport(worksheet, reportResultadoPameter));
                await worker.Run();
                workbook.SaveAs(path);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            await Task.CompletedTask;
        }
    }

    public class ToDataTable : IActionResult<DataTable>
    {
        private readonly IEnumerable<EstadoResultadoIntegralReport> data;
        private readonly decimal totalAmount;

        public ToDataTable(IEnumerable<EstadoResultadoIntegralReport> data, decimal totalAmount)
        {
            this.data = data;
            this.totalAmount = totalAmount;
        }
        public DataTable Execute()
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(EstadoResultadoIntegralReport));
            DataTable table = new DataTable();

            var deepTree = data.Max(x => x.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries).Length);
            var _accountTreeDeep = deepTree;
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

                var accountSplit = item.AccountPath.Split(new char[] { '¡' }, StringSplitOptions.RemoveEmptyEntries);
                var lastName = (item.IsMainAccount) ? $"TOTAL {accountSplit.LastOrDefault()}" : accountSplit.LastOrDefault();
                row[$"AccountName{accountSplit.Length - 1}"] = lastName;

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
    }

    public class ReportEstadoResultadoIntegralData : IActionResult<ResultReportEstadoResultadoIntegral>
    {
        private readonly IFinancialReportService financialReportService;
        private readonly ReportResultadoPameter reportResultadoPameter;

        public ReportEstadoResultadoIntegralData(IFinancialReportService financialReportService,
            ReportResultadoPameter reportResultadoPameter)
        {
            this.financialReportService = financialReportService;
            this.reportResultadoPameter = reportResultadoPameter;
        }

        public ResultReportEstadoResultadoIntegral Execute()
        {
            var firstDate = reportResultadoPameter.FirstDate;
            var endDate = reportResultadoPameter.EndDate;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = reportResultadoPameter.CompanyId,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            var output = financialReportService.EstadoResultadoIntegral(reportParamns);
            return output;
        }
    }

    public class ReportResultadoPameter
    {
        public string CompanyId { get; set; }
        public PostingPeriod FirstDate { get; set; }
        public PostingPeriod EndDate { get; set; }
        public string UserName { get; set; }
    }

    public class RemoveColumnsExcelReport : IActionClass
    {
        private readonly IXLWorksheet worksheet;
        private readonly int accountTreeDeep;

        public RemoveColumnsExcelReport(IXLWorksheet worksheet, int accountTreeDeep)
        {
            this.worksheet = worksheet;
            this.accountTreeDeep = accountTreeDeep;
        }
        public async Task Execute()
        {
            worksheet.Column(accountTreeDeep + 1).Delete();
            worksheet.Column(accountTreeDeep + 1).Delete();
            worksheet.Column(accountTreeDeep + 1).Delete();
            await Task.CompletedTask;
        }
    }

    public class SetColumnsFormatExcelReport : IActionClass
    {
        private readonly IXLWorksheet worksheet;
        private readonly int accountTreeDeep;

        public SetColumnsFormatExcelReport(IXLWorksheet worksheet, int accountTreeDeep)
        {
            this.worksheet = worksheet;
            this.accountTreeDeep = accountTreeDeep;
        }

        public async Task Execute()
        {
            worksheet.Column(accountTreeDeep + 1).Style.NumberFormat.Format = "₡#,##0.00";
            await Task.CompletedTask;
        }
    }
    public class SetExcelSheetGeneralFormat : IActionClass
    {
        private readonly IXLWorksheet worksheet;
        private readonly int accountTreeDeep;

        public SetExcelSheetGeneralFormat(IXLWorksheet worksheet, int accountTreeDeep)
        {
            this.worksheet = worksheet;
            this.accountTreeDeep = accountTreeDeep;
        }
        public async Task Execute()
        {
            worksheet.Tables.FirstOrDefault().Theme = XLTableTheme.None;
            worksheet.Tables.FirstOrDefault().ShowHeaderRow = false;
            worksheet.Tables.FirstOrDefault().ShowAutoFilter = false;
            worksheet.Columns().AdjustToContents();
            await Task.CompletedTask;
        }
    }

    public class ConfigSheetHeaders : IActionClass
    {
        private readonly IXLWorksheet worksheet;
        private readonly int accountTreeDeep;

        public ConfigSheetHeaders(IXLWorksheet worksheet, int accountTreeDeep)
        {
            this.worksheet = worksheet;
            this.accountTreeDeep = accountTreeDeep;
        }
        public async Task Execute()
        {
            var range2 = worksheet.Range(worksheet.Cell(4, 1).Address, worksheet.Cell(4, accountTreeDeep).Address);
            range2.Value = "Cuentas";
            range2.Merge();
            range2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(4, accountTreeDeep + 1).Value = "Saldo";

            for (int i = 1; i < accountTreeDeep; i++)
            {
                worksheet.Column(i).Width = 3;
            }

            await Task.CompletedTask;
        }

        public class AddHeadExcelReport : IActionClass
        {
            private readonly IXLWorksheet worksheet;
            private readonly ReportResultadoPameter pameter;

            public AddHeadExcelReport(IXLWorksheet worksheet,
                ReportResultadoPameter pameter)
            {
                this.worksheet = worksheet;
                this.pameter = pameter;
            }
            public async Task Execute()
            {
                var firstDate = pameter.FirstDate;
                var endDate = pameter.EndDate;

                worksheet.Cell(1, 1).Value = pameter.CompanyId;
                worksheet.Cell(2, 1).Value = $"Estado de resultado integral {firstDate.Date.ToString("Y")} a {endDate.Date.ToString("Y")}";
                worksheet.Cell(3, 1).Value = pameter.UserName;
                await Task.CompletedTask;
            }
        }
    }
}
