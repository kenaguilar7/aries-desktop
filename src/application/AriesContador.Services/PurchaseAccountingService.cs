using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class PurchaseAccountingService : IPurchaseAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFinancialService _financial;

        public PurchaseAccountingService(IUnitOfWork unitOfWork, IFinancialService financial)
        {
            _unitOfWork = unitOfWork;
            _financial = financial;
        }

        public async Task<PurchaseAccountMap> GetAccountMapAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var map = await _unitOfWork.PurchaseAccountMapRepository.GetByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false);
            if (map == null)
            {
                map = new PurchaseAccountMap
                {
                    CompanyId = companyId,
                    TaxRate = Core.Models.PointOfSale.PosTax.DefaultRate,
                    PricesIncludeTax = true,
                    Active = true
                };
            }

            await SuggestMappedAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            return map;
        }

        public async Task SaveAccountMapAsync(PurchaseAccountMap map, CancellationToken cancellationToken = default)
        {
            if (map == null)
                throw new InvalidOperationException("El mapeo es requerido");
            RequireCompany(map.CompanyId);
            if (map.TaxRate < 0 || map.TaxRate > 1)
                throw new InvalidOperationException("La tasa de IVA debe estar entre 0 y 1");
            await ValidateMapAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            if (!map.IsComplete)
                throw new InvalidOperationException("Debe asignar todas las cuentas del mapeo de compras");
            map.Active = true;
            await _unitOfWork.PurchaseAccountMapRepository.UpsertAsync(map, cancellationToken).ConfigureAwait(false);
        }

        public async Task<PurchaseAccountMap> EnsureSuggestedAccountsAsync(
            string companyId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            await _financial.EnsurePurchaseAccountsAsync(companyId, userId, cancellationToken).ConfigureAwait(false);
            return await GetAccountMapAsync(companyId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Purchase>> GetUnpostedPurchasesAsync(
            string companyId,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var purchases = (await _unitOfWork.PurchaseRepository.FindByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false))
                .Where(p => p.Status == PurchaseStatus.Confirmed && p.Active)
                .ToList();
            var posted = new HashSet<int>((await _unitOfWork.PurchasePostingRepository.FindByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false)).Select(x => x.PurchaseId));
            return purchases.Where(p => !posted.Contains(p.Id)).ToList();
        }

        public Task<PurchasePostingPreview> PreviewPurchaseAsync(int purchaseId, CancellationToken cancellationToken = default)
        {
            return BuildPreviewAsync(purchaseId, persist: false, userId: 0, cancellationToken);
        }

        public Task<PurchasePostingPreview> PostPurchaseAsync(int purchaseId, int userId, CancellationToken cancellationToken = default)
        {
            return BuildPreviewAsync(purchaseId, persist: true, userId: userId, cancellationToken);
        }

        private async Task<PurchasePostingPreview> BuildPreviewAsync(
            int purchaseId,
            bool persist,
            int userId,
            CancellationToken cancellationToken)
        {
            var purchase = await _unitOfWork.PurchaseRepository.GetByIdAsync(purchaseId, cancellationToken)
                .ConfigureAwait(false);
            if (purchase == null || !purchase.Active)
                throw new InvalidOperationException("Compra no encontrada");
            if (purchase.Status != PurchaseStatus.Confirmed)
                throw new InvalidOperationException("Solo se asientan compras confirmadas");

            var existing = await _unitOfWork.PurchasePostingRepository.GetByPurchaseIdAsync(purchaseId, cancellationToken)
                .ConfigureAwait(false);
            var hash = PurchasePostingHash.Compute(
                purchase.NetAmount, purchase.TaxAmount, purchase.Total, purchase.PaymentMethod);
            var map = await _unitOfWork.PurchaseAccountMapRepository.GetByCompanyIdAsync(purchase.CompanyId, cancellationToken)
                .ConfigureAwait(false);
            var period = await ResolvePeriodAsync(purchase, cancellationToken).ConfigureAwait(false);

            var preview = new PurchasePostingPreview
            {
                Purchase = purchase,
                NetAmount = purchase.NetAmount,
                TaxAmount = purchase.TaxAmount,
                Total = purchase.Total,
                PaymentMethod = purchase.PaymentMethod,
                TotalsHash = hash,
                AlreadyPosted = existing != null,
                JournalEntryId = existing?.JournalEntryId,
                PostingPeriodId = period?.Id,
                PeriodName = period?.ToString()
            };

            if (existing != null)
            {
                preview.Warning = "La compra ya fue asentada";
                if (persist)
                    throw new InvalidOperationException(preview.Warning);
                return preview;
            }

            if (map == null || !map.IsComplete)
                throw new InvalidOperationException("Configure el mapeo de cuentas de compras antes de generar el asiento");
            var accounts = await ValidateMapAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            if (period == null)
                throw new InvalidOperationException("No hay un periodo contable para la fecha de la compra");
            if (period.Closed)
                throw new InvalidOperationException("El periodo contable está cerrado");

            preview.Lines = BuildJournalLines(purchase, map, accounts);
            if (preview.Lines.Count == 0)
                throw new InvalidOperationException("No hay movimientos para asentar");

            var draft = new JournalEntry
            {
                PostingPeriodId = period.Id,
                JournalEntryLines = preview.Lines
            };
            draft.ApplyStatusFromBalance();
            if (!draft.Cuadrado)
                throw new InvalidOperationException("El asiento generado no está cuadrado");

            if (!persist)
                return preview;

            var entry = new JournalEntry
            {
                PostingPeriodId = period.Id,
                CreatedBy = userId,
                UpdatedBy = userId,
                Active = true,
                JournalEntryLines = preview.Lines
            };
            await _financial.CreateApprovedJournalEntryAsync(entry, purchase.CompanyId, cancellationToken)
                .ConfigureAwait(false);

            var posting = new PurchasePosting
            {
                PurchaseId = purchase.Id,
                CompanyId = purchase.CompanyId,
                JournalEntryId = entry.Id,
                PostedAt = DateTime.Now,
                TotalsHash = hash,
                CreatedBy = userId,
                UpdatedBy = userId,
                Active = true
            };
            await _unitOfWork.PurchasePostingRepository.AddAsync(posting, cancellationToken).ConfigureAwait(false);

            preview.AlreadyPosted = true;
            preview.JournalEntryId = entry.Id;
            return preview;
        }

        private static List<JournalEntryLine> BuildJournalLines(
            Purchase purchase,
            PurchaseAccountMap map,
            Dictionary<int, Account> accounts)
        {
            var date = purchase.PurchasedAt == default ? DateTime.Now : purchase.PurchasedAt;
            var reference = "COMP-" + (purchase.DocumentNumber ?? purchase.Id.ToString());
            var memo = "Compra " + (purchase.SupplierName ?? ("#" + purchase.SupplierId))
                + " factura " + purchase.DocumentNumber
                + " " + date.ToString("g");
            var lines = new List<JournalEntryLine>();

            AddLine(lines, accounts, map.InventoryAccountId, purchase.NetAmount, DebOrCred.Debito, date, reference, memo);
            AddLine(lines, accounts, map.TaxAccountId, purchase.TaxAmount, DebOrCred.Debito, date, reference, memo);

            var creditAccountId = CreditAccountId(purchase.PaymentMethod, map);
            AddLine(lines, accounts, creditAccountId, purchase.Total, DebOrCred.Credito, date, reference, memo);

            return lines;
        }

        private static int CreditAccountId(PurchaseSettlement method, PurchaseAccountMap map)
        {
            switch (method)
            {
                case PurchaseSettlement.Card:
                    return map.CardAccountId;
                case PurchaseSettlement.Transfer:
                    return map.TransferAccountId;
                case PurchaseSettlement.OnAccount:
                    return map.PayableAccountId;
                default:
                    return map.CashAccountId;
            }
        }

        private static void AddLine(
            List<JournalEntryLine> lines,
            Dictionary<int, Account> accounts,
            int accountId,
            decimal amount,
            DebOrCred side,
            DateTime date,
            string reference,
            string memo)
        {
            if (amount <= 0)
                return;
            Account account;
            accounts.TryGetValue(accountId, out account);
            lines.Add(new JournalEntryLine
            {
                AccountId = accountId,
                AccountName = account?.Name,
                AccountPath = account?.PathDirection,
                Amount = amount,
                DebOrCred = side,
                Date = date,
                Reference = reference,
                Memo = memo,
                Currency = Currency.colones,
                RateAmount = 1m,
                Active = true
            });
        }

        private async Task<PostingPeriod> ResolvePeriodAsync(Purchase purchase, CancellationToken cancellationToken)
        {
            var date = purchase.PurchasedAt == default ? DateTime.Now : purchase.PurchasedAt;
            var periods = await _financial.GetPostingPeriodsAsync(purchase.CompanyId, cancellationToken).ConfigureAwait(false);
            return periods.FirstOrDefault(p => p.Date.Year == date.Year && p.Date.Month == date.Month);
        }

        private async Task<Dictionary<int, Account>> ValidateMapAccountsAsync(
            PurchaseAccountMap map,
            CancellationToken cancellationToken)
        {
            var accounts = (await _financial.GetAccountsAsync(map.CompanyId, cancellationToken).ConfigureAwait(false))
                .ToDictionary(a => a.Id);
            RequireAuxiliar(accounts, map.InventoryAccountId, "inventario");
            RequireAuxiliar(accounts, map.TaxAccountId, "IVA soportado");
            RequireAuxiliar(accounts, map.PayableAccountId, "cuentas por pagar");
            RequireAuxiliar(accounts, map.CashAccountId, "efectivo");
            RequireAuxiliar(accounts, map.CardAccountId, "tarjeta");
            RequireAuxiliar(accounts, map.TransferAccountId, "transferencia");
            return accounts;
        }

        private static void RequireAuxiliar(Dictionary<int, Account> accounts, int accountId, string slot)
        {
            if (accountId <= 0)
                throw new InvalidOperationException("Falta la cuenta de " + slot);
            Account account;
            if (!accounts.TryGetValue(accountId, out account))
                throw new InvalidOperationException("La cuenta de " + slot + " no pertenece a la compañía");
            if (account.AccountType != AccountType.Cuenta_Auxiliar)
                throw new InvalidOperationException("La cuenta de " + slot + " debe ser auxiliar");
        }

        private async Task SuggestMappedAccountsAsync(PurchaseAccountMap map, CancellationToken cancellationToken)
        {
            var accounts = (await _financial.GetAccountsAsync(map.CompanyId, cancellationToken).ConfigureAwait(false))
                .Where(a => a.AccountType == AccountType.Cuenta_Auxiliar)
                .ToList();
            if (map.InventoryAccountId == 0)
                map.InventoryAccountId = FindAccountId(accounts, "INVENTARIOS");
            if (map.TaxAccountId == 0)
                map.TaxAccountId = FindAccountId(accounts, DefaultChartOfAccounts.IvaSoportadoName);
            if (map.PayableAccountId == 0)
                map.PayableAccountId = FindAccountId(accounts, DefaultChartOfAccounts.CuentasPorPagarName);
            if (map.CashAccountId == 0)
                map.CashAccountId = FindAccountId(accounts, "CAJA");
            if (map.CardAccountId == 0)
                map.CardAccountId = FindAccountId(accounts, "BANCOS");
            if (map.TransferAccountId == 0)
            {
                map.TransferAccountId = FindAccountId(accounts, "BANCOS");
                if (map.TransferAccountId == 0)
                    map.TransferAccountId = map.CardAccountId;
            }
        }

        private static int FindAccountId(IEnumerable<Account> accounts, string name)
        {
            var found = accounts.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
            return found?.Id ?? 0;
        }

        private static void RequireCompany(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
                throw new InvalidOperationException("La compañía es requerida");
        }
    }
}
