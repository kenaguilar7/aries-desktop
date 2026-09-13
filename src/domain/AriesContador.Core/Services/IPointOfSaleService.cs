using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;

namespace AriesContador.Core.Services
{
    public interface IPointOfSaleService
    {
        Task<IEnumerable<Product>> GetProductsAsync(string companyId, CancellationToken cancellationToken = default);
        Task<Product> FindProductAsync(int id, CancellationToken cancellationToken = default);
        Task<Product> FindProductByBarcodeAsync(string companyId, string barcode, CancellationToken cancellationToken = default);
        Task CreateProductAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<SalesRegister>> GetRegistersAsync(string companyId, CancellationToken cancellationToken = default);
        Task<SalesRegister> FindRegisterAsync(int id, CancellationToken cancellationToken = default);
        Task CreateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default);
        Task UpdateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default);
        Task DeleteRegisterAsync(int id, CancellationToken cancellationToken = default);

        Task<SalesRegisterSession> GetOpenSessionAsync(int registerId, CancellationToken cancellationToken = default);
        Task<SalesRegisterSession> OpenSessionAsync(int registerId, decimal openingAmount, string notes, int userId, CancellationToken cancellationToken = default);
        Task<SalesRegisterSession> CloseSessionAsync(int registerId, decimal declaredAmount, string notes, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesRegisterSession>> GetSessionHistoryAsync(string companyId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Sale>> GetSalesByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetSalesBySessionAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default);

        Task<PosTodaySalesReport> GetTodaySalesAsync(string companyId, DateTime? day, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLowStockAsync(string companyId, decimal minimum, CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesRegisterSession>> GetClosingsAsync(string companyId, CancellationToken cancellationToken = default);
    }
}
