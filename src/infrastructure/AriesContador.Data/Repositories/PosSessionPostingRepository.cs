using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class PosSessionPostingRepository : IPosSessionPostingRepository
    {
        private readonly IConnectionString _connectionString;

        public PosSessionPostingRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PosSessionPosting> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PosSessionPosting, object>(
                PosAccountingQuery.SelectPostingBySession, new { SessionId = sessionId }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<PosSessionPosting>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<PosSessionPosting, object>(
                PosAccountingQuery.SelectPostingsByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(PosSessionPosting posting, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            posting.Id = await dataAccess.InsertAndGetIdAsync(PosAccountingQuery.InsertPosting, posting, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
