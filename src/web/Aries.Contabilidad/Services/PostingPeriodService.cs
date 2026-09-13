using System.Net.Http.Json;
using AriesContador.Core.Models.PostingPeriods;
using Microsoft.Extensions.Logging;

namespace Aries.Contabilidad.Services
{
    public class PostingPeriodService : BaseHttpService, IPostingPeriodService
    {
        public PostingPeriodService(IHttpClientFactory httpClientFactory, ILogger<PostingPeriodService> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<PostingPeriod>> GetPostingPeriodsAsync(string companyId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<PostingPeriod>>(
                $"postingPeriod/GetPostingPeriods/{companyId}", _jsonOptions);
            return response ?? new List<PostingPeriod>();
        }
    }
}
