using System.Text.Json;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using Microsoft.JSInterop;

namespace Aries.Contabilidad.Services
{
    public interface ILocalStorageService
    {
        Task StoreCompanyInLocalStorage(Company company);
        Task<Company?> GetStoredCompany();
        Task<string?> GetItem(string key);
        Task SetItem(string key, string value);
        Task RemoveItem(string key);
        Task StoreCurrentUser(User user);
        Task<User> GetCurrentUserSesion();
        Task<string?> GetAuthToken();
        Task StoreAuthToken(string token);
        Task RemoveAuthToken();
        Task RemoveCurrentUser();
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        public const string COMPANY_DATA_KEY = "company_data";
        public const string CURRENT_USER_KEY = "current_user_data";
        public const string AUTH_TOKEN_KEY = "auth_token";
        private readonly JsonSerializerOptions _jsonOptions;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _jsonOptions = BaseHttpService.CreateJsonOptions();
        }

        public async Task StoreCompanyInLocalStorage(Company company)
        {
            await SetItem(COMPANY_DATA_KEY, JsonSerializer.Serialize(company, _jsonOptions));
        }

        public async Task<Company?> GetStoredCompany()
        {
            var json = await GetItem(COMPANY_DATA_KEY);
            if (string.IsNullOrEmpty(json))
                return null;
            return JsonSerializer.Deserialize<Company>(json, _jsonOptions);
        }

        public async Task<string?> GetItem(string key)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetItem(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task StoreCurrentUser(User user)
        {
            await SetItem(CURRENT_USER_KEY, JsonSerializer.Serialize(user, _jsonOptions));
        }

        public async Task<User> GetCurrentUserSesion()
        {
            var userJson = await GetItem(CURRENT_USER_KEY);
            if (string.IsNullOrEmpty(userJson))
                return new User();
            return JsonSerializer.Deserialize<User>(userJson, _jsonOptions) ?? new User();
        }

        public async Task<string?> GetAuthToken()
        {
            return await GetItem(AUTH_TOKEN_KEY);
        }

        public async Task StoreAuthToken(string token)
        {
            await SetItem(AUTH_TOKEN_KEY, token);
        }

        public async Task RemoveAuthToken()
        {
            await RemoveItem(AUTH_TOKEN_KEY);
        }

        public async Task RemoveCurrentUser()
        {
            await RemoveItem(CURRENT_USER_KEY);
        }
    }
}
