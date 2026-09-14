using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class PosAccountMapRepository : IPosAccountMapRepository
    {
        private readonly IConnectionString _connectionString;

        public PosAccountMapRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PosAccountMap> GetByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PosAccountMap, object>(
                PosAccountingQuery.SelectMapByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task UpsertAsync(PosAccountMap map, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var existing = await GetByCompanyIdAsync(map.CompanyId, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                map.Id = await dataAccess.InsertAndGetIdAsync(PosAccountingQuery.InsertMap, map, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            map.Id = existing.Id;
            await dataAccess.ExecuteTextAsync(PosAccountingQuery.UpdateMap, map, cancellationToken).ConfigureAwait(false);
        }
    }
}
