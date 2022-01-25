using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Repositories
{
    public interface IPostingPeriodRepository : IRepository<PostingPeriod>
    {
        IEnumerable<PostingPeriod> FindByCompanyId(string companyId);
        void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod); 
    }
}
