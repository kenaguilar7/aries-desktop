using AriesContador.Core.Models.Users;

namespace Aries.Contabilidad.Services
{
    public interface IAuthService
    {
        Task<WebToken?> LoginAsync(Login login);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
