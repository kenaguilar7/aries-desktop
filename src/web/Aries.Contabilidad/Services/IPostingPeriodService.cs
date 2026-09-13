using AriesContador.Core.Models.PostingPeriods;

namespace Aries.Contabilidad.Services
{
    public interface IPostingPeriodService
    {
        Task<List<PostingPeriod>> GetPostingPeriodsAsync(string companyId);
    }
}
