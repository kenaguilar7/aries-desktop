using AriesContador.Core;
using AriesContador.Core.Repositories;
using AriesContador.Data.Repositories;
using System;

namespace AriesContador.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConnectionString _context;
        private IUserRepository _usuarioRepository;
        private IAccountRepository _cuentaRepository;
        private IPostingPeriodRepository _periodoContableRepository;
        private IJournalEntryRepository _asientoRepository;
        private IJournalEntryLineRepository _asientoLineaRepository;
        private CompanyRepository _companiaRepository;
        private IFinancialReportRepository _financialReportRepository;
        private IPermissionRepository _permissionRepository;
        private IEmailRepository _emailRepository;
        private IProductRepository _productRepository;
        private ISalesRegisterRepository _salesRegisterRepository;
        private ISaleRepository _saleRepository;
        private IPosAccountMapRepository _posAccountMapRepository;
        private IPosSessionPostingRepository _posSessionPostingRepository;

        public UnitOfWork(IConnectionString context)
        {
            this._context = context;
        }

        public IUserRepository UserRepository
            => _usuarioRepository = _usuarioRepository ?? new UserRepository(_context);

        public IAccountRepository AccountRepository
            => _cuentaRepository = _cuentaRepository ?? new AccountRepository(_context);

        public IPostingPeriodRepository PostingPeriodRepository 
            => _periodoContableRepository = _periodoContableRepository 
            ?? new PostingPeriodRepository(_context);

        public IJournalEntryRepository JournalEntryRepository
            => _asientoRepository = _asientoRepository ?? new JournalEntryRepository(_context); 

        public IJournalEntryLineRepository JournalEntryLineRepository 
            => _asientoLineaRepository = _asientoLineaRepository 
            ?? new JournalEntryLineRepository(_context);

        public IFinancialReportRepository FinancialReportRepository
            => _financialReportRepository = _financialReportRepository ?? new FinancialReportRepository(_context);

        public ICompanyRepository CompanyRepository 
            => _companiaRepository = _companiaRepository ?? new CompanyRepository(_context);

        public IPermissionRepository PermissionRepository
            => _permissionRepository = _permissionRepository ?? new PermissionRepository(_context);

        public IEmailRepository EmailRepository
            => _emailRepository = _emailRepository ?? new EmailRepository(_context);

        public IProductRepository ProductRepository
            => _productRepository = _productRepository ?? new ProductRepository(_context);

        public ISalesRegisterRepository SalesRegisterRepository
            => _salesRegisterRepository = _salesRegisterRepository ?? new SalesRegisterRepository(_context);

        public ISaleRepository SaleRepository
            => _saleRepository = _saleRepository ?? new SaleRepository(_context);

        public IPosAccountMapRepository PosAccountMapRepository
            => _posAccountMapRepository = _posAccountMapRepository ?? new PosAccountMapRepository(_context);

        public IPosSessionPostingRepository PosSessionPostingRepository
            => _posSessionPostingRepository = _posSessionPostingRepository ?? new PosSessionPostingRepository(_context);

        public void Dispose()
        {
        }
    }
}
