using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Models.Utils;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.PurchasesTests
{
    public class PurchasingServiceTests
    {
        private const string CompanyA = "C001";
        private const string CompanyB = "C002";

        [Fact]
        public async Task CreateSupplier_persists_in_company()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);

            var supplier = Sample(CompanyA, "Distribuidora Sol", "3-101-123456");
            await svc.CreateSupplierAsync(supplier);

            Assert.True(supplier.Id > 0);
            var listed = (await svc.GetSuppliersAsync(CompanyA)).ToList();
            Assert.Single(listed);
            Assert.Equal("Distribuidora Sol", listed[0].Name);
            Assert.Equal("3-101-123456", listed[0].NumberId);
            Assert.Equal(IdType.CEDULA_JURIDICA, listed[0].IdType);
        }

        [Fact]
        public async Task CreateSupplier_rejects_duplicate_number_id_in_same_company()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            await svc.CreateSupplierAsync(Sample(CompanyA, "Uno", "3-101-123456"));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CreateSupplierAsync(Sample(CompanyA, "Dos", "3-101-123456")));
            Assert.Equal("Ya existe un proveedor con esa identificación", ex.Message);
        }

        [Fact]
        public async Task DeleteSupplier_soft_deletes_and_hides_from_GetAll()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = Sample(CompanyA, "Borrar", "1-2345-6789");
            await svc.CreateSupplierAsync(supplier);

            await svc.DeleteSupplierAsync(supplier.Id);

            Assert.False(uow.Suppliers.Items.Single().Active);
            Assert.Empty(await svc.GetSuppliersAsync(CompanyA));
            var found = await svc.FindSupplierAsync(supplier.Id);
            Assert.NotNull(found);
            Assert.False(found.Active);
        }

        [Fact]
        public async Task GetSuppliers_does_not_return_other_company()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            await svc.CreateSupplierAsync(Sample(CompanyA, "De A", "3-101-111111"));
            await svc.CreateSupplierAsync(Sample(CompanyB, "De B", "3-101-111111"));

            var fromA = (await svc.GetSuppliersAsync(CompanyA)).ToList();
            var fromB = (await svc.GetSuppliersAsync(CompanyB)).ToList();

            Assert.Single(fromA);
            Assert.Equal("De A", fromA[0].Name);
            Assert.Single(fromB);
            Assert.Equal("De B", fromB[0].Name);
        }

        [Fact]
        public async Task CreateSupplier_allows_empty_number_id_more_than_once()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            await svc.CreateSupplierAsync(Sample(CompanyA, "Sin cédula 1", ""));
            await svc.CreateSupplierAsync(Sample(CompanyA, "Sin cédula 2", "  "));

            Assert.Equal(2, (await svc.GetSuppliersAsync(CompanyA)).Count());
        }

        [Fact]
        public async Task CreateSupplier_requires_name()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = Sample(CompanyA, "   ", "3-101-123456");

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateSupplierAsync(supplier));
            Assert.Equal("El nombre del proveedor es requerido", ex.Message);
        }

        private static Supplier Sample(string companyId, string name, string numberId) => new Supplier
        {
            CompanyId = companyId,
            Name = name,
            NumberId = numberId,
            IdType = IdType.CEDULA_JURIDICA,
            Active = true
        };
    }
}
