using AriesContador.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aries.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAdministrationService _administrationService;

        public AuthController(IConfiguration config, IAdministrationService administrationService)
        {
            _config = config;
            _administrationService = administrationService;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var users = _administrationService.GetAllUsers();
            if (users.Any(u => u.UserName == username && u.Password == password))
            {
                var tokenString = GenerateToken();
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }

        private string GenerateToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[]
                //{
                //  new Claim(ClaimTypes.Name, "name here"),
                //  new Claim(ClaimTypes.Email, "email@here")
                //}),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
