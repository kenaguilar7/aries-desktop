using AriesContador.Core;
using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialServices
{
    public interface IJournalEntryLineService
    {
        Task<JournalEntryLine> GetJournalEntryLineById(int id);
        Task DeleteJournalEntryLine(JournalEntryLine journalEntryLine);
        Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryId(int journalEntryId);
        Task<int> CreateJournalEntryLine(JournalEntryLine journalEntryLine);
        Task UpdateJournalEntryLine(JournalEntryLine journalEntryLine);
        Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeleted(BasicReportParam reportParam);
        Task RestoreJournalEntryLine(JournalEntryLine journalEntryLine);
    }

    public class JournalEntryLineService : IJournalEntryLineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JournalEntryLineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<int> CreateJournalEntryLine(JournalEntryLine journalEntryLine)
            => _unitOfWork.JournalEntryLineRepository.AddAsyncWithReturnId(journalEntryLine);

        public async Task DeleteJournalEntryLine(JournalEntryLine journalEntryLine)
            => await _unitOfWork.JournalEntryLineRepository.Remove(journalEntryLine);

        public Task<IEnumerable<JournalEntryLineDeletedReport>> GetAllJournalEntryLineDeleted(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public Task<JournalEntryLine> GetJournalEntryLineById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryLine>> GetJournalEntryLineByJournalEntryId(int journalEntryId)
            => _unitOfWork.JournalEntryLineRepository.FindByJournalEntryIdAsync(journalEntryId);

        public Task RestoreJournalEntryLine(JournalEntryLine journalEntryLine)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJournalEntryLine(JournalEntryLine journalEntryLine)
            => _unitOfWork.JournalEntryLineRepository.UpdateAsync(journalEntryLine);
    }
}
