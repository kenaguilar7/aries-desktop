using AriesContador.Core.Services;
using DocumentFormat.OpenXml.ExtendedProperties;
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

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id) 
        {
            try
            {
                
                var company = new AriesContador.Core.Models.Companies.Company{ Code = id }; 
                await administrationService.DeleteCompany(company); 
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("BuildCode")]
        public async Task<IActionResult> BuildNewCode() 
        {
            try
            {
                var code = await administrationService.GetCompanyConsecutive();
                return Ok(new { Code= code });
            }
            catch (Exception e)
            {
                throw;
            }
        }
        
    }
}
