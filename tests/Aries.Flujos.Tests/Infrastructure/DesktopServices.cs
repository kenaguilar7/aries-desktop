using AriesContador.Core;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;

namespace Aries.Flujos.Tests.Infrastructure
{
    /// <summary>
    /// Misma composición que <c>Aries.Desktop.Program</c>: UnitOfWork + Administration + Financial + Reports.
    /// </summary>
    public sealed class DesktopServices
    {
        public IConnectionString Connection { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IAdministrationService Administration { get; }
        public IFinancialService Financial { get; }
        public IFinancialReportService Reports { get; }

        private DesktopServices(
            IConnectionString connection,
            IUnitOfWork unitOfWork,
            IAdministrationService administration,
            IFinancialService financial,
            IFinancialReportService reports)
        {
            Connection = connection;
            UnitOfWork = unitOfWork;
            Administration = administration;
            Financial = financial;
            Reports = reports;
        }

        public static DesktopServices Create(IConnectionString connection)
        {
            var unitOfWork = new UnitOfWork(connection);
            return new DesktopServices(
                connection,
                unitOfWork,
                new AdministrationService(unitOfWork),
                new FinancialService(unitOfWork),
                new FinancialReportService(unitOfWork));
        }
    }
}
