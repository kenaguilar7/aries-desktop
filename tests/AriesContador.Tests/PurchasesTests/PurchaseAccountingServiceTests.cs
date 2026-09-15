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
    public class PurchaseAccountingServiceTests
    {
        private const string Company = "C001";

        [Fact]
        public async Task PreviewPurchase_builds_balanced_entry_inventory_tax_and_payable()
        {
            var env = await SeedReadyToPost(unitPrice: 11300m, settlement: PurchaseSettlement.OnAccount);
            var preview = await env.Accounting.PreviewPurchaseAsync(env.PurchaseId);

            Assert.False(preview.AlreadyPosted);
            Assert.Equal(10000m, preview.NetAmount);
            Assert.Equal(1300m, preview.TaxAmount);
            Assert.Equal(11300m, preview.Total);
            Assert.Contains(preview.Lines, l => l.AccountId == 1 && l.DebOrCred == DebOrCred.Debito && l.Amount == 10000m);
            Assert.Contains(preview.Lines, l => l.AccountId == 2 && l.DebOrCred == DebOrCred.Debito && l.Amount == 1300m);
            Assert.Contains(preview.Lines, l => l.AccountId == 3 && l.DebOrCred == DebOrCred.Credito && l.Amount == 11300m);
            Assert.Contains(preview.Lines, l => l.AccountName == "INVENTARIOS");
            Assert.Contains(preview.Lines, l => l.AccountName == DefaultChartOfAccounts.IvaSoportadoName);
            Assert.Contains(preview.Lines, l => l.AccountName == DefaultChartOfAccounts.CuentasPorPagarName);
        }

        [Fact]
        public async Task PostPurchase_creates_approved_journal_and_marks_posted()
        {
            var env = await SeedReadyToPost(unitPrice: 113m, settlement: PurchaseSettlement.Cash);
            var preview = await env.Accounting.PostPurchaseAsync(env.PurchaseId, 7);

            Assert.True(preview.AlreadyPosted);
            Assert.Equal(1, preview.JournalEntryId);
            var entry = env.Uow.JournalEntries.Items.Single();
            Assert.True(entry.Cuadrado);
            Assert.Equal(JournalEntryStatus.Approved, entry.JournalEntryStatus);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 4 && l.DebOrCred == DebOrCred.Credito && l.Amount == 113m);
            Assert.Empty(await env.Accounting.GetUnpostedPurchasesAsync(Company));
        }

        [Fact]
        public async Task PostPurchase_rejects_duplicate()
        {
            var env = await SeedReadyToPost(unitPrice: 113m, settlement: PurchaseSettlement.Cash);
            await env.Accounting.PostPurchaseAsync(env.PurchaseId, 7);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PostPurchaseAsync(env.PurchaseId, 7));
            Assert.Equal("La compra ya fue asentada", ex.Message);
        }

        [Fact]
        public async Task PreviewPurchase_rejects_incomplete_map()
        {
            var env = await SeedReadyToPost(unitPrice: 113m, settlement: PurchaseSettlement.Cash);
            env.Uow.PurchaseAccountMaps.Items.Clear();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PreviewPurchaseAsync(env.PurchaseId));
            Assert.Equal("Configure el mapeo de cuentas de compras antes de generar el asiento", ex.Message);
        }

        [Fact]
        public async Task PostPurchase_rejects_closed_period()
        {
            var env = await SeedReadyToPost(unitPrice: 113m, settlement: PurchaseSettlement.Cash);
            env.Uow.PostingPeriods.Items[0].Closed = true;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                env.Accounting.PostPurchaseAsync(env.PurchaseId, 7));
            Assert.Equal("El periodo contable está cerrado", ex.Message);
        }

        [Fact]
        public async Task PostPurchase_omits_tax_line_when_exempt()
        {
            var env = await SeedReadyToPost(unitPrice: 5000m, settlement: PurchaseSettlement.Transfer, taxExempt: true);
            var preview = await env.Accounting.PostPurchaseAsync(env.PurchaseId, 7);

            Assert.Equal(0m, preview.TaxAmount);
            Assert.Equal(5000m, preview.NetAmount);
            Assert.DoesNotContain(preview.Lines, l => l.AccountId == 2);
            Assert.Contains(preview.Lines, l => l.AccountId == 5 && l.DebOrCred == DebOrCred.Credito && l.Amount == 5000m);
            Assert.True(env.Uow.JournalEntries.Items[0].Cuadrado);
        }

        [Fact]
        public async Task PreviewPurchase_credits_card_account_for_card_settlement()
        {
            var env = await SeedReadyToPost(unitPrice: 113m, settlement: PurchaseSettlement.Card);
            var preview = await env.Accounting.PreviewPurchaseAsync(env.PurchaseId);

            Assert.Contains(preview.Lines, l => l.AccountId == 5 && l.DebOrCred == DebOrCred.Credito && l.Amount == 113m);
            Assert.DoesNotContain(preview.Lines, l => l.AccountId == 3);
        }

        private static async Task<TestEnv> SeedReadyToPost(
            decimal unitPrice,
            PurchaseSettlement settlement,
            bool taxExempt = false)
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
                TaxExempt = taxExempt,
                Active = true
            });

            var purchasing = new PurchasingService(uow);
            var supplier = new Supplier
            {
                CompanyId = Company,
                Name = "Proveedor Test",
                NumberId = "3-101-999999",
                IdType = IdType.CEDULA_JURIDICA,
                Active = true
            };
            await purchasing.CreateSupplierAsync(supplier);

            var purchase = await purchasing.ConfirmPurchaseAsync(new Purchase
            {
                CompanyId = Company,
                SupplierId = supplier.Id,
                DocumentNumber = "F-ACC-1",
                PurchasedAt = DateTime.Now,
                PaymentMethod = settlement,
                PaymentReference = settlement == PurchaseSettlement.Card || settlement == PurchaseSettlement.Transfer
                    ? "REF-1"
                    : null,
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
