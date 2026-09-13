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
    public class ProductRepository : IProductRepository
    {
        private readonly IConnectionString _connectionString;

        public ProductRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.InsertAndGetIdAsync(PosQuery.InsertProduct, entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PosQuery.UpdateProduct, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(Product entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PosQuery.DeactivateProduct, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Product, object>(PosQuery.SelectProductById, new { Id = id }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<Product, object>(
                PosQuery.SelectProductsByCompany, new { CompanyId = companyId }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Product> FindByBarcodeAsync(string companyId, string barcode, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<Product, object>(
                PosQuery.SelectProductByBarcode, new { CompanyId = companyId, Barcode = barcode }, cancellationToken)
                .ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> FindLowStockAsync(string companyId, decimal minimum, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<Product, object>(
                PosQuery.SelectLowStock, new { CompanyId = companyId, Minimum = minimum }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<int> CountActiveByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<int, object>(
                PosQuery.CountActiveProducts, new { CompanyId = companyId }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }
    }
}
