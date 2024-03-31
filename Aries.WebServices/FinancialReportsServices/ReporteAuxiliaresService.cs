using Aries.WebServices.FinancialServices;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.ReporteAuxiliaresModels;
using AriesContador.Core.Models.Utils;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
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
        
        public ReporteAuxiliaresService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ReporteAuxiliarResponse> Generate(ReporteAuxiliarRequestBody requestBody)
        {
            var reportResponse = new ReporteAuxiliarResponse();
            
            var reportData = await GetPostingPeriodAndAccountUsingIt(requestBody); 
            var combinedAccounts = reportData.GetUniqueAccountsByPeriod();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
     
                var column = 1;
                var row = 1;

                worksheet.Cell(row++, column).Value = requestBody.ReportHeader.CompanyName;
                worksheet.Cell(row++, column).Value = requestBody.ReportHeader.ReportName;
                worksheet.Cell(row++, column).Value = requestBody.ReportHeader.IssuerName;

                List<string> Headers = new List<string>();

                var currencyProcessing = CurrencyProcessingFactory.GetCurrencyProcessing(requestBody.CurrencyType);
                currencyProcessing.Process(ref worksheet, ref column, combinedAccounts, reportData.GetReportTable(), out Headers);

                reportResponse.AccountNamesColumnLength = column -1;

                reportResponse.ColumnsBalanceHeaderText = Headers; 

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    reportResponse.Report = stream.ToArray();
                }

                return reportResponse; 
            }
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

    }
}


