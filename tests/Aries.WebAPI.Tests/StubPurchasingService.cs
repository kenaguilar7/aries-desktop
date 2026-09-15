using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Tests
{
    public class StubPurchasingService : IPurchasingService
    {
        public List<Supplier> Items { get; } = new List<Supplier>();

        public Task<IEnumerable<Supplier>> GetSuppliersAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Supplier>>(Items.Where(s => s.CompanyId == companyId && s.Active).ToList());

        public Task<Supplier> FindSupplierAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(s => s.Id == id));

        public Task CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            supplier.Id = Items.Count + 1;
            supplier.Active = true;
            Items.Add(supplier);
            return Task.CompletedTask;
        }

        public Task UpdateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteSupplierAsync(int id, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
