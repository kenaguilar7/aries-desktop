using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class JournalEntryLineRepository : IJournalEntryLineRepository
    {
        private readonly IConnectionString _connectionString;
        public JournalEntryLineRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            entity.Id = await AddAsyncWithReturnId(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JournalEntryLine>> FindByAccountIdAndPostingPeriodIdAsync(int accountId, int postingPeriodId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<JournalEntryLine, dynamic>(
                "SP_GetAllJournalEntyLineByAccoudIdAndPostingPeriodId", new { accountId, postingPeriodId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<JournalEntryLine>> FindByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<JournalEntryLine, dynamic>(
                "SP_GetJournalEntryLineByJournalEntryId", new { JournalEntryId = journalEntryId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<JournalEntryLine> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<JournalEntryLine, dynamic>("SP_GetJournalEntryLineById", new { Id = id }, cancellationToken)
                .ConfigureAwait(false);
            return output.FirstOrDefault();
        }

        public async Task RemoveAsync(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_DesactivateJournalEntryLine", entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_UpdateJournalEntryLine", entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JournalEntryLineDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<JournalEntryLineDeletedReport, dynamic>(
                "SP_GetJournalEntyLineDeletedByDateRange", reportParam, cancellationToken).ConfigureAwait(false);
        }

        public async Task RestoreJournalEntryLineAsync(JournalEntryLine entryLine, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_RestoreJournalEntryLine", entryLine, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> AddAsyncWithReturnId(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.SaveDataAsync<JournalEntryLine, int>("SP_InsertJournalEntryLine", entity, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
