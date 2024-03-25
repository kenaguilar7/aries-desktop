using Aries.WebServices.FinancialServices;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialReportsServices
{
    public interface IReporteAuxiliaresService
    {
        Task<ReporteAuxiliarResponse> Generate(ReporteAuxiliarRequestBody requestBody);
    }

    public class ReporteAuxiliaresService : IReporteAuxiliaresService
    {
        private readonly IAccountService _accountService;
        private readonly IPostingPeriodService _postingPeriodService;

        public ReporteAuxiliaresService(IAccountService accountService, IPostingPeriodService postingPeriodRepository)
        {
            _accountService = accountService;
            _postingPeriodService = postingPeriodRepository;
        }


        public async Task<ReporteAuxiliarResponse> Generate(ReporteAuxiliarRequestBody requestBody)
        {
            var reportResponse = new ReporteAuxiliarResponse();

            var lst = new Dictionary<PostingPeriod, List<Account>>();
            //var dummyPP = await _postingPeriodService.GetPostingPeriods(requestBody.CompanyId);

            //var resultado = dummyPP.Where(
            //    p => p.Date.Year == 2024 && (p.Date.Month == 1 || p.Date.Month == 2) ||
            //    p.Date.Year == 2023 && (p.Date.Month == 11 || p.Date.Month == 12)).ToList();


            foreach (var pPeriod in requestBody.PostingPeriods)
            {
                var accounts = await _accountService.GetAccountsBalance(requestBody.CompanyId, pPeriod, pPeriod);

                var accountsCleaner = accounts
                    .Where(x => x.HasBalances() || 
                    (!x.HasBalances() && x.AccountType == AccountType.Cuenta_Titulo))
                    .ToList();

                lst.Add(pPeriod, accountsCleaner);
            }

            var combinedAccounts = lst.SelectMany(kv => kv.Value)
                          .GroupBy(account => account.Id)
                          .Select(group => group.First())
                          .ToList();

            var tableReport = GetReportTable(combinedAccounts, lst);


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                ///Creamos el encabezado
                var column = 1;
                var row = 1;

                worksheet.Cell(row++, column).Value = $"{requestBody.CompanyId} {requestBody.CompanyId}";
                worksheet.Cell(row++, column).Value = $"Balance Auxiliares";
                worksheet.Cell(row++, column).Value = "usuario";

                List<string> Headers = new List<string>(); 

                switch (requestBody.currencyTypeCompany)
                {
                    case CurrencyTypeCompany.Dolares_y_Colones:
                        LLenarNombreCuentas(ref worksheet, 6, ref column, combinedAccounts);
                        LlenarSaldoCuentasColonesDolares(ref worksheet, 7, column, tableReport, out Headers);
                        break;
                    case CurrencyTypeCompany.Solo_Colones:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, combinedAccounts);
                        //LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        //LlenarSaldoCuentasColones(ref worksheet, 6, column, tablaCuentas);

                        break;
                    case CurrencyTypeCompany.Solo_Dolares:
                        LLenarNombreCuentas(ref worksheet, 5, ref column, combinedAccounts);
                        //LlenarTitulosUnaDivisa(ref worksheet, row, column, list);
                        //LlenarSaldoCuentasDolares(ref worksheet, 6, column, tablaCuentas);
                        //LlenarSaldoCuentasColonesDolares(worksheet, 7, column, tablaCuentas);

                        break;
                    default:
                        break;
                }


                // var direccion = @"balance_comprobacion_" + compañia.MyNombre.Replace(' ', '_') + ".xlsx"; 
                //workbook.SaveAs(direccion);
                ////Show report
                //Process.Start(new ProcessStartInfo(direccion) { UseShellExecute = true });
                reportResponse.NumberOfColumns = column -1;
                reportResponse.ColumnsBalanceHeaderText = Headers; 

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    reportResponse.Report = stream.ToArray();
                }

                return reportResponse; 
            }
        }

        private void LLenarNombreCuentas(ref IXLWorksheet worksheet, int row, ref int column, List<Account> list)
        {
            var quiebreNombreColumna = column;
            var quiebreRow = row;
            for (int i = 0; i < list.Count; i++)
            {
                var nombre = list[i].GetPathName(list);

                for (int j = 0; j < nombre.Length; j++)
                {
                    worksheet.Cell(i + row + 1, j + column).Value = nombre[j];
                    //hacemos una sumatoria del punto de quiebre 
                    //para obtener el mayor rago
                    //solo sumanos si es mayor
                    quiebreNombreColumna = (quiebreNombreColumna < j + column) ? j + column : quiebreNombreColumna;
                }

                quiebreRow = (quiebreRow < i + row) ? i + row : quiebreRow;
            }
            worksheet.Cell("A4").Value = "Cuentas";
            worksheet.Range("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(4, 1, row, quiebreNombreColumna).Merge();
            column = quiebreNombreColumna + 1;
        }

        private static void LlenarSaldoCuentasColonesDolares(
            ref IXLWorksheet worksheet, 
            int row, 
            int column, 
            Dictionary<PostingPeriod, Account[]> lstFechas, 
            out List<string> headers)
        {

            var rowFechas = row;
            var columnaQuiebre = column;
            List<string> header = new List<string>();
            
            foreach (var fecha in lstFechas)
            {
                header.Add($"{fecha.Key.ToString()}-COL");
                header.Add($"{fecha.Key.ToString()}-USD"); 
                

                ///Se escribe la fecha en el exel 
                ///
                worksheet.Cell(rowFechas - 2, column).Value = fecha.Key.ToString();
                worksheet.Cell(rowFechas - 2, column).Style.NumberFormat.Format = "MMM yyyy";

                worksheet.Cell(rowFechas - 2, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(rowFechas - 2, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(rowFechas - 2, column, rowFechas - 2, column + 1).Merge();

                worksheet.Cell(rowFechas - 1, column).Value = "COL";
                worksheet.Cell(rowFechas - 1, column + 1).Value = "USD";


                ///Ahora imprimos las cuentas
                var rowMonto = rowFechas;
                foreach (var cuenta in fecha.Value)
                {
                    worksheet.Cell(rowMonto, column).Value = cuenta.MontlyBalance;
                    worksheet.Cell(rowMonto, column).Style.NumberFormat.NumberFormatId = 4;
                    worksheet.Cell(rowMonto, column + 1).Value = cuenta.MontlyBalanceForeign;
                    worksheet.Cell(rowMonto, column + 1).Style.NumberFormat.NumberFormatId = 4;
                    rowMonto++;
                }
                column = column + 2;
            }
            headers = header; 
        }

        private Dictionary<PostingPeriod, Account[]> GetReportTable(List<Account> lstCuentas, IDictionary<PostingPeriod, List<Account>> allData)
        {
            var tablaCuentas = new Dictionary<PostingPeriod, Account[]>();

            foreach (var mes in allData)
            {
                Account[] cuentasConSaldo = lstCuentas.Select(c => new Account() { Name = c.Name, Id = c.Id, FatherAccount = c.FatherAccount }).ToArray();

                foreach (var cuenta in cuentasConSaldo)
                {
                    var cuentaMes = mes.Value.FirstOrDefault(cv => cv.Id == cuenta.Id);
                    if (cuentaMes != null)
                    {
                        cuenta.DebOrCred = cuentaMes.DebOrCred;
                        cuenta.AccountTag = cuentaMes.AccountTag;
                        cuenta.AccountType = cuentaMes.AccountType;
                        cuenta.PriorBalance = cuentaMes.PriorBalance;
                        cuenta.PriorBalanceForeign = cuentaMes.PriorBalanceForeign;
                        cuenta.DebitBalance = cuentaMes.DebitBalance;
                        cuenta.CreditBalance = cuentaMes.CreditBalance;
                        cuenta.DebitBalanceForeign = cuentaMes.DebitBalanceForeign;
                        cuenta.CreditBalanceForeign = cuentaMes.CreditBalanceForeign;
                    }
                    else
                    {
                        AsignarValoresPredeterminados(cuenta);
                    }
                }

                tablaCuentas.Add(mes.Key, cuentasConSaldo);
            }
            return tablaCuentas;
        }
        void AsignarValoresPredeterminados(Account cuenta)
        {
            cuenta.AccountTag = AccountTag.Activo;

            cuenta.PriorBalance = 0.00m;
            cuenta.PriorBalanceForeign = 0.00m;
            cuenta.DebitBalance = 0.00m;
            cuenta.CreditBalance = 0.00m;
            cuenta.DebitBalanceForeign = 0.00m;
            cuenta.CreditBalanceForeign = 0.00m;
        }
    }

    public class ReporteAuxiliarRequestBody
    {
        public List<PostingPeriod> PostingPeriods { get; set; }
        public string CompanyId { get; set; }
        public CurrencyTypeCompany currencyTypeCompany { get; set; }
    }


    public class ReporteAuxiliarResponse
    {
        public byte[] Report { get; set; }
        public int HeadersEntAt { get; set; }
        /// <summary>
        /// Numer of columns in base 0
        /// </summary>
        public int NumberOfColumns { get; set; }

        public List<string> ColumnsBalanceHeaderText { get; set; } = new List<string>(); 
    }

}


