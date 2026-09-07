using AriesContador.Core.Models.PostingPeriods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IPostingPeriodRepository : IRepository<PostingPeriod>
    {
        IEnumerable<PostingPeriod> FindByCompanyId(string companyId);
        Task<IEnumerable<PostingPeriod>> FindByCompanyIdAsync(string companyId);
        void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod); 
    }
}
