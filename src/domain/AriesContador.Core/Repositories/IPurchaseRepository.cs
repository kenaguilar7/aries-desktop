using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Repositories
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<Purchase> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Purchase>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task<Purchase> FindByDocumentAsync(string companyId, int supplierId, string documentNumber, CancellationToken cancellationToken = default);
        Task CreateWithEffectsAsync(Purchase purchase, CancellationToken cancellationToken = default);
        /// <summary>Soft delete + revierte stock. No modifica product.Cost.</summary>
        Task CancelWithEffectsAsync(Purchase purchase, CancellationToken cancellationToken = default);
    }
}
