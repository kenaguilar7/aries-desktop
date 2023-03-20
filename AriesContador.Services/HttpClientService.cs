using AriesContador.Core.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Services
{
    public class HttpClientService
    {
        private static HttpClient _client = new HttpClient();
        private HttpClientService()
        {
        }

        public static async Task<T> GetAsync<T, P>(string requestUri, P parameter)
        {

            var url = new Uri(requestUri);
            
            string jsonString = JsonConvert.SerializeObject(parameter, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Crear instancia de HttpContent
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.Token);
                // Enviar solicitud POST con HttpContent en el cuerpo
                var response = await _client.PostAsync(url, httpContent);

                // Leer respuesta de la solicitud
                string responseJson = await response.Content.ReadAsStringAsync();

                var returnedObject = JsonConvert.DeserializeObject<T>(responseJson);
                return await Task.FromResult(returnedObject);
            }
            catch (Exception e)
            {
                throw;
            }

        }
        public static async Task<T> GetAsync<T>(string requestUri)
        {

            var url = new Uri(requestUri);

            //string jsonString = JsonConvert.SerializeObject(parameter, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Crear instancia de HttpContent
            //var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentVariable.ApiToken.Token);
                // Enviar solicitud POST con HttpContent en el cuerpo
                var response = await _client.GetAsync(url);

                // Leer respuesta de la solicitud
                string responseJson = await response.Content.ReadAsStringAsync();

                var returnedObject = JsonConvert.DeserializeObject<T>(responseJson);
                return await Task.FromResult(returnedObject);
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }

}
