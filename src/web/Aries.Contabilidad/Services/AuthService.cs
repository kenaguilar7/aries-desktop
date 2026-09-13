using System.Net.Http.Json;
using AriesContador.Core.Models.Users;

namespace Aries.Contabilidad.Services
{
    public class AuthService : BaseHttpService, IAuthService
    {
        private readonly ILocalStorageService _localStorageService;

        public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
            : base(httpClientFactory)
        {
            _localStorageService = localStorageService;
        }

        public async Task<WebToken?> LoginAsync(Login login)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", login, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<WebToken>(_jsonOptions);
                if (result == null || result.User == null || string.IsNullOrEmpty(result.Token))
                    return null;

                await _localStorageService.StoreAuthToken(result.Token);
                await _localStorageService.StoreCurrentUser(result.User);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveAuthToken();
            await _localStorageService.RemoveCurrentUser();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var token = await _localStorageService.GetAuthToken();
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }
    }
}
