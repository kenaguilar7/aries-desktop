using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core.Models;
using AriesContador.Services;
using Newtonsoft.Json;

namespace Aries.WebAPI.Tests
{
    public class TestHttpClientService : IHttpClientService
    {
        private readonly HttpClient _http;

        public TestHttpClientService(HttpClient http)
        {
            _http = http;
        }

        public Task<T> GetAsync<T>(string requestUri) =>
            Send<T>(HttpMethod.Get, requestUri, null);

        public Task<T> PostAsync<T, P>(string requestUri, P parameter) =>
            Send<T>(HttpMethod.Post, requestUri, parameter);

        public Task<T> PutAsync<T, P>(string requestUri, P parameter) =>
            Send<T>(HttpMethod.Put, requestUri, parameter);

        public async Task DeleteAsync(string requestUri)
        {
            using var request = Build(HttpMethod.Delete, requestUri, null);
            using var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private async Task<T> Send<T>(HttpMethod method, string requestUri, object body)
        {
            using var request = Build(method, requestUri, body);
            using var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return default;
            return JsonConvert.DeserializeObject<T>(json);
        }

        private HttpRequestMessage Build(HttpMethod method, string requestUri, object body)
        {
            var request = new HttpRequestMessage(method, ToRelative(requestUri));
            var token = EnvironmentVariable.ApiToken?.Token;
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }

        private static string ToRelative(string requestUri)
        {
            if (Uri.TryCreate(requestUri, UriKind.Absolute, out var absolute))
                return absolute.PathAndQuery;
            return requestUri;
        }
    }
}
