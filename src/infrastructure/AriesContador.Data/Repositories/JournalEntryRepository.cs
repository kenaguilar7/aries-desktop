using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly IConnectionString _connectionString;
        public JournalEntryRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            var originalId = entity.Id;
            var lines = DetachLines(entity);
            try
            {
                using (var dataAccess = new MySqlDataAccess(_connectionString))
                {
                    try
                    {
                        await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                        entity.Id = await dataAccess.SaveDataInTransactionAsync<JournalEntry, int>("SP_InsertJournalEntry", entity, cancellationToken)
                            .ConfigureAwait(false);
                        foreach (var line in lines)
                        {
                            line.JournalEntryId = entity.Id;
                            line.Id = await dataAccess.SaveDataInTransactionAsync<JournalEntryLine, int>("SP_InsertJournalEntryLine", line, cancellationToken)
                                .ConfigureAwait(false);
                        }
                        await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch
                    {
                        await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                        entity.Id = originalId;
                        throw;
                    }
                }
            }
            finally
            {
                entity.JournalEntryLines = lines;
            }
        }

        public async Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int pstPeriodId, CancellationToken cancellationToken = default)
        {
            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                var output = await dataAccess.LoadDataInTransactionAsync<JournalEntry, dynamic>(
                    "SP_GetJournalEntryByPostingPeriodId", new { PostingPeriodId = pstPeriodId }, cancellationToken)
                    .ConfigureAwait(false);

                foreach (var jEntry in output)
                {
                    var jELines = await dataAccess.LoadDataInTransactionAsync<JournalEntryLine, dynamic>(
                        "SP_GetJournalEntryLineByJournalEntryId", new { JournalEntryId = jEntry.Id }, cancellationToken)
                        .ConfigureAwait(false);
                    jEntry.JournalEntryLines = jELines;
                }

                await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                return output;
            }
        }

        public async Task<JournalEntry> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<JournalEntry, dynamic>("SP_GetJournalEntryById", new { Id = id }, cancellationToken)
                .ConfigureAwait(false);
            return output.FirstOrDefault();
        }

        public async Task<int> GetConsecutiveNumberAsync(int postingPeriodId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var newNumber = await dataAccess.LoadDataAsync<int, dynamic>("SP_GetJournalEntryConsecutive", new { postingPeriodId }, cancellationToken)
                .ConfigureAwait(false);
            return newNumber.First();
        }

        public async Task<IEnumerable<JournalEntryDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<JournalEntryDeletedReport, BasicReportParam>("SP_GetJournalEntryDeletedBydDateRange", reportParam, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RestoreJournalEntryAsync(JournalEntry entryLine, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_RestoreJournalEntry", entryLine, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            entity.JournalEntryLines = new List<JournalEntryLine>();
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_DesactivateJournalEntry", entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            var repoEntities = entity.JournalEntryLines;
            entity.JournalEntryLines = new List<JournalEntryLine>();
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_UpdateJournalEntry", entity, cancellationToken).ConfigureAwait(false);
            entity.JournalEntryLines = repoEntities;
        }

        private static List<JournalEntryLine> DetachLines(JournalEntry entity)
        {
            var lines = entity.JournalEntryLines?.ToList() ?? new List<JournalEntryLine>();
            entity.JournalEntryLines = new List<JournalEntryLine>();
            return lines;
        }
    }
}
