using Aries.WebServices.FinancialServices;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.ReporteAuxiliaresModels;
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

        private async Task<Dictionary<PostingPeriod, List<Account>>> GetPostingPeriodAndAccountUsingIt(ReporteAuxiliarRequestBody requestBody)
        {
            var lst = new Dictionary<PostingPeriod, List<Account>>();
            
            foreach (var pPeriod in requestBody.PostingPeriods)
            {
                var accounts = await _accountService.GetAccountsBalance(requestBody.CompanyId, pPeriod, pPeriod);
                lst.Add(pPeriod, accounts.RemoveAccountWithOutBalances());
            }

            return lst; 
        }

        public async Task<ReporteAuxiliarResponse> Generate(ReporteAuxiliarRequestBody requestBody)
        {
            var reportResponse = new ReporteAuxiliarResponse();
            
            var reportData = await GetPostingPeriodAndAccountUsingIt(requestBody); 
            var combinedAccounts = reportData.GetUniqueAccountsByPeriod();

            var tableReport = GetReportTable(reportData);


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
     
                var column = 1;
                var row = 1;

                worksheet.Cell(row++, column).Value = $"{requestBody.CompanyId} {requestBody.CompanyId}";
                worksheet.Cell(row++, column).Value = $"Balance Auxiliares";
                worksheet.Cell(row++, column).Value = "usuario";

                List<string> Headers = new List<string>();

                var currencyProcessing = CurrencyProcessingFactory.GetCurrencyProcessing(requestBody.currencyTypeCompany);
                currencyProcessing.Process(ref worksheet, ref column, combinedAccounts, tableReport, out Headers);

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

        private Dictionary<PostingPeriod, Account[]> GetReportTable(IDictionary<PostingPeriod, List<Account>> allData)
        {
            var tablaCuentas = new Dictionary<PostingPeriod, Account[]>();

            foreach (var mes in allData)
            {
                Account[] cuentasConSaldo = allData.TransformUniqueAccounts();

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
                        cuenta.AccountTag = AccountTag.Activo;
                        cuenta.PriorBalance = 0.00m;
                        cuenta.PriorBalanceForeign = 0.00m;
                        cuenta.DebitBalance = 0.00m;
                        cuenta.CreditBalance = 0.00m;
                        cuenta.DebitBalanceForeign = 0.00m;
                        cuenta.CreditBalanceForeign = 0.00m;
                    }
                }

                tablaCuentas.Add(mes.Key, cuentasConSaldo);
            }
            return tablaCuentas;
        }
    }



}


