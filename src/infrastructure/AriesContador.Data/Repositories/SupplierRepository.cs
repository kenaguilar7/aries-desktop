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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IConnectionString _connectionString;

        public SupplierRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Supplier entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.InsertAndGetIdAsync(PurchasesQuery.InsertSupplier, entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Supplier entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PurchasesQuery.UpdateSupplier, entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveAsync(Supplier entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PurchasesQuery.DeactivateSupplier, entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Supplier> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Supplier, object>(
                PurchasesQuery.SelectSupplierById, new { Id = id }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<Supplier>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<Supplier, object>(
                PurchasesQuery.SelectSuppliersByCompany, new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Supplier> FindByNumberIdAsync(string companyId, string numberId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Supplier, object>(
                PurchasesQuery.SelectSupplierByNumberId,
                new { CompanyId = companyId, NumberId = numberId },
                cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }
    }
}
