using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public abstract class BaseHttpService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly ILogger? _logger;

        public static JsonSerializerOptions CreateJsonOptions() => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString
        };

        protected BaseHttpService(
            IHttpClientFactory httpClientFactory,
            ILogger? logger = null)
        {
            _httpClient = httpClientFactory.CreateClient("AriesAPI");
            _logger = logger;
            _jsonOptions = CreateJsonOptions();
        }
    }
}
