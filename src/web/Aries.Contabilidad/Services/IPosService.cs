using AriesContador.Core.Models.PointOfSale;

namespace Aries.Contabilidad.Services
{
    public interface IPosService
    {
        Task<List<Product>> GetProductsAsync(string companyId);
        Task<Product?> FindByBarcodeAsync(string companyId, string barcode);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<List<SalesRegister>> GetRegistersAsync(string companyId);
        Task<SalesRegister> CreateRegisterAsync(SalesRegister register);
        Task UpdateRegisterAsync(SalesRegister register);
        Task DeleteRegisterAsync(int id);

        Task<CashRegisterStatus> GetStatusAsync(int registerId);
        Task<SalesRegisterSession> OpenAsync(OpenCashRegisterRequest request);
        Task<SalesRegisterSession> CloseAsync(CloseCashRegisterRequest request);
        Task<List<SalesRegisterSession>> GetHistoryAsync(string companyId);

        Task<List<Sale>> GetSalesByCompanyAsync(string companyId);
        Task<List<Sale>> GetSalesBySessionAsync(int sessionId);
        Task<Sale> CreateSaleAsync(Sale sale);

        Task<PosTodaySalesReport> GetTodaySalesAsync(string companyId, DateTime? date = null);
        Task<List<Product>> GetLowStockAsync(string companyId, decimal? minimum = null);
        Task<List<SalesRegisterSession>> GetClosingsAsync(string companyId);
    }
}
