using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class PurchasePostingRepository : IPurchasePostingRepository
    {
        private readonly IConnectionString _connectionString;

        public PurchasePostingRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PurchasePosting> GetByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<PurchasePosting, object>(
                PurchaseAccountingQuery.SelectPostingByPurchase, new { PurchaseId = purchaseId }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<PurchasePosting>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<PurchasePosting, object>(
                PurchaseAccountingQuery.SelectPostingsByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(PurchasePosting posting, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            posting.Id = await dataAccess.InsertAndGetIdAsync(PurchaseAccountingQuery.InsertPosting, posting, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
