using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> FindByIdsAsync(string companyId, IEnumerable<int> ids, CancellationToken cancellationToken = default);
        Task<Product> FindByBarcodeAsync(string companyId, string barcode, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> FindLowStockAsync(string companyId, decimal minimum, CancellationToken cancellationToken = default);
        Task<int> CountActiveByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    }
}
