using System;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.PosTests
{
    public class PointOfSaleServiceTests
    {
        private const string Company = "C001";

        [Fact]
        public async Task OpenSession_allows_two_registers_of_same_company_in_parallel()
        {
            var uow = SeedCompanyWithTwoRegisters();
            var svc = new PointOfSaleService(uow);

            var first = await svc.OpenSessionAsync(1, 100, "caja 1", 7);
            var second = await svc.OpenSessionAsync(2, 50, "caja 2", 7);

            Assert.True(first.IsOpen);
            Assert.True(second.IsOpen);
            Assert.Equal(2, uow.Registers.Sessions.Count(s => !s.ClosedAt.HasValue));
        }

        [Fact]
        public async Task OpenSession_rejects_second_open_on_same_register()
        {
            var uow = SeedCompanyWithTwoRegisters();
            var svc = new PointOfSaleService(uow);
            await svc.OpenSessionAsync(1, 100, null, 7);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.OpenSessionAsync(1, 20, null, 7));
            Assert.Equal("Ya hay una caja abierta", ex.Message);
        }

        [Fact]
        public async Task CreateSale_requires_open_session()
        {
            var uow = SeedCompanyWithTwoRegisters();
            AddProduct(uow, 10);
            var svc = new PointOfSaleService(uow);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateSaleAsync(CashSale(1, 1)));
            Assert.Equal("Debe abrir la caja antes de vender", ex.Message);
        }

        [Fact]
        public async Task CreateSale_rejects_insufficient_stock()
        {
            var uow = SeedCompanyWithTwoRegisters();
            AddProduct(uow, stock: 1);
            var svc = new PointOfSaleService(uow);
            await svc.OpenSessionAsync(1, 0, null, 7);

            var sale = CashSale(1, 1);
            sale.Lines[0].Quantity = 5;
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateSaleAsync(sale));
            Assert.StartsWith("Stock insuficiente", ex.Message);
        }

        [Fact]
        public async Task CloseSession_computes_expected_cash_and_difference()
        {
            var uow = SeedCompanyWithTwoRegisters();
            AddProduct(uow, stock: 10, price: 25);
            var svc = new PointOfSaleService(uow);
            await svc.OpenSessionAsync(1, 100, null, 7);
            await svc.CreateSaleAsync(CashSale(1, 1));

            var closed = await svc.CloseSessionAsync(1, 120, "faltante", 7);

            Assert.Equal(125m, closed.ExpectedClosingAmount);
            Assert.Equal(120m, closed.DeclaredClosingAmount);
            Assert.Equal(-5m, closed.Difference);
            Assert.False(closed.IsOpen);
        }

        [Fact]
        public async Task CreateSale_card_requires_reference_and_does_not_change_expected_cash()
        {
            var uow = SeedCompanyWithTwoRegisters();
            AddProduct(uow, stock: 10, price: 40);
            var svc = new PointOfSaleService(uow);
            await svc.OpenSessionAsync(1, 80, null, 7);

            var noRef = CashSale(1, 1);
            noRef.PaymentMethod = PaymentMethod.Tarjeta;
            var missing = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateSaleAsync(noRef));
            Assert.Equal("La referencia de pago es requerida", missing.Message);

            var card = CashSale(1, 1);
            card.PaymentMethod = PaymentMethod.Tarjeta;
            card.PaymentReference = "AUTH-9";
            await svc.CreateSaleAsync(card);

            var session = await svc.GetOpenSessionAsync(1);
            Assert.Equal(40m, session.CardSales);
            Assert.Equal(0m, session.CashSales);
            Assert.Equal(80m, session.ExpectedClosingAmount);
        }

        [Fact]
        public async Task DeleteRegister_rejected_when_session_is_open()
        {
            var uow = SeedCompanyWithTwoRegisters();
            var svc = new PointOfSaleService(uow);
            await svc.OpenSessionAsync(1, 10, null, 7);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeleteRegisterAsync(1));
            Assert.Equal("No se puede eliminar una caja con sesión abierta", ex.Message);
        }

        private static FakeUnitOfWork SeedCompanyWithTwoRegisters()
        {
            var uow = new FakeUnitOfWork();
            uow.Registers.Items.Add(new SalesRegister { Id = 1, CompanyId = Company, Code = "C1", Name = "Caja 1", Active = true });
            uow.Registers.Items.Add(new SalesRegister { Id = 2, CompanyId = Company, Code = "C2", Name = "Caja 2", Active = true });
            return uow;
        }

        private static void AddProduct(FakeUnitOfWork uow, decimal stock, decimal price = 25)
        {
            uow.Products.Items.Add(new Product
            {
                Id = 1,
                CompanyId = Company,
                Barcode = "1001",
                Name = "Pan",
                Category = "Panadería",
                Price = price,
                Stock = stock,
                Active = true
            });
        }

        private static Sale CashSale(int registerId, int productId)
        {
            return new Sale
            {
                CompanyId = Company,
                SalesRegisterId = registerId,
                PaymentMethod = PaymentMethod.Efectivo,
                Lines =
                {
                    new SaleLine { ProductId = productId, Quantity = 1 }
                }
            };
        }
    }
}
