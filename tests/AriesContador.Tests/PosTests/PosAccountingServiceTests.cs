using System;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.PosTests
{
    public class PosAccountingServiceTests
    {
        private const string Company = "C001";

        [Fact]
        public async Task PostSession_creates_balanced_entry_with_iva_and_cogs()
        {
            var env = SeedReadyToPost(price: 113m, cost: 40m);
            var preview = await env.Accounting.PostSessionAsync(env.SessionId, 7);

            Assert.True(preview.AlreadyPosted);
            Assert.Equal(1, preview.JournalEntryId);
            var entry = env.Uow.JournalEntries.Items.Single();
            Assert.True(entry.Cuadrado);
            Assert.Equal(JournalEntryStatus.Approved, entry.JournalEntryStatus);
            Assert.Equal(100m, preview.NetSales);
            Assert.Equal(13m, preview.TaxAmount);
            Assert.Equal(40m, preview.CostAmount);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 4 && l.DebOrCred == DebOrCred.Credito && l.Amount == 100m);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 5 && l.DebOrCred == DebOrCred.Credito && l.Amount == 13m);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 1 && l.DebOrCred == DebOrCred.Debito && l.Amount == 113m);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 7 && l.DebOrCred == DebOrCred.Debito && l.Amount == 40m);
            Assert.Contains(entry.JournalEntryLines, l => l.AccountId == 6 && l.DebOrCred == DebOrCred.Credito && l.Amount == 40m);
            Assert.Contains(preview.Lines, l => l.AccountId == 1 && l.AccountName == "CAJA");
            Assert.Contains(preview.Lines, l => l.AccountId == 4 && l.AccountName == "VENTAS");
            Assert.Contains(preview.Lines, l => l.AccountId == 5 && l.AccountName == "IVA POR PAGAR");
            Assert.Contains(preview.Lines, l => l.AccountId == 7 && l.AccountName == "COSTO DE MERCADERÍA");
            Assert.Contains(preview.Lines, l => l.AccountId == 6 && l.AccountName == "INVENTARIOS");
        }

        [Fact]
        public async Task PostSession_rejects_duplicate()
        {
            var env = SeedReadyToPost(price: 113m, cost: 40m);
            await env.Accounting.PostSessionAsync(env.SessionId, 7);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => env.Accounting.PostSessionAsync(env.SessionId, 7));
            Assert.Equal("La sesión ya fue asentada", ex.Message);
        }

        [Fact]
        public async Task PreviewSession_rejects_incomplete_map()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m);
            env.Uow.AccountMaps.Items.Clear();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => env.Accounting.PreviewSessionAsync(env.SessionId));
            Assert.Equal("Configure el mapeo de cuentas antes de generar el asiento", ex.Message);
        }

        [Fact]
        public async Task PostSession_rejects_closed_period()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m);
            env.Uow.PostingPeriods.Items[0].Closed = true;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => env.Accounting.PostSessionAsync(env.SessionId, 7));
            Assert.Equal("El periodo contable está cerrado", ex.Message);
        }

        [Fact]
        public async Task PostSession_omits_cogs_when_cost_is_zero()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m);
            var preview = await env.Accounting.PostSessionAsync(env.SessionId, 7);

            Assert.Equal(0m, preview.CostAmount);
            Assert.DoesNotContain(preview.Lines, l => l.AccountId == 6 || l.AccountId == 7);
            Assert.True(env.Uow.JournalEntries.Items[0].Cuadrado);
        }

        [Fact]
        public async Task PostSession_exempt_product_has_no_tax_line()
        {
            var env = SeedReadyToPost(price: 50m, cost: 10m, taxExempt: true);
            var preview = await env.Accounting.PostSessionAsync(env.SessionId, 7);

            Assert.Equal(50m, preview.NetSales);
            Assert.Equal(0m, preview.TaxAmount);
            Assert.DoesNotContain(preview.Lines, l => l.AccountId == 5);
            Assert.True(env.Uow.JournalEntries.Items[0].Cuadrado);
        }

        [Fact]
        public async Task PostSession_posts_cash_shortage()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m, declared: 100m);
            var preview = await env.Accounting.PostSessionAsync(env.SessionId, 7);

            Assert.Equal(-13m, preview.Difference);
            Assert.Contains(preview.Lines, l => l.AccountId == 8 && l.DebOrCred == DebOrCred.Debito && l.Amount == 13m);
            Assert.Contains(preview.Lines, l => l.AccountId == 1 && l.DebOrCred == DebOrCred.Credito && l.Amount == 13m);
            Assert.True(env.Uow.JournalEntries.Items[0].Cuadrado);
        }

        [Fact]
        public async Task Reconciliation_marks_pending_posted_and_difference()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m);
            var pending = (await env.Accounting.GetReconciliationAsync(Company)).Single();
            Assert.Equal("Pendiente", pending.Status);

            await env.Accounting.PostSessionAsync(env.SessionId, 7);
            var posted = (await env.Accounting.GetReconciliationAsync(Company)).Single();
            Assert.Equal("Posteado", posted.Status);
            Assert.True(posted.TotalsMatch);

            env.Uow.SessionPostings.Items[0].TotalsHash = "broken";
            var diff = (await env.Accounting.GetReconciliationAsync(Company)).Single();
            Assert.Equal("Diferencia", diff.Status);
        }

        [Fact]
        public async Task SaveAccountMap_requires_auxiliar_accounts()
        {
            var env = SeedReadyToPost(price: 113m, cost: 0m);
            env.Uow.Accounts.Items.First(a => a.Id == 1).AccountType = AccountType.Cuenta_De_Mayor;
            var map = env.Uow.AccountMaps.Items[0];

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => env.Accounting.SaveAccountMapAsync(map));
            Assert.Contains("auxiliar", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        private static TestEnv SeedReadyToPost(decimal price, decimal cost, bool taxExempt = false, decimal declared = 0m)
        {
            var uow = new FakeUnitOfWork();
            uow.Registers.Items.Add(new SalesRegister { Id = 1, CompanyId = Company, Code = "C1", Name = "Caja 1", Active = true });
            uow.Accounts.Items.Add(new Account
            {
                Id = 100,
                CompanyId = Company,
                Name = "ACTIVO",
                AccountTag = AccountTag.Activo,
                AccountType = AccountType.Cuenta_Titulo,
                Active = true
            });
            AddAux(uow, 1, "CAJA", AccountTag.Activo);
            AddAux(uow, 2, "BANCOS", AccountTag.Activo);
            AddAux(uow, 3, "CUENTAS POR COBRAR", AccountTag.Activo);
            AddAux(uow, 4, "VENTAS", AccountTag.Ingreso);
            AddAux(uow, 5, "IVA POR PAGAR", AccountTag.Pasivo);
            AddAux(uow, 6, "INVENTARIOS", AccountTag.Activo);
            AddAux(uow, 7, "COSTO DE MERCADERÍA", AccountTag.CostoVenta);
            AddAux(uow, 8, "FALTANTE DE CAJA", AccountTag.Egreso);
            AddAux(uow, 9, "SOBRANTE DE CAJA", AccountTag.Ingreso);
            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                Id = 10,
                CompanyId = Company,
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Closed = false
            });
            uow.AccountMaps.Items.Add(new PosAccountMap
            {
                Id = 1,
                CompanyId = Company,
                CashAccountId = 1,
                CardAccountId = 2,
                TransferAccountId = 3,
                SalesAccountId = 4,
                TaxAccountId = 5,
                InventoryAccountId = 6,
                CogsAccountId = 7,
                CashShortAccountId = 8,
                CashOverAccountId = 9,
                TaxRate = 0.13m,
                PricesIncludeTax = true,
                Active = true
            });
            uow.Products.Items.Add(new Product
            {
                Id = 1,
                CompanyId = Company,
                Barcode = "1001",
                Name = "Pan",
                Category = "Panadería",
                Price = price,
                Cost = cost,
                Stock = 10,
                TaxExempt = taxExempt,
                Active = true
            });

            var pos = new PointOfSaleService(uow);
            var financial = new FinancialService(uow);
            var accounting = new PosAccountingService(uow, financial);

            pos.OpenSessionAsync(1, 0, null, 7).GetAwaiter().GetResult();
            pos.CreateSaleAsync(new Sale
            {
                CompanyId = Company,
                SalesRegisterId = 1,
                PaymentMethod = PaymentMethod.Efectivo,
                Lines = { new SaleLine { ProductId = 1, Quantity = 1 } }
            }).GetAwaiter().GetResult();
            var closed = pos.CloseSessionAsync(1, declared == 0m ? price : declared, null, 7).GetAwaiter().GetResult();

            return new TestEnv
            {
                Uow = uow,
                Accounting = accounting,
                SessionId = closed.Id
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
            public PosAccountingService Accounting { get; set; }
            public int SessionId { get; set; }
        }
    }
}
