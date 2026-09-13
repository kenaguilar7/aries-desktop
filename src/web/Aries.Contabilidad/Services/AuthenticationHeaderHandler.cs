using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace Aries.Contabilidad.Services
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TOKEN_KEY = "auth_token";

        public AuthenticationHeaderHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // If we can't get the token, just proceed with the request
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
} 