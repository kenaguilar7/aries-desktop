using AriesContador.Core.Models.Purchases;

namespace Aries.Contabilidad.Services
{
    public interface IPurchasingClient
    {
        Task<List<Supplier>> GetAllAsync(string companyId);
        Task<Supplier?> FindAsync(int id);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
    }
}
