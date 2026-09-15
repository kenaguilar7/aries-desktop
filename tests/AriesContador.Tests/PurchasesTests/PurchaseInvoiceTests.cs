using System;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.Purchases;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.PurchasesTests
{
    public class PurchaseInvoiceTests
    {
        private const string CompanyA = "C001";
        private const string CompanyB = "C002";

        [Fact]
        public async Task ConfirmPurchase_increments_stock_and_sets_unit_net_cost()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = await SeedSupplier(svc, CompanyA);
            var product = SeedProduct(uow, CompanyA, stock: 5m, cost: 100m, taxExempt: false);

            // Bruto unitario 113 → neto 100; 2 unidades → stock +2, Cost = 100
            var purchase = await svc.ConfirmPurchaseAsync(new Purchase
            {
                CompanyId = CompanyA,
                SupplierId = supplier.Id,
                DocumentNumber = "F-100",
                PaymentMethod = PurchaseSettlement.Cash,
                Lines =
                {
                    new PurchaseLine { ProductId = product.Id, Quantity = 2, UnitPrice = 113m }
                }
            });

            Assert.True(purchase.Id > 0);
            Assert.Equal(PurchaseStatus.Confirmed, purchase.Status);
            Assert.Equal(7m, product.Stock);
            Assert.Equal(100m, product.Cost);
            Assert.Equal(226m, purchase.Total);
            Assert.Equal(200m, purchase.NetAmount);
            Assert.Equal(26m, purchase.TaxAmount);
            Assert.Equal(200m, purchase.Lines[0].CostAmount);
        }

        [Fact]
        public async Task ConfirmPurchase_exempt_line_has_zero_tax()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = await SeedSupplier(svc, CompanyA);
            var product = SeedProduct(uow, CompanyA, stock: 0m, cost: 0m, taxExempt: true);

            var purchase = await svc.ConfirmPurchaseAsync(new Purchase
            {
                CompanyId = CompanyA,
                SupplierId = supplier.Id,
                DocumentNumber = "F-EX",
                PaymentMethod = PurchaseSettlement.OnAccount,
                Lines =
                {
                    new PurchaseLine { ProductId = product.Id, Quantity = 1, UnitPrice = 5000m }
                }
            });

            Assert.Equal(0m, purchase.TaxAmount);
            Assert.Equal(5000m, purchase.NetAmount);
            Assert.Equal(5000m, purchase.Total);
            Assert.Equal(0m, purchase.Lines[0].TaxAmount);
            Assert.Equal(5000m, purchase.Lines[0].NetAmount);
            Assert.Equal(5000m, product.Cost);
        }

        [Fact]
        public async Task ConfirmPurchase_taxed_11300_splits_to_10000_net_and_1300_tax()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = await SeedSupplier(svc, CompanyA);
            var product = SeedProduct(uow, CompanyA, stock: 0m, cost: 0m, taxExempt: false);

            var purchase = await svc.ConfirmPurchaseAsync(new Purchase
            {
                CompanyId = CompanyA,
                SupplierId = supplier.Id,
                DocumentNumber = "F-IVA",
                PaymentMethod = PurchaseSettlement.Transfer,
                PaymentReference = "TRX-1",
                Lines =
                {
                    new PurchaseLine { ProductId = product.Id, Quantity = 1, UnitPrice = 11300m }
                }
            });

            var expected = PosTax.SplitGross(11300m, PosTax.DefaultRate, exempt: false);
            Assert.Equal(expected.Net, purchase.NetAmount);
            Assert.Equal(expected.Tax, purchase.TaxAmount);
            Assert.Equal(10000m, purchase.NetAmount);
            Assert.Equal(1300m, purchase.TaxAmount);
            Assert.Equal(10000m, product.Cost);
        }

        [Fact]
        public async Task ConfirmPurchase_rejects_duplicate_document_for_same_supplier()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplier = await SeedSupplier(svc, CompanyA);
            var product = SeedProduct(uow, CompanyA, stock: 0m, cost: 0m, taxExempt: false);

            await svc.ConfirmPurchaseAsync(SamplePurchase(CompanyA, supplier.Id, "DUP-1", product.Id));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.ConfirmPurchaseAsync(SamplePurchase(CompanyA, supplier.Id, "DUP-1", product.Id)));
            Assert.Equal("Ya existe una factura con ese número para este proveedor", ex.Message);
        }

        [Fact]
        public async Task GetPurchases_filters_by_company()
        {
            var uow = new FakeUnitOfWork();
            var svc = new PurchasingService(uow);
            var supplierA = await SeedSupplier(svc, CompanyA);
            var supplierB = await SeedSupplier(svc, CompanyB, "Proveedor B");
            var productA = SeedProduct(uow, CompanyA, stock: 0m, cost: 0m, taxExempt: false);
            var productB = SeedProduct(uow, CompanyB, stock: 0m, cost: 0m, taxExempt: false, barcode: "B-1");

            await svc.ConfirmPurchaseAsync(SamplePurchase(CompanyA, supplierA.Id, "A-1", productA.Id));
            await svc.ConfirmPurchaseAsync(SamplePurchase(CompanyB, supplierB.Id, "B-1", productB.Id));

            var fromA = (await svc.GetPurchasesAsync(CompanyA)).ToList();
            var fromB = (await svc.GetPurchasesAsync(CompanyB)).ToList();

            Assert.Single(fromA);
            Assert.Equal("A-1", fromA[0].DocumentNumber);
            Assert.Single(fromB);
            Assert.Equal("B-1", fromB[0].DocumentNumber);
        }

        private static async Task<Supplier> SeedSupplier(PurchasingService svc, string companyId, string name = "Proveedor A")
        {
            var supplier = new Supplier
            {
                CompanyId = companyId,
                Name = name,
                NumberId = companyId + "-ID",
                IdType = Core.Models.Utils.IdType.CEDULA_JURIDICA,
                Active = true
            };
            await svc.CreateSupplierAsync(supplier);
            return supplier;
        }

        private static Product SeedProduct(
            FakeUnitOfWork uow,
            string companyId,
            decimal stock,
            decimal cost,
            bool taxExempt,
            string barcode = "P-1")
        {
            var product = new Product
            {
                CompanyId = companyId,
                Barcode = barcode,
                Name = "Producto " + barcode,
                Category = "General",
                Price = 1500m,
                Cost = cost,
                Stock = stock,
                TaxExempt = taxExempt,
                Active = true
            };
            uow.Products.Items.Add(product);
            product.Id = uow.Products.Items.Count;
            return product;
        }

        private static Purchase SamplePurchase(string companyId, int supplierId, string doc, int productId) =>
            new Purchase
            {
                CompanyId = companyId,
                SupplierId = supplierId,
                DocumentNumber = doc,
                PaymentMethod = PurchaseSettlement.Cash,
                Lines =
                {
                    new PurchaseLine { ProductId = productId, Quantity = 1, UnitPrice = 113m }
                }
            };
    }
}
