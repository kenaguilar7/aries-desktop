using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class PosAccountingService : IPosAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFinancialService _financial;

        public PosAccountingService(IUnitOfWork unitOfWork, IFinancialService financial)
        {
            _unitOfWork = unitOfWork;
            _financial = financial;
        }

        public async Task<PosAccountMap> GetAccountMapAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var map = await _unitOfWork.PosAccountMapRepository.GetByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false);
            if (map == null)
            {
                map = new PosAccountMap
                {
                    CompanyId = companyId,
                    TaxRate = PosTax.DefaultRate,
                    PricesIncludeTax = true,
                    Active = true
                };
            }

            await SuggestMappedAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            return map;
        }

        public async Task SaveAccountMapAsync(PosAccountMap map, CancellationToken cancellationToken = default)
        {
            if (map == null)
                throw new InvalidOperationException("El mapeo es requerido");
            RequireCompany(map.CompanyId);
            if (map.TaxRate < 0 || map.TaxRate > 1)
                throw new InvalidOperationException("La tasa de IVA debe estar entre 0 y 1");
            await ValidateMapAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            if (!map.IsComplete)
                throw new InvalidOperationException("Debe asignar todas las cuentas del mapeo");
            map.Active = true;
            await _unitOfWork.PosAccountMapRepository.UpsertAsync(map, cancellationToken).ConfigureAwait(false);
        }

        public async Task<PosAccountMap> EnsureSuggestedAccountsAsync(
            string companyId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var accounts = (await _financial.GetAccountsAsync(companyId, cancellationToken).ConfigureAwait(false)).ToList();
            await EnsureAccountAsync(accounts, companyId, userId, PosAccountMap.VentasName, "INGRESO", AccountTag.Ingreso, cancellationToken)
                .ConfigureAwait(false);
            await EnsureAccountAsync(accounts, companyId, userId, PosAccountMap.IvaPorPagarName, "PASIVO CORTO PLAZO", AccountTag.Pasivo, cancellationToken)
                .ConfigureAwait(false);
            await EnsureAccountAsync(accounts, companyId, userId, PosAccountMap.CostoMercaderiaName, "COSTO VENTA", AccountTag.CostoVenta, cancellationToken)
                .ConfigureAwait(false);
            await EnsureAccountAsync(accounts, companyId, userId, PosAccountMap.FaltanteCajaName, "OTROS GASTOS", AccountTag.Egreso, cancellationToken)
                .ConfigureAwait(false);
            await EnsureAccountAsync(accounts, companyId, userId, PosAccountMap.SobranteCajaName, "INGRESO", AccountTag.Ingreso, cancellationToken)
                .ConfigureAwait(false);

            var map = await GetAccountMapAsync(companyId, cancellationToken).ConfigureAwait(false);
            return map;
        }

        public async Task<IEnumerable<SalesRegisterSession>> GetUnpostedSessionsAsync(
            string companyId,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var history = await _unitOfWork.SalesRegisterRepository.GetSessionHistoryAsync(companyId, cancellationToken)
                .ConfigureAwait(false);
            var postings = new HashSet<int>((await _unitOfWork.PosSessionPostingRepository.FindByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false)).Select(p => p.SessionId));
            return history.Where(s => s.ClosedAt.HasValue && !postings.Contains(s.Id)).ToList();
        }

        public async Task<PosPostingPreview> PreviewSessionAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await BuildPreviewAsync(sessionId, persist: false, userId: 0, cancellationToken).ConfigureAwait(false);
        }

        public Task<PosPostingPreview> PostSessionAsync(int sessionId, int userId, CancellationToken cancellationToken = default)
        {
            return BuildPreviewAsync(sessionId, persist: true, userId: userId, cancellationToken);
        }

        public async Task<IEnumerable<PosSessionReconciliationRow>> GetReconciliationAsync(
            string companyId,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var sessions = (await _unitOfWork.SalesRegisterRepository.GetSessionHistoryAsync(companyId, cancellationToken)
                .ConfigureAwait(false)).Where(s => s.ClosedAt.HasValue).ToList();
            var postings = (await _unitOfWork.PosSessionPostingRepository.FindByCompanyIdAsync(companyId, cancellationToken)
                .ConfigureAwait(false)).ToDictionary(p => p.SessionId);
            var rows = new List<PosSessionReconciliationRow>();
            foreach (var session in sessions)
            {
                var totals = await SessionTotalsAsync(session, cancellationToken).ConfigureAwait(false);
                PosSessionPosting posting;
                postings.TryGetValue(session.Id, out posting);
                var match = posting != null && string.Equals(posting.TotalsHash, totals.Hash, StringComparison.Ordinal);
                string status;
                if (posting == null)
                    status = "Pendiente";
                else if (match)
                    status = "Posteado";
                else
                    status = "Diferencia";

                rows.Add(new PosSessionReconciliationRow
                {
                    SessionId = session.Id,
                    RegisterCode = session.RegisterCode,
                    RegisterName = session.RegisterName,
                    OpenedAt = session.OpenedAt,
                    ClosedAt = session.ClosedAt,
                    TotalSales = session.TotalSales,
                    CashSales = totals.Cash,
                    CardSales = totals.Card,
                    TransferSales = totals.Transfer,
                    NetSales = totals.Net,
                    TaxAmount = totals.Tax,
                    CostAmount = totals.Cost,
                    Difference = totals.Difference,
                    Posted = posting != null,
                    JournalEntryId = posting?.JournalEntryId,
                    TotalsMatch = match,
                    Status = status
                });
            }

            return rows;
        }

        private async Task<PosPostingPreview> BuildPreviewAsync(
            int sessionId,
            bool persist,
            int userId,
            CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.SalesRegisterRepository.GetSessionByIdAsync(sessionId, cancellationToken)
                .ConfigureAwait(false);
            if (session == null)
                throw new InvalidOperationException("Sesión no encontrada");
            if (!session.ClosedAt.HasValue)
                throw new InvalidOperationException("Solo se asientan sesiones cerradas");

            var existing = await _unitOfWork.PosSessionPostingRepository.GetBySessionIdAsync(sessionId, cancellationToken)
                .ConfigureAwait(false);
            var totals = await SessionTotalsAsync(session, cancellationToken).ConfigureAwait(false);
            var map = await _unitOfWork.PosAccountMapRepository.GetByCompanyIdAsync(session.CompanyId, cancellationToken)
                .ConfigureAwait(false);
            var period = await ResolvePeriodAsync(session, cancellationToken).ConfigureAwait(false);

            var preview = new PosPostingPreview
            {
                Session = session,
                CashSales = totals.Cash,
                CardSales = totals.Card,
                TransferSales = totals.Transfer,
                NetSales = totals.Net,
                TaxAmount = totals.Tax,
                CostAmount = totals.Cost,
                Difference = totals.Difference,
                TotalsHash = totals.Hash,
                AlreadyPosted = existing != null,
                JournalEntryId = existing?.JournalEntryId,
                PostingPeriodId = period?.Id,
                PeriodName = period?.ToString()
            };

            if (existing != null)
            {
                preview.Warning = "La sesión ya fue asentada";
                if (persist)
                    throw new InvalidOperationException(preview.Warning);
                return preview;
            }

            if (map == null || !map.IsComplete)
                throw new InvalidOperationException("Configure el mapeo de cuentas antes de generar el asiento");
            var accounts = await ValidateMapAccountsAsync(map, cancellationToken).ConfigureAwait(false);
            if (period == null)
                throw new InvalidOperationException("No hay un periodo contable para el mes de cierre de la caja");
            if (period.Closed)
                throw new InvalidOperationException("El periodo contable está cerrado");

            preview.Lines = BuildJournalLines(session, totals, map, accounts);
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
            await _financial.CreateApprovedJournalEntryAsync(entry, session.CompanyId, cancellationToken)
                .ConfigureAwait(false);

            var posting = new PosSessionPosting
            {
                SessionId = session.Id,
                CompanyId = session.CompanyId,
                JournalEntryId = entry.Id,
                PostedAt = DateTime.Now,
                TotalsHash = totals.Hash,
                CreatedBy = userId,
                UpdatedBy = userId,
                Active = true
            };
            await _unitOfWork.PosSessionPostingRepository.AddAsync(posting, cancellationToken).ConfigureAwait(false);

            preview.AlreadyPosted = true;
            preview.JournalEntryId = entry.Id;
            return preview;
        }

        private async Task<(decimal Cash, decimal Card, decimal Transfer, decimal Net, decimal Tax, decimal Cost, decimal Difference, string Hash)>
            SessionTotalsAsync(SalesRegisterSession session, CancellationToken cancellationToken)
        {
            var sales = (await _unitOfWork.SaleRepository.FindBySessionIdAsync(session.Id, cancellationToken)
                .ConfigureAwait(false)).ToList();
            var net = sales.Sum(s => s.NetAmount);
            var tax = sales.Sum(s => s.TaxAmount);
            var cost = sales.Sum(s => s.CostAmount);
            if (net == 0 && tax == 0 && sales.Sum(s => s.Total) > 0)
                net = sales.Sum(s => s.Total);

            var difference = session.Difference ?? 0m;
            var hash = PosTax.TotalsHash(
                session.CashSales,
                session.CardSales,
                session.TransferSales,
                net,
                tax,
                cost,
                difference);
            return (session.CashSales, session.CardSales, session.TransferSales, net, tax, cost, difference, hash);
        }

        private static List<JournalEntryLine> BuildJournalLines(
            SalesRegisterSession session,
            (decimal Cash, decimal Card, decimal Transfer, decimal Net, decimal Tax, decimal Cost, decimal Difference, string Hash) totals,
            PosAccountMap map,
            Dictionary<int, Account> accounts)
        {
            var date = session.ClosedAt ?? DateTime.Now;
            var reference = "POS-" + (session.RegisterCode ?? session.SalesRegisterId.ToString()) + "-" + session.Id;
            var memo = "Cierre caja " + (session.RegisterName ?? session.RegisterCode)
                + " " + session.OpenedAt.ToString("g") + " — " + date.ToString("g");
            var lines = new List<JournalEntryLine>();

            AddLine(lines, accounts, map.CashAccountId, totals.Cash, DebOrCred.Debito, date, reference, memo);
            AddLine(lines, accounts, map.CardAccountId, totals.Card, DebOrCred.Debito, date, reference, memo);
            AddLine(lines, accounts, map.TransferAccountId, totals.Transfer, DebOrCred.Debito, date, reference, memo);
            AddLine(lines, accounts, map.SalesAccountId, totals.Net, DebOrCred.Credito, date, reference, memo);
            AddLine(lines, accounts, map.TaxAccountId, totals.Tax, DebOrCred.Credito, date, reference, memo);

            if (totals.Difference < 0)
            {
                var shortAmount = Math.Abs(totals.Difference);
                AddLine(lines, accounts, map.CashShortAccountId, shortAmount, DebOrCred.Debito, date, reference, "Faltante de caja " + memo);
                AddLine(lines, accounts, map.CashAccountId, shortAmount, DebOrCred.Credito, date, reference, "Faltante de caja " + memo);
            }
            else if (totals.Difference > 0)
            {
                AddLine(lines, accounts, map.CashAccountId, totals.Difference, DebOrCred.Debito, date, reference, "Sobrante de caja " + memo);
                AddLine(lines, accounts, map.CashOverAccountId, totals.Difference, DebOrCred.Credito, date, reference, "Sobrante de caja " + memo);
            }

            if (totals.Cost > 0)
            {
                AddLine(lines, accounts, map.CogsAccountId, totals.Cost, DebOrCred.Debito, date, reference, "Costo de ventas " + memo);
                AddLine(lines, accounts, map.InventoryAccountId, totals.Cost, DebOrCred.Credito, date, reference, "Costo de ventas " + memo);
            }

            return lines;
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

        private async Task<PostingPeriod> ResolvePeriodAsync(SalesRegisterSession session, CancellationToken cancellationToken)
        {
            var closed = session.ClosedAt ?? DateTime.Now;
            var periods = await _financial.GetPostingPeriodsAsync(session.CompanyId, cancellationToken).ConfigureAwait(false);
            return periods.FirstOrDefault(p => p.Date.Year == closed.Year && p.Date.Month == closed.Month);
        }

        private async Task<Dictionary<int, Account>> ValidateMapAccountsAsync(PosAccountMap map, CancellationToken cancellationToken)
        {
            var accounts = (await _financial.GetAccountsAsync(map.CompanyId, cancellationToken).ConfigureAwait(false))
                .ToDictionary(a => a.Id);
            RequireAuxiliar(accounts, map.CashAccountId, "efectivo");
            RequireAuxiliar(accounts, map.CardAccountId, "tarjeta");
            RequireAuxiliar(accounts, map.TransferAccountId, "transferencia");
            RequireAuxiliar(accounts, map.SalesAccountId, "ventas");
            RequireAuxiliar(accounts, map.TaxAccountId, "IVA");
            RequireAuxiliar(accounts, map.InventoryAccountId, "inventario");
            RequireAuxiliar(accounts, map.CogsAccountId, "costo de ventas");
            RequireAuxiliar(accounts, map.CashShortAccountId, "faltante de caja");
            RequireAuxiliar(accounts, map.CashOverAccountId, "sobrante de caja");
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

        private async Task SuggestMappedAccountsAsync(PosAccountMap map, CancellationToken cancellationToken)
        {
            var accounts = (await _financial.GetAccountsAsync(map.CompanyId, cancellationToken).ConfigureAwait(false))
                .Where(a => a.AccountType == AccountType.Cuenta_Auxiliar)
                .ToList();
            if (map.CashAccountId == 0)
                map.CashAccountId = FindAccountId(accounts, "CAJA");
            if (map.CardAccountId == 0)
                map.CardAccountId = FindAccountId(accounts, "BANCOS");
            if (map.TransferAccountId == 0)
            {
                map.TransferAccountId = FindAccountId(accounts, "CUENTAS POR COBRAR");
                if (map.TransferAccountId == 0)
                    map.TransferAccountId = map.CardAccountId;
            }
            if (map.SalesAccountId == 0)
                map.SalesAccountId = FindAccountId(accounts, PosAccountMap.VentasName);
            if (map.TaxAccountId == 0)
                map.TaxAccountId = FindAccountId(accounts, PosAccountMap.IvaPorPagarName);
            if (map.InventoryAccountId == 0)
                map.InventoryAccountId = FindAccountId(accounts, "INVENTARIOS");
            if (map.CogsAccountId == 0)
                map.CogsAccountId = FindAccountId(accounts, PosAccountMap.CostoMercaderiaName);
            if (map.CashShortAccountId == 0)
                map.CashShortAccountId = FindAccountId(accounts, PosAccountMap.FaltanteCajaName);
            if (map.CashOverAccountId == 0)
                map.CashOverAccountId = FindAccountId(accounts, PosAccountMap.SobranteCajaName);
        }

        private async Task EnsureAccountAsync(
            List<Account> accounts,
            string companyId,
            int userId,
            string name,
            string parentName,
            AccountTag fallbackTag,
            CancellationToken cancellationToken)
        {
            if (accounts.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
                return;

            var parent = accounts.FirstOrDefault(a =>
                    a.AccountType == AccountType.Cuenta_De_Mayor
                    && string.Equals(a.Name, parentName, StringComparison.OrdinalIgnoreCase))
                ?? accounts.FirstOrDefault(a =>
                    a.AccountType == AccountType.Cuenta_Titulo && a.AccountTag == fallbackTag);
            if (parent == null)
                throw new InvalidOperationException("No se encontró la cuenta padre para " + name);

            var account = new Account
            {
                Name = name,
                CompanyId = companyId,
                FatherAccount = parent.Id,
                AccountTag = parent.AccountTag,
                AccountType = AccountType.Cuenta_Auxiliar,
                Editable = true,
                Active = true,
                CreatedBy = userId,
                UpdatedBy = userId
            };
            await _financial.CreateAccountAsync(account, parent, cancellationToken).ConfigureAwait(false);
            accounts.Add(account);
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
