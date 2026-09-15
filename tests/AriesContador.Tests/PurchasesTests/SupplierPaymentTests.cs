using System;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Models.Utils;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.PurchasesTests
{
    public class SupplierPaymentTests
    {
        private const string Company = "C001";

        [Fact]
        public async Task PayPurchase_clears_outstanding_balance_and_posts_cxp_vs_cash()
        {
            var env = await SeedPostedOnAccount(unitPrice: 11300m);
            var before = await env.Accounting.GetPurchaseDetailAsync(env.PurchaseId);
            Assert.True(before.CanPay);
            Assert.Equal(11300m, before.OutstandingBalance);
            Assert.False(before.Paid);

            var payment = await env.Accounting.PayPurchaseAsync(new SupplierPayment
            {
                PurchaseId = env.PurchaseId,
                Amount = 11300m,
                PaymentMethod = PurchaseSettlement.Cash,
                PaidAt = DateTime.Now
            }, 7);

            Assert.True(payment.Id > 0);
            Assert.Equal(11300m, payment.Amount);
            Assert.Equal(2, env.Uow.JournalEntries.Items.Count);

            var payEntry = env.Uow.JournalEntries.Items.Single(e => e.Id == payment.JournalEntryId);
            Assert.True(payEntry.Cuadrado);
            Assert.Equal(JournalEntryStatus.Approved, payEntry.JournalEntryStatus);
            Assert.Contains(payEntry.JournalEntryLines, l => l.AccountId == 3 && l.DebOrCred == DebOrCred.Debito && l.Amount == 11300m);
            Assert.Contains(payEntry.JournalEntryLines, l => l.AccountId == 4 && l.DebOrCred == DebOrCred.Credito && l.Amount == 11300m);

            var after = await env.Accounting.GetPurchaseDetailAsync(env.PurchaseId);
            Assert.True(after.Paid);
            Assert.Equal(0m, after.OutstandingBalance);
            Assert.False(after.CanPay);
        }

        [Fact]
        public async Task PayPurchase_rejects_when_already_paid()
        {
            var env = await SeedPostedOnAccount(unitPrice: 113m);
            await env.Accounting.PayPurchaseAsync(new SupplierPayment
            {
                PurchaseId = env.PurchaseId,
                Amount = 113m,
                PaymentMethod = PurchaseSettlement.Cash
            }, 7);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PayPurchaseAsync(new SupplierPayment
                {
                    PurchaseId = env.PurchaseId,
                    Amount = 113m,
                    PaymentMethod = PurchaseSettlement.Cash
                }, 7));
            Assert.Equal("La factura ya está pagada", ex.Message);
        }

        [Fact]
        public async Task PayPurchase_rejects_when_not_posted()
        {
            var env = await SeedConfirmedOnAccountWithoutPosting(unitPrice: 113m);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PayPurchaseAsync(new SupplierPayment
                {
                    PurchaseId = env.PurchaseId,
                    Amount = 113m,
                    PaymentMethod = PurchaseSettlement.Cash
                }, 7));
            Assert.Equal("La compra debe estar asentada antes de registrar el pago", ex.Message);
        }

        [Fact]
        public async Task PayPurchase_rejects_cash_purchase()
        {
            var env = await SeedPostedCash(unitPrice: 113m);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PayPurchaseAsync(new SupplierPayment
                {
                    PurchaseId = env.PurchaseId,
                    Amount = 113m,
                    PaymentMethod = PurchaseSettlement.Cash
                }, 7));
            Assert.Equal("Solo se pagan facturas a crédito", ex.Message);
        }

        [Fact]
        public async Task PayPurchase_rejects_partial_amount()
        {
            var env = await SeedPostedOnAccount(unitPrice: 11300m);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PayPurchaseAsync(new SupplierPayment
                {
                    PurchaseId = env.PurchaseId,
                    Amount = 1000m,
                    PaymentMethod = PurchaseSettlement.Transfer,
                    PaymentReference = "TRX"
                }, 7));
            Assert.Equal("En v1 el pago debe ser por el total de la factura", ex.Message);
        }

        private static async Task<TestEnv> SeedPostedOnAccount(decimal unitPrice)
        {
            var env = await SeedConfirmed(unitPrice, PurchaseSettlement.OnAccount);
            await env.Accounting.PostPurchaseAsync(env.PurchaseId, 7);
            return env;
        }

        private static async Task<TestEnv> SeedPostedCash(decimal unitPrice)
        {
            var env = await SeedConfirmed(unitPrice, PurchaseSettlement.Cash);
            await env.Accounting.PostPurchaseAsync(env.PurchaseId, 7);
            return env;
        }

        private static Task<TestEnv> SeedConfirmedOnAccountWithoutPosting(decimal unitPrice) =>
            SeedConfirmed(unitPrice, PurchaseSettlement.OnAccount);

        private static async Task<TestEnv> SeedConfirmed(decimal unitPrice, PurchaseSettlement settlement)
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.Items.Add(new Account
            {
                Id = 100,
                CompanyId = Company,
                Name = "ACTIVO",
                AccountTag = AccountTag.Activo,
                AccountType = AccountType.Cuenta_Titulo,
                Active = true
            });
            AddAux(uow, 1, "INVENTARIOS", AccountTag.Activo);
            AddAux(uow, 2, DefaultChartOfAccounts.IvaSoportadoName, AccountTag.Activo);
            AddAux(uow, 3, DefaultChartOfAccounts.CuentasPorPagarName, AccountTag.Pasivo);
            AddAux(uow, 4, "CAJA", AccountTag.Activo);
            AddAux(uow, 5, "BANCOS", AccountTag.Activo);

            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                Id = 10,
                CompanyId = Company,
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Closed = false
            });

            uow.PurchaseAccountMaps.Items.Add(new PurchaseAccountMap
            {
                Id = 1,
                CompanyId = Company,
                InventoryAccountId = 1,
                TaxAccountId = 2,
                PayableAccountId = 3,
                CashAccountId = 4,
                CardAccountId = 5,
                TransferAccountId = 5,
                TaxRate = PosTax.DefaultRate,
                PricesIncludeTax = true,
                Active = true
            });

            uow.Products.Items.Add(new Product
            {
                Id = 1,
                CompanyId = Company,
                Barcode = "P-1",
                Name = "Mercadería",
                Category = "General",
                Price = 1500m,
                Cost = 0m,
                Stock = 0m,
                TaxExempt = false,
                Active = true
            });

            var purchasing = new PurchasingService(uow);
            var supplier = new Supplier
            {
                CompanyId = Company,
                Name = "Proveedor Crédito",
                NumberId = "3-101-888888",
                IdType = IdType.CEDULA_JURIDICA,
                Active = true
            };
            await purchasing.CreateSupplierAsync(supplier);

            var purchase = await purchasing.ConfirmPurchaseAsync(new Purchase
            {
                CompanyId = Company,
                SupplierId = supplier.Id,
                DocumentNumber = "F-PAY-1",
                PurchasedAt = DateTime.Now,
                PaymentMethod = settlement,
                Lines =
                {
                    new PurchaseLine { ProductId = 1, Quantity = 1, UnitPrice = unitPrice }
                }
            });

            var financial = new FinancialService(uow);
            var accounting = new PurchaseAccountingService(uow, financial);
            return new TestEnv
            {
                Uow = uow,
                Accounting = accounting,
                PurchaseId = purchase.Id
            };
        }

        private static void AddAux(FakeUnitOfWork uow, int id, string name, AccountTag tag)
        {
            uow.Accounts.Items.Add(new Account
            {
                Id = id,
                CompanyId = Company,
                Name = name,
                AccountTag = tag,
                AccountType = AccountType.Cuenta_Auxiliar,
                FatherAccount = 100,
                Active = true
            });
        }

        private sealed class TestEnv
        {
            public FakeUnitOfWork Uow { get; set; }
            public PurchaseAccountingService Accounting { get; set; }
            public int PurchaseId { get; set; }
        }
    }
}
