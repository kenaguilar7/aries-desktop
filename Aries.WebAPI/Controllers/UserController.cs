using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Aries.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAdministrationService administrationService;

        public UserController(IAdministrationService administrationService) 
        {
            this.administrationService = administrationService;
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var list = administrationService.GetAllUsers();
            return Ok(list); 
        } 
    }
}
