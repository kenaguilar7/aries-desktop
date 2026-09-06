using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialServices
{
    public interface IPostingPeriodService
    {
        Task CreatePostingPeriod(PostingPeriod postingPeriod);
        Task DeletePostingPeriod(PostingPeriod postingPeriod);
        Task<IEnumerable<PostingPeriod>> GetPostingPeriods(string companyId);
        Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreated(string companyId);
        Task UpdatePostingPeriod(PostingPeriod postingPeriod);
        Task ClosePostingPeriod(PostingPeriodEndClosing postingPeriod);
    }

    public class PostingPeriodService : IPostingPeriodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostingPeriodRepository _postingPeriodRepository;

        public PostingPeriodService(IPostingPeriodRepository postingPeriodRepository)
        {
            _postingPeriodRepository = postingPeriodRepository;
        }

        public Task ClosePostingPeriod(PostingPeriodEndClosing postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task CreatePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostingPeriod>> GetAvailablePostingPeriodsForBeCreated(string companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PostingPeriod>> GetPostingPeriods(string companyId)
        {
            var output = await _postingPeriodRepository.FindByCompanyIdAsync(companyId);
            return output.OrderBy(x => x.Date);
        }

        public Task UpdatePostingPeriod(PostingPeriod postingPeriod)
        {
            throw new NotImplementedException();
        }
    }
}
