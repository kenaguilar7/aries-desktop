using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class PostingPeriodRepository : IPostingPeriodRepository
    {
        private readonly IConnectionString _connectionString;
        public PostingPeriodRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(PostingPeriod entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.SaveDataAsync<PostingPeriod, int>("SP_InsertPostingPeriod", entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var saveMyPostingPeriod = postingPeriod.PostingPeriods;
            try
            {
                await dataAccess.StartTransactionAsync(cancellationToken).ConfigureAwait(false);
                foreach (var period in saveMyPostingPeriod)
                {
                    await dataAccess.SaveDataInTransactionAsync("SP_ClosePeriod", period, cancellationToken)
                        .ConfigureAwait(false);
                }

                postingPeriod.PostingPeriods = null;
                postingPeriod.Id = await dataAccess.SaveDataInTransactionAsync<PostingPeriodEndClosing, int>(
                    "SP_InsertClosingPostingPeriod",
                    postingPeriod,
                    cancellationToken).ConfigureAwait(false);
                postingPeriod.PostingPeriods = saveMyPostingPeriod;

                await dataAccess.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                postingPeriod.PostingPeriods = saveMyPostingPeriod;
                await dataAccess.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task<IEnumerable<PostingPeriod>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<PostingPeriod, dynamic>("SP_GetAllPostingPeriod", new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveAsync(PostingPeriod entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(
                "UPDATE accounting_months SET active = 0, updated_by = @UpdatedBy, updated_at = NOW() WHERE accounting_months_id = @Id",
                new { entity.Id, entity.UpdatedBy },
                cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(PostingPeriod entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(
                "UPDATE accounting_months SET closed = @ClosedMySQL, updated_by = @UpdatedBy, updated_at = NOW() WHERE accounting_months_id = @Id",
                new { entity.Id, entity.ClosedMySQL, entity.UpdatedBy },
                cancellationToken).ConfigureAwait(false);
        }
    }
}
