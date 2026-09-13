using System.Security.Claims;
using System.Text.Json;
using AriesContador.Core.Models.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Aries.Contabilidad.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IAuthService _authService;
        private const string TOKEN_KEY = "auth_token";
        private const string USER_KEY = "current_user_data";
        private readonly JsonSerializerOptions _jsonOptions = BaseHttpService.CreateJsonOptions();

        public CustomAuthStateProvider(IJSRuntime jsRuntime, IAuthService authService)
        {
            _jsRuntime = jsRuntime;
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_KEY);

                if (string.IsNullOrEmpty(token))
                    return Anonymous();

                var user = string.IsNullOrEmpty(userJson)
                    ? null
                    : JsonSerializer.Deserialize<User>(userJson, _jsonOptions);
                if (user == null)
                    return Anonymous();

                return Authenticated(user);
            }
            catch
            {
                return Anonymous();
            }
        }

        public Task MarkUserAsAuthenticated(WebToken token)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(Authenticated(token.User)));
            return Task.CompletedTask;
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _authService.LogoutAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
        }

        private static AuthenticationState Anonymous() =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        private static AuthenticationState Authenticated(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
    }
}
