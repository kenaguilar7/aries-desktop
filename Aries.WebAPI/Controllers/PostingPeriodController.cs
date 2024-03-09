using Aries.WebServices.FinancialServices;
using AriesContador.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Aries.WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostingPeriodController : ControllerBase
    {
        private readonly IPostingPeriodService _postingPeriodService;

        public PostingPeriodController(IPostingPeriodService postingPeriodService)
        {
            _postingPeriodService = postingPeriodService;
        }

        //[HttpDelete("Delete/{id}")]
        [HttpGet("GetPostingPeriods/{companyId}")]
        public async Task<IActionResult> GetPostingPeriods(string companyId)
        {
            var result = await _postingPeriodService.GetPostingPeriods(companyId);
            return Ok(result);
        }
    }

}
