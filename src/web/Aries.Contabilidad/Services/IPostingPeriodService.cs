using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;

namespace Aries.Contabilidad.Services
{
    public interface IPostingPeriodService
    {
        Task<List<PostingPeriod>> GetPostingPeriodsAsync(string companyId);
        Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreatedAsync(string companyId);
        Task CreatePostingPeriodAsync(PostingPeriod postingPeriod);
        Task ClosePostingPeriodAsync(PostingPeriodEndClosing closing);
        Task<List<PostingPeriodInfoReport>> GetPostingPeriodInfoAsync(string companyId);
        Task<List<ClosingPostingPeriodReport>> GetClosingPostingPeriodReportAsync(string companyId);
        Task<ClosurePostingPeriodBalance> GetClosureBalanceAsync(BasicReportParam reportParam);
    }
}
