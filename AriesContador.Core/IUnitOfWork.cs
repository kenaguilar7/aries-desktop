using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Repositories;
using System.Threading.Tasks;

namespace AriesContador.Core
{
    public interface IUnitOfWork
    {
        ICompanyRepository CompanyRepository { get;  }
        IUserRepository UserRepository { get;  }
        IAccountRepository AccountRepository { get;  }
        IPostingPeriodRepository PostingPeriodRepository { get;  }
        IJournalEntryRepository JournalEntryRepository { get;  }
        IJournalEntryLineRepository JournalEntryLineRepository { get;  }
        IFinancialReportRepository FinancialReportRepository { get; }

        int Commit(); 
    }
}
