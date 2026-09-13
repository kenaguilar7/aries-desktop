using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubPointOfSaleService : IPointOfSaleService
    {
        public List<SalesRegister> Registers { get; } = new List<SalesRegister>();
        public SalesRegisterSession OpenSession { get; set; }

        public Task<IEnumerable<Product>> GetProductsAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<Product>());

        public Task<Product> FindProductAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Product>(null);

        public Task<Product> FindProductByBarcodeAsync(string companyId, string barcode, CancellationToken cancellationToken = default) =>
            Task.FromResult<Product>(null);

        public Task CreateProductAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteProductAsync(int id, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<SalesRegister>> GetRegistersAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<SalesRegister>>(Registers.Where(r => r.CompanyId == companyId).ToList());

        public Task<SalesRegister> FindRegisterAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Registers.FirstOrDefault(r => r.Id == id));

        public Task CreateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default)
        {
            register.Id = Registers.Count + 1;
            Registers.Add(register);
            return Task.CompletedTask;
        }

        public Task UpdateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteRegisterAsync(int id, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<SalesRegisterSession> GetOpenSessionAsync(int registerId, CancellationToken cancellationToken = default)
        {
            if (OpenSession != null && OpenSession.SalesRegisterId == registerId && !OpenSession.ClosedAt.HasValue)
                return Task.FromResult(OpenSession);
            return Task.FromResult<SalesRegisterSession>(null);
        }

        public Task<SalesRegisterSession> OpenSessionAsync(int registerId, decimal openingAmount, string notes, int userId, CancellationToken cancellationToken = default)
        {
            OpenSession = new SalesRegisterSession
            {
                Id = 11,
                SalesRegisterId = registerId,
                OpeningAmount = openingAmount,
                OpeningNotes = notes,
                CreatedBy = userId,
                OpenedAt = System.DateTime.Now
            };
            return Task.FromResult(OpenSession);
        }

        public Task<SalesRegisterSession> CloseSessionAsync(int registerId, decimal declaredAmount, string notes, int userId, CancellationToken cancellationToken = default)
        {
            if (OpenSession != null)
            {
                OpenSession.ClosedAt = System.DateTime.Now;
                OpenSession.DeclaredClosingAmount = declaredAmount;
                OpenSession.UpdatedBy = userId;
            }
            return Task.FromResult(OpenSession);
        }

        public Task<IEnumerable<SalesRegisterSession>> GetSessionHistoryAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<SalesRegisterSession>());

        public Task<IEnumerable<Sale>> GetSalesByCompanyAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<Sale>());

        public Task<IEnumerable<Sale>> GetSalesBySessionAsync(int sessionId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<Sale>());

        public Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default) =>
            Task.FromResult(sale);

        public Task<PosTodaySalesReport> GetTodaySalesAsync(string companyId, DateTime? day, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PosTodaySalesReport());

        public Task<IEnumerable<Product>> GetLowStockAsync(string companyId, decimal minimum, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<Product>());

        public Task<IEnumerable<SalesRegisterSession>> GetClosingsAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<SalesRegisterSession>());
    }
}
