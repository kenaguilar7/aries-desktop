using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class PurchaseAccountMapRepository : IPurchaseAccountMapRepository
    {
        private readonly IConnectionString _connectionString;

        public PurchaseAccountMapRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PurchaseAccountMap> GetByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PurchaseAccountMap, object>(
                PurchaseAccountingQuery.SelectMapByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task UpsertAsync(PurchaseAccountMap map, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var existing = await GetByCompanyIdAsync(map.CompanyId, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                map.Id = await dataAccess.InsertAndGetIdAsync(PurchaseAccountingQuery.InsertMap, map, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            map.Id = existing.Id;
            await dataAccess.ExecuteTextAsync(PurchaseAccountingQuery.UpdateMap, map, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
