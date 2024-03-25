using Aries.WebServices.FinancialReportsServices;
using AriesContador.Core.Models.ReporteAuxiliaresModels;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aries.WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FinancialReportController : ControllerBase
    {
        private readonly IReporteAuxiliaresService _reporteAuxiliaresService;

        public FinancialReportController(IReporteAuxiliaresService reporteAuxiliaresService)
        {
            _reporteAuxiliaresService = reporteAuxiliaresService;
        }
 
        [HttpPost("ReporteAuxiliares")]
        public async Task<IActionResult> GetExcelFile([FromBody] ReporteAuxiliarRequestBody requestBody)
        {
            var exc = await _reporteAuxiliaresService.Generate(requestBody);
            return Ok(exc); 
            //return File(exc, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SampleFile.xlsx");
        }
    }


}
