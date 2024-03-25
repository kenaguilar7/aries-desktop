using AriesContador.Core.Models;
using AriesContador.Services.Models.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Services
{

    public interface IHttpFinancialReportService
    {
        Task<ReporteAuxiliarResponse> GetAuxiliaresReport(AuxiliaresReportReqBody reqBody);
    }

    public class HttpFinancialReportService : IHttpFinancialReportService
    {
        private readonly IHttpClientService _httpClient;

        public HttpFinancialReportService(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ReporteAuxiliarResponse> GetAuxiliaresReport(AuxiliaresReportReqBody reqBody)
        {
            try
            {

                return await _httpClient
                    .PostAsync<ReporteAuxiliarResponse, AuxiliaresReportReqBody>
                    (string.Concat(EnvironmentVariable.ApiUrl, $"FinancialReport/ReporteAuxiliares"), reqBody);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
