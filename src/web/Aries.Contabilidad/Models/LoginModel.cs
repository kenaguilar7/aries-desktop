using System.ComponentModel.DataAnnotations;

namespace Aries.Contabilidad.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }
} 