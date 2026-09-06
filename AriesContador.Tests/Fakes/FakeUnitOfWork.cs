using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
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

        public int Commit() => 0;
    }

    public class FakeCompanyRepository : ICompanyRepository
    {
        public void Add(Company entity) { }
        public Task AddAsync(Company entity) { Add(entity); return Task.CompletedTask; }
        public void Update(Company entity) { }
        public Task Remove(Company entity) => Task.CompletedTask;
        public Task<IEnumerable<Company>> GetAll() => Task.FromResult(Enumerable.Empty<Company>());
        public Task<string> LatestCode() => Task.FromResult("C001");
    }

    public class FakeUserRepository : IUserRepository
    {
        public void Add(User entity) { }
        public Task AddAsync(User entity) { Add(entity); return Task.CompletedTask; }
        public void Update(User entity) { }
        public Task Remove(User entity) => Task.CompletedTask;
        public User GetById(int id) => null;
        public IEnumerable<User> GetAll() => Enumerable.Empty<User>();
    }

    public class FakeAccountRepository : IAccountRepository
    {
        public List<Account> AccountsWithBalance { get; } = new List<Account>();

        public void Add(Account entity) { }
        public Task AddAsync(Account entity) { Add(entity); return Task.CompletedTask; }
        public void Update(Account entity) { }
        public Task Remove(Account entity) => Task.CompletedTask;
        public Task<Account> GetById(int id) => Task.FromResult<Account>(null);
        public IEnumerable<Account> FindByCompanyId(string companyId) => Enumerable.Empty<Account>();
        public IEnumerable<Account> GetDefaultAccounts() => Enumerable.Empty<Account>();
        public IEnumerable<Account> AccountsWithBalanceByDateRange(BasicReportParam reportParam) => AccountsWithBalance;
    }

    public class FakePostingPeriodRepository : IPostingPeriodRepository
    {
        public List<PostingPeriod> Items { get; } = new List<PostingPeriod>();
        public List<PostingPeriodEndClosing> Closed { get; } = new List<PostingPeriodEndClosing>();

        public void Add(PostingPeriod entity) => Items.Add(entity);
        public Task AddAsync(PostingPeriod entity) { Add(entity); return Task.CompletedTask; }
        public void Update(PostingPeriod entity) { }
        public Task Remove(PostingPeriod entity) => Task.CompletedTask;
        public IEnumerable<PostingPeriod> FindByCompanyId(string companyId) =>
            Items.Where(x => x.CompanyId == companyId).ToList();
        public Task<IEnumerable<PostingPeriod>> FindByCompanyIdAsync(string companyId) =>
            Task.FromResult(FindByCompanyId(companyId));
        public void ClosePostingPeriod(PostingPeriodEndClosing postingPeriod) => Closed.Add(postingPeriod);
    }

    public class FakeJournalEntryRepository : IJournalEntryRepository
    {
        public List<JournalEntry> Items { get; } = new List<JournalEntry>();
        public List<JournalEntry> Updated { get; } = new List<JournalEntry>();
        public List<JournalEntry> Restored { get; } = new List<JournalEntry>();
        public List<JournalEntry> Removed { get; } = new List<JournalEntry>();
        public int ConsecutiveNumber { get; set; } = 1;

        public void Add(JournalEntry entity)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
        }

        public Task AddAsync(JournalEntry entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public Task<int> AddAsyncReturningId(JournalEntry journalEntry)
        {
            Add(journalEntry);
            return Task.FromResult(journalEntry.Id);
        }

        public void Update(JournalEntry entity) => Updated.Add(entity);

        public Task UpdateAsync(JournalEntry entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public Task Remove(JournalEntry entity)
        {
            Removed.Add(entity);
            return Task.CompletedTask;
        }

        public JournalEntry GetById(int jEntryId) => Items.FirstOrDefault(x => x.Id == jEntryId);

        public IEnumerable<JournalEntry> FindByPostingPeriodId(int postPeriodId) =>
            Items.Where(x => x.PostingPeriodId == postPeriodId).ToList();

        public Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int pstPeriodId) =>
            Task.FromResult(FindByPostingPeriodId(pstPeriodId));

        public int GetConsecutiveNumber(int postingPeriodId) => ConsecutiveNumber;

        public Task<int> GetConsecutiveNumberAsync(int postingPeriodId) =>
            Task.FromResult(GetConsecutiveNumber(postingPeriodId));

        public IEnumerable<JournalEntryDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam) =>
            Enumerable.Empty<JournalEntryDeletedReport>();

        public void RestoreJournalEntry(JournalEntry entryLine) => Restored.Add(entryLine);
    }

    public class FakeJournalEntryLineRepository : IJournalEntryLineRepository
    {
        public List<JournalEntryLine> Items { get; } = new List<JournalEntryLine>();
        public List<JournalEntryLine> Restored { get; } = new List<JournalEntryLine>();
        public List<JournalEntryLine> Removed { get; } = new List<JournalEntryLine>();

        public void Add(JournalEntryLine entity)
        {
            if (entity.Id == 0) entity.Id = Items.Count + 1;
            Items.Add(entity);
        }

        public Task AddAsync(JournalEntryLine entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public Task<int> AddAsyncWithReturnId(JournalEntryLine entity)
        {
            Add(entity);
            return Task.FromResult(entity.Id);
        }

        public void Update(JournalEntryLine entity) { }

        public Task UpdateAsync(JournalEntryLine entity) => Task.CompletedTask;

        public Task Remove(JournalEntryLine entity)
        {
            Removed.Add(entity);
            return Task.CompletedTask;
        }

        public JournalEntryLine GetById(int id) => Items.FirstOrDefault(x => x.Id == id);

        public IEnumerable<JournalEntryLine> FindByJournalEntryId(int journalEntryId) =>
            Items.Where(x => x.JournalEntryId == journalEntryId).ToList();

        public Task<IEnumerable<JournalEntryLine>> FindByJournalEntryIdAsync(int journalEntryId) =>
            Task.FromResult(FindByJournalEntryId(journalEntryId));

        public IEnumerable<JournalEntryLine> FindByAccountIdAndPostingPeriodId(int accountId, int postingPeriodId) =>
            Enumerable.Empty<JournalEntryLine>();

        public IEnumerable<JournalEntryLineDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam) =>
            Enumerable.Empty<JournalEntryLineDeletedReport>();

        public void RestoreJournalEntryLine(JournalEntryLine entryLine) => Restored.Add(entryLine);
    }

    public class FakeFinancialReportRepository : IFinancialReportRepository
    {
        public List<Account> EstadoResultadoAccounts { get; } = new List<Account>();
        public List<JournalEntryReport> JournalReports { get; } = new List<JournalEntryReport>();

        public IEnumerable<JournalEntryReport> JournalEntryReport(BasicReportParam jEParams) => JournalReports;

        public IEnumerable<Account> EstadoResultadoIntegralAccounts(BasicReportParam reportParam) =>
            EstadoResultadoAccounts;

        public IEnumerable<PostingPeriodInfo> PostingPeriodReport(string companyId) =>
            Enumerable.Empty<PostingPeriodInfo>();

        public IEnumerable<ClosingPostingPeriodReport> ClosingPostingPeriodReport(string companyId) =>
            Enumerable.Empty<ClosingPostingPeriodReport>();
    }
}
