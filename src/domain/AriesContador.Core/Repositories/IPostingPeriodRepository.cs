using AriesContador.Core.Models.PostingPeriods;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IPostingPeriodRepository : IRepository<PostingPeriod>
    {
        Task<IEnumerable<PostingPeriod>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default);
    }
}
