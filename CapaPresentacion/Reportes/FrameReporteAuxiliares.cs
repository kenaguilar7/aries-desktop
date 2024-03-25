using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Services;
using AriesContador.Services.Models.Reports;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class FrameReporteAuxiliares : Form
    {

        private IEnumerable<PostingPeriod> postingPeriods = new List<PostingPeriod>();
        private ReporteAuxiliarResponse _reporteAuxiliarResponse { get; set; } 
        private readonly IHttpFinancialReportService _httpFinancialReportService;
        private readonly IHttpFinancialService _financialService;

        public FrameReporteAuxiliares(IHttpFinancialReportService httpFinancialReportService,
            IHttpFinancialService financialService)
        {
            InitializeComponent();
            _httpFinancialReportService = httpFinancialReportService;
            _financialService = financialService;
        }

        private async void FrameReporteAuxiliares_Load(object sender, EventArgs e)
        {
            postingPeriods = await _financialService.GetPostingPeriods(GlobalConfig.Company.Code);
            this.lstMesInicio.DataSource = postingPeriods; 
        }

        private void lstMesInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var meses = (from n in postingPeriods where n.Date >= ((PostingPeriod)lstMesInicio.SelectedItem).Date select n).ToList<PostingPeriod>();
            lstMesFinal.DataSource = meses;
        }
        private async void btnGenerar_Click(object sender, EventArgs e)
            => await GenerarReport(); 

        private async Task GenerarReport()
        {
            try
            {
                var listaFiltrada = postingPeriods
                                    .Where(item => item.Date >= ((PostingPeriod)lstMesInicio.SelectedItem).Date &&
                                                   item.Date <= ((PostingPeriod)lstMesFinal.SelectedItem).Date)
                                    .ToList();

                _reporteAuxiliarResponse = await _httpFinancialReportService.GetAuxiliaresReport(new AuxiliaresReportReqBody()
                {
                    CompanyId = GlobalConfig.Company.Code,
                    PostingPeriods = listaFiltrada,
                    currencyTypeCompany = GlobalConfig.Company.CurrencyType
                });

                var dt = ConvertExcelToDataTable(_reporteAuxiliarResponse.Report);
                dtGridReportContainer
                    .DataSource = dt;

                SetGridStile(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            if(_reporteAuxiliarResponse == null)
                await GenerarReport(); 

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", Title = "Reporte auxiliares", FileName = $"REPORTE DE AUXILIARES {GlobalConfig.Company.ToString()} - {GlobalConfig.Company.IdNumber}" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, _reporteAuxiliarResponse.Report);
                    Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                }
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void SetGridStile()
        {
            for (int i = 0; i < numberOfColumns; i++)
            {
                if(i< _reporteAuxiliarResponse.NumberOfColumns) 
                {
                    dtGridReportContainer.Columns[i].HeaderText = string.Empty;
                }
                else
                {
                    dtGridReportContainer.Columns[i].DefaultCellStyle.Format = "#,0.00";
                }
            }
        }

        private int numberOfColumns = 0;

        public DataTable ConvertExcelToDataTable(byte[] excelData)
        {
            using (var stream = new MemoryStream(excelData))
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1); // Asume que quieres la primera hoja
                var dataTable = new DataTable();

                int numberOfColumns = worksheet.Row(7).CellsUsed().Count(); // Determina el número de columnas basado en la primera fila
                this.numberOfColumns = numberOfColumns; 
                for (int i = 0; i < numberOfColumns; i++)
                {
                    //var col = new DataColumn();

                    if (i >= _reporteAuxiliarResponse.NumberOfColumns)
                    {
                        dataTable.Columns.Add(_reporteAuxiliarResponse.ColumnsBalanceHeaderText[i-_reporteAuxiliarResponse.NumberOfColumns], typeof(decimal));
                    }
                    else
                    {
                        dataTable.Columns.Add("Column" + (i + 1), typeof(string)); // Agrega columnas genéricas (Column1, Column2, etc.)
                    }
                }

                foreach (IXLRow row in worksheet.RowsUsed().Skip(6)) // Usa RowsUsed() para ignorar filas completamente vacías
                {
                    var dataRow = dataTable.NewRow();
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        var cell = row.Cell(i + 1); 
                        dataRow[i] = cell.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }

                return dataTable;
            }
        }
    }
}
