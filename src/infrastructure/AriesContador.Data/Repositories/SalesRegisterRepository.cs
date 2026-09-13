using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using AriesContador.Data.Query;

namespace AriesContador.Data.Repositories
{
    public class SalesRegisterRepository : ISalesRegisterRepository
    {
        private readonly IConnectionString _connectionString;

        public SalesRegisterRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(SalesRegister entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.InsertAndGetIdAsync(PosQuery.InsertRegister, entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(SalesRegister entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PosQuery.UpdateRegister, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(SalesRegister entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PosQuery.DeactivateRegister, entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<SalesRegister> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<SalesRegister, object>(
                PosQuery.SelectRegisterById, new { Id = id }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<IEnumerable<SalesRegister>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<SalesRegister, object>(
                PosQuery.SelectRegistersByCompany, new { CompanyId = companyId }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<SalesRegisterSession> GetOpenSessionAsync(int registerId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<SalesRegisterSession, object>(
                PosQuery.SelectOpenSession, new { RegisterId = registerId }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task<SalesRegisterSession> GetSessionByIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteQueryAsync<SalesRegisterSession, object>(
                PosQuery.SelectSessionById, new { Id = sessionId }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault();
        }

        public async Task AddSessionAsync(SalesRegisterSession session, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            session.Id = await dataAccess.InsertAndGetIdAsync(PosQuery.InsertSession, session, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task CloseSessionAsync(SalesRegisterSession session, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.ExecuteTextAsync(PosQuery.CloseSession, session, cancellationToken)
                .ConfigureAwait(false);
            if (rows == 0)
                throw new System.InvalidOperationException("La caja no tiene una sesión abierta");
        }

        public async Task<IEnumerable<SalesRegisterSession>> GetSessionHistoryAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.ExecuteQueryAsync<SalesRegisterSession, object>(
                PosQuery.SelectSessionHistory, new { CompanyId = companyId }, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateSessionTotalsAsync(SalesRegisterSession session, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.ExecuteTextAsync(PosQuery.UpdateSessionTotals, session, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
