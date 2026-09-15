using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;

namespace AriesContador.Core.Repositories
{
    public interface ISupplierPaymentRepository
    {
        Task<SupplierPayment> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SupplierPayment> GetByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SupplierPayment>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default);
        Task AddAsync(SupplierPayment payment, CancellationToken cancellationToken = default);
    }
}
