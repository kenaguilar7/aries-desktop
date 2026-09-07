using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Email;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Permissions;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Reports;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Repositories;

namespace AriesContador.Tests.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public FakeCompanyRepository Companies { get; } = new FakeCompanyRepository();
        public FakeUserRepository Users { get; } = new FakeUserRepository();
        public FakeAccountRepository Accounts { get; } = new FakeAccountRepository();
        public FakePostingPeriodRepository PostingPeriods { get; } = new FakePostingPeriodRepository();
        public FakeJournalEntryRepository JournalEntries { get; } = new FakeJournalEntryRepository();
        public FakeJournalEntryLineRepository JournalEntryLines { get; } = new FakeJournalEntryLineRepository();
        public FakeFinancialReportRepository FinancialReports { get; } = new FakeFinancialReportRepository();

        public ICompanyRepository CompanyRepository => Companies;
        public IUserRepository UserRepository => Users;
        public IAccountRepository AccountRepository => Accounts;
        public IPostingPeriodRepository PostingPeriodRepository => PostingPeriods;
        public IJournalEntryRepository JournalEntryRepository => JournalEntries;
        public IJournalEntryLineRepository JournalEntryLineRepository => JournalEntryLines;
        public IFinancialReportRepository FinancialReportRepository => FinancialReports;
        public IPermissionRepository PermissionRepository { get; } = new FakePermissionRepository();
        public IEmailRepository EmailRepository { get; } = new FakeEmailRepository();
    }

    public class FakePermissionRepository : IPermissionRepository
    {
        public List<ModulePermission> Modules { get; } = new List<ModulePermission>();
        public List<string> AssignedCodes { get; } = new List<string>();
        public int LastTargetUserId { get; private set; }
        public int LastUpdatedBy { get; private set; }

        public Task<IList<ModulePermission>> GetModulesAsync(int userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IList<ModulePermission>>(Modules);

        public Task<bool> AssignCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default)
        {
            LastTargetUserId = targetUserId;
            LastUpdatedBy = updatedByUserId;
            AssignedCodes.AddRange(companyCodes);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> UpdateWindowPermissionsAsync(IList<ModulePermission> modules, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
    }

    public class FakeEmailRepository : IEmailRepository
    {
        public Task<DataTable> GetLogAsync(CancellationToken cancellationToken = default) => Task.FromResult(new DataTable());
        public Task<bool> InsertAsync(MailMessageLog message, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    public class FakeCompanyRepository : ICompanyRepository
    {
        public List<Company> Added { get; } = new List<Company>();

        public Task AddAsync(Company entity, CancellationToken cancellationToken = default)
        {
            Added.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Company entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveAsync(Company entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Company>>(Added);
        public Task<string> LatestCodeAsync(CancellationToken cancellationToken = default) => Task.FromResult("C001");
        public Task<IEnumerable<string>> GetCodesAllowedForUserAsync(int userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<string>());
    }

    public class FakeUserRepository : IUserRepository
    {
        public List<User> Items { get; } = new List<User>();
        public int GetAllCalls { get; set; }

        public void Add(User entity)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
        }

        public Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var i = Items.FindIndex(x => x.Id == entity.Id);
            if (i >= 0) Items[i] = entity;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(User entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public Task<User> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)));

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            GetAllCalls++;
            return Task.FromResult<IEnumerable<User>>(Items);
        }
    }

    public class FakeAccountRepository : IAccountRepository
    {
        public List<Account> Items { get; } = new List<Account>();
        public List<Account> Removed { get; } = new List<Account>();
        public List<Account> Updated { get; } = new List<Account>();
        public List<int> PromotedFatherIds { get; } = new List<int>();
        public List<Account> AccountsWithBalance { get; } = new List<Account>();
        public List<Account> BalanceRows { get; } = new List<Account>();
        public bool? NameTakenOverride { get; set; }
        public bool HasOpenPeriodMovementsResult { get; set; }

        public Task AddAsync(Account entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0)
                entity.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddChildAsync(Account entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0)
                entity.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;
            Items.Add(entity);
            if (!entity.FatherAccount.HasValue || entity.FatherAccount.Value == 0)
                return Task.CompletedTask;

            var father = Items.FirstOrDefault(x => x.Id == entity.FatherAccount.Value);
            if (father != null && father.AccountType == AccountType.Cuenta_Auxiliar)
            {
                father.AccountType = AccountType.Cuenta_De_Mayor;
                PromotedFatherIds.Add(father.Id);
            }
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Account entity, CancellationToken cancellationToken = default) =>
            UpdateNameInfoAsync(entity, cancellationToken);

        public Task UpdateNameInfoAsync(Account entity, CancellationToken cancellationToken = default)
        {
            Updated.Add(entity);
            var i = Items.FindIndex(x => x.Id == entity.Id);
            if (i >= 0) Items[i] = entity;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Account entity, CancellationToken cancellationToken = default)
        {
            Removed.Add(entity);
            return Task.CompletedTask;
        }

        public Task<Account> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<Account>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Account>>(Items.Where(x => x.CompanyId == companyId).ToList());

        public IEnumerable<Account> GetDefaultAccounts() => DefaultChartOfAccounts.Create();

        public Task<IEnumerable<Account>> AccountsWithBalanceByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Account>>(AccountsWithBalance);

        public Task<bool> NameTakenAsync(int accountId, string companyId, string name, CancellationToken cancellationToken = default)
        {
            if (NameTakenOverride.HasValue)
                return Task.FromResult(NameTakenOverride.Value);

            return Task.FromResult(Items.Any(x =>
                x.Id != accountId
                && x.CompanyId == companyId
                && x.AccountTag == AccountTag.Activo
                && string.Equals(x.Name, name, StringComparison.Ordinal)));
        }

        public Task<bool> HasOpenPeriodMovementsAsync(int accountId, CancellationToken cancellationToken = default) =>
            Task.FromResult(HasOpenPeriodMovementsResult);

        public Task<IEnumerable<Account>> GetBalancesFromAccountInfoAsync(string companyId, DateTime from, DateTime to, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Account>>(BalanceRows);

        public Task<DataTable> GetMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default) =>
            Task.FromResult(new DataTable());
    }

    public class FakePostingPeriodRepository : IPostingPeriodRepository
    {
        public List<PostingPeriod> Items { get; } = new List<PostingPeriod>();
        public List<PostingPeriodEndClosing> Closed { get; } = new List<PostingPeriodEndClosing>();

        public Task AddAsync(PostingPeriod entity, CancellationToken cancellationToken = default)
        {
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(PostingPeriod entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveAsync(PostingPeriod entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<PostingPeriod>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<PostingPeriod>>(Items.Where(x => x.CompanyId == companyId).ToList());
        public Task ClosePostingPeriodAsync(PostingPeriodEndClosing postingPeriod, CancellationToken cancellationToken = default)
        {
            Closed.Add(postingPeriod);
            return Task.CompletedTask;
        }
    }

    public class FakeJournalEntryRepository : IJournalEntryRepository
    {
        public List<JournalEntry> Items { get; } = new List<JournalEntry>();
        public List<JournalEntry> Updated { get; } = new List<JournalEntry>();
        public List<JournalEntry> Restored { get; } = new List<JournalEntry>();
        public List<JournalEntry> Removed { get; } = new List<JournalEntry>();
        public int ConsecutiveNumber { get; set; } = 1;
        public bool FailAfterHeader { get; set; }

        public Task AddAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0)
                entity.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;

            if (FailAfterHeader)
                throw new InvalidOperationException("Fallo al insertar línea");

            if (entity.JournalEntryLines != null)
            {
                var nextId = 1;
                foreach (var line in entity.JournalEntryLines)
                {
                    line.JournalEntryId = entity.Id;
                    if (line.Id == 0)
                        line.Id = nextId++;
                    else if (line.Id >= nextId)
                        nextId = line.Id + 1;
                }
            }

            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            Updated.Add(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(JournalEntry entity, CancellationToken cancellationToken = default)
        {
            Removed.Add(entity);
            return Task.CompletedTask;
        }

        public Task<JournalEntry> GetByIdAsync(int jEntryId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == jEntryId));

        public Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int postPeriodId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntry>>(Items.Where(x => x.PostingPeriodId == postPeriodId).ToList());

        public Task<int> GetConsecutiveNumberAsync(int postingPeriodId, CancellationToken cancellationToken = default) =>
            Task.FromResult(ConsecutiveNumber);

        public Task<IEnumerable<JournalEntryDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<JournalEntryDeletedReport>());

        public Task RestoreJournalEntryAsync(JournalEntry entryLine, CancellationToken cancellationToken = default)
        {
            Restored.Add(entryLine);
            return Task.CompletedTask;
        }
    }

    public class FakeJournalEntryLineRepository : IJournalEntryLineRepository
    {
        public List<JournalEntryLine> Items { get; } = new List<JournalEntryLine>();
        public List<JournalEntryLine> Restored { get; } = new List<JournalEntryLine>();
        public List<JournalEntryLine> Removed { get; } = new List<JournalEntryLine>();

        public Task AddAsync(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task<int> AddAsyncWithReturnId(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
            return Task.FromResult(entity.Id);
        }

        public Task UpdateAsync(JournalEntryLine entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RemoveAsync(JournalEntryLine entity, CancellationToken cancellationToken = default)
        {
            Removed.Add(entity);
            return Task.CompletedTask;
        }

        public Task<JournalEntryLine> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<JournalEntryLine>> FindByJournalEntryIdAsync(int journalEntryId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryLine>>(Items.Where(x => x.JournalEntryId == journalEntryId).ToList());

        public Task<IEnumerable<JournalEntryLine>> FindByAccountIdAndPostingPeriodIdAsync(int accountId, int postingPeriodId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<JournalEntryLine>());

        public Task<IEnumerable<JournalEntryLineDeletedReport>> GetDeletedItemByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<JournalEntryLineDeletedReport>());

        public Task RestoreJournalEntryLineAsync(JournalEntryLine entryLine, CancellationToken cancellationToken = default)
        {
            Restored.Add(entryLine);
            return Task.CompletedTask;
        }
    }

    public class FakeFinancialReportRepository : IFinancialReportRepository
    {
        public List<Account> EstadoResultadoAccounts { get; } = new List<Account>();
        public List<JournalEntryReport> JournalReports { get; } = new List<JournalEntryReport>();

        public Task<IEnumerable<JournalEntryReport>> JournalEntryReportAsync(BasicReportParam jEParams, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<JournalEntryReport>>(JournalReports);

        public Task<IEnumerable<Account>> EstadoResultadoIntegralAccountsAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<Account>>(EstadoResultadoAccounts);

        public Task<IEnumerable<PostingPeriodInfo>> PostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<PostingPeriodInfo>());

        public Task<IEnumerable<ClosingPostingPeriodReport>> ClosingPostingPeriodReportAsync(string companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Enumerable.Empty<ClosingPostingPeriodReport>());
    }
}
