using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Internal.DataAccess
{
    internal class MySqlDataAccess : IDisposable
    {
        private readonly IConnectionString _connectionString;
        private MySqlConnection _connection;
        private MySqlTransaction _transaction;
        private bool _transactionOpen;
        private bool _completed;

        public MySqlDataAccess(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = await connection.QueryAsync<T>(Proc(storedProcedure, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
                return result.ToList();
            }
        }

        public async Task<List<T>> LoadDataAsync<T>(string storedProcedure, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = await connection.QueryAsync<T>(Proc(storedProcedure, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
                return result.ToList();
            }
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string query, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = await connection.QueryAsync<T>(Text(query, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
                return result.ToList();
            }
        }

        public async Task<List<T>> ExecuteQueryAsync<T, U>(string query, U parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = await connection.QueryAsync<T>(Text(query, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
                return result.ToList();
            }
        }

        public async Task ExecuteSingleAsync<U>(string query, U parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await connection.ExecuteAsync(Text(query, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public async Task<int> ExecuteTextAsync(string sql, object parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return await connection.ExecuteAsync(Text(sql, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public async Task<DataTable> QueryTableAsync(string sql, object parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var reader = await connection.ExecuteReaderAsync(Text(sql, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                {
                    var table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public async Task SaveDataAsync<T>(string storedProcedure, T parameters, CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await connection.ExecuteAsync(Proc(storedProcedure, parameters, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
            }
        }

        public async Task<Q> SaveDataAsync<T, Q>(string storedProcedure, T parameters, CancellationToken cancellationToken = default)
        {
            var args = WithIdOutput(parameters);
            using (var connection = new MySqlConnection(_connectionString.MySQLDefault))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await connection.ExecuteAsync(Proc(storedProcedure, args, cancellationToken: cancellationToken))
                    .ConfigureAwait(false);
                return args.Get<Q>("Id");
            }
        }

        public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
        {
            _connection = new MySqlConnection(_connectionString.MySQLDefault);
            await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            _transaction = await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            _transactionOpen = true;
            _completed = false;
        }

        public async Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(Proc(storedProcedure, parameters, _transaction, cancellationToken))
                .ConfigureAwait(false);
        }

        public async Task<Q> SaveDataInTransactionAsync<T, Q>(string storedProcedure, T parameters, CancellationToken cancellationToken = default)
        {
            var args = WithIdOutput(parameters);
            await _connection.ExecuteAsync(Proc(storedProcedure, args, _transaction, cancellationToken))
                .ConfigureAwait(false);
            return args.Get<Q>("Id");
        }

        public async Task<List<T>> LoadDataInTransactionAsync<T, U>(string storedProcedure, U parameters, CancellationToken cancellationToken = default)
        {
            var result = await _connection.QueryAsync<T>(Proc(storedProcedure, parameters, _transaction, cancellationToken))
                .ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<int> ExecuteTextInTransactionAsync(string sql, object parameters, CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteAsync(Text(sql, parameters, _transaction, cancellationToken))
                .ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionOpen && !_completed)
            {
                await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                _completed = true;
                _transactionOpen = false;
            }
            CloseConnection();
        }

        public async Task RollBackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionOpen && !_completed)
            {
                await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                _completed = true;
                _transactionOpen = false;
            }
            CloseConnection();
        }

        public void Dispose()
        {
            if (_transactionOpen && !_completed)
            {
                try
                {
                    _transaction?.Rollback();
                }
                catch
                {
                    // last-resort cleanup; callers should RollbackAsync on failure
                }
                _completed = true;
                _transactionOpen = false;
            }
            CloseConnection();
        }

        private void CloseConnection()
        {
            _connection?.Close();
            _connection = null;
            _transaction = null;
        }

        private static DynamicParameters WithIdOutput<T>(T parameters)
        {
            var args = new DynamicParameters();
            args.Add("@Id", direction: ParameterDirection.Output);
            args.AddDynamicParams(parameters);
            return args;
        }

        private static CommandDefinition Proc(string sql, object parameters = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return new CommandDefinition(sql, parameters, transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        private static CommandDefinition Text(string sql, object parameters = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return new CommandDefinition(sql, parameters, transaction, commandType: CommandType.Text, cancellationToken: cancellationToken);
        }
    }
}
