using AriesContador.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aries.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly IAdministrationService administrationService;

        public CompanyController(IAdministrationService administrationService)
        {
            this.administrationService = administrationService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll() 
            => Ok(await administrationService.GetAllCompanies());
        
    }
}
