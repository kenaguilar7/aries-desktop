using AriesContador.Core.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Services
{

    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string requestUri);
        Task<T> PostAsync<T, P>(string requestUri, P parameter);
        Task<T> PutAsync<T, P>(string requestUri, P parameter);
        Task<T> DeleteAsync<T>(string requestUri);
    }

    public class HttpClientService : IHttpClientService
    {
        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() => new HttpClient());

        public async Task<T> GetAsync<T>(string requestUri)
        {
            var url = new Uri(requestUri);
            try
            {
                _client.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.ApiToken.Token);
                var response = await _client.Value.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Failed to send GET request to {requestUri}: {e.Message}", e);
            }
        }

        public async Task<T> PostAsync<T, P>(string requestUri, P parameter)
        {
            var url = new Uri(requestUri);
            string jsonString = JsonConvert.SerializeObject(parameter, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                _client.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.ApiToken.Token);
                var response = await _client.Value.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Failed to send POST request to {requestUri}: {e.Message}", e);
            }
        }

        public async Task<T> PutAsync<T, P>(string requestUri, P parameter)
        {
            var url = new Uri(requestUri);
            string jsonString = JsonConvert.SerializeObject(parameter, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                _client.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.ApiToken.Token);
                var response = await _client.Value.PutAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Failed to send PUT request to {requestUri}: {e.Message}", e);
            }
        }

        public async Task<T> DeleteAsync<T>(string requestUri)
        {
            var url = new Uri(requestUri);
            try
            {
                _client.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.ApiToken.Token);
                var response = await _client.Value.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Failed to send DELETE request to {requestUri}: {e.Message}", e);
            }
        }
    }
}
