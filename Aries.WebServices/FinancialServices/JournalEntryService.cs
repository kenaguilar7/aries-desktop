using AriesContador.Core;
using AriesContador.Core.Models.JournalEntries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aries.WebServices.FinancialServices
{
    public interface IJournalEntryService
    {
        Task<int> CreateJournalEntryConsecutive(int postingPeriodId);
        Task DeleteJournalEntry(JournalEntry journalEntry);
        Task<IEnumerable<JournalEntry>> GetJournalEntries(int postingPeriodId);
        Task<JournalEntry> GetJournalEntryById(int id);
        Task<int> CreateJournalEntry(JournalEntry journalEntry);
        Task UpdateJournalEntry(JournalEntry journalEntry);
        Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeleted(BasicReportParam reportParam);
        Task RestoreJournalEntry(JournalEntry journalEntry);
        Task UpdatedJournalEntryPeriod(JournalEntry journalEntry);
    }

    public class JournalEntryService : IJournalEntryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JournalEntryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateJournalEntry(JournalEntry journalEntry)
            => await _unitOfWork.JournalEntryRepository.AddAsyncReturningId(journalEntry);


        public Task<int> CreateJournalEntryConsecutive(int postingPeriodId)
            => _unitOfWork.JournalEntryRepository.GetConsecutiveNumberAsync(postingPeriodId);

        public Task DeleteJournalEntry(JournalEntry journalEntry)
            => _unitOfWork.JournalEntryRepository.Remove(journalEntry); 

        public Task<IEnumerable<JournalEntryDeletedReport>> GetAllJournalEntryDeleted(BasicReportParam reportParam)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntry>> GetJournalEntries(int postingPeriodId)
            => _unitOfWork.JournalEntryRepository.FindByPostingPeriodIdAsync(postingPeriodId);  

        public Task<JournalEntry> GetJournalEntryById(int id)
        {
            throw new NotImplementedException();
        }

        public Task RestoreJournalEntry(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task UpdatedJournalEntryPeriod(JournalEntry journalEntry)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJournalEntry(JournalEntry journalEntry)
        => _unitOfWork.JournalEntryRepository.UpdateAsync(journalEntry); 
    }
}
