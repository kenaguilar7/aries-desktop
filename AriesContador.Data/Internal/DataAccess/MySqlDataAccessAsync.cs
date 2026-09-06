using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AriesContador.Data.Internal.DataAccess
{
    internal class MySqlDataAccessAsync : IDisposable
    {
        private readonly IConnectionString _connectionString;
        public MySqlDataAccessAsync(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var result = await connection.QueryAsync<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
                List<T> rows = result.ToList();
                return rows;
            }
        }

        public async Task ExecuteSingle<U>(string query, U parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                await connection.QueryAsync(query, parameters,
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<T>> LoadData<T>(string storedProcedure, CommandType commandType = CommandType.StoredProcedure)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var result = await connection.QueryAsync<T>(storedProcedure, commandType:
                    commandType);
                List<T> rows = result.ToList();
                return rows;
            }
        }

        public async Task<List<T>> ExecuteQuery<T>(string query)
            => await LoadData<T>(query, CommandType.Text);

        public async Task<List<T>> ExecuteQueryInTransaction<T>(string query)
            => await LoadData<T>(query, CommandType.Text);

        public async Task SaveData<T>(string storedProcedure, T parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Q> SaveData<T, Q>(string storedProcedure, T parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            DynamicParameters _params = new DynamicParameters();
            _params.Add($"@Id", direction: ParameterDirection.Output);
            _params.AddDynamicParams(parameters);

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, _params,
                    commandType: CommandType.StoredProcedure);
                var retVal = _params.Get<Q>("Id");

                return retVal;
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public Q SaveDataInTransaction<T, Q>(string storedProcedure, T parameters)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add($"@Id", direction: ParameterDirection.Output);
            _params.AddDynamicParams(parameters);

            _connection.Execute(storedProcedure, _params,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
            var retVal = _params.Get<Q>("Id");

            return retVal;
        }

        public async Task<Q> SaveDataInTransactionAsync<T, Q>(string storedProcedure, T parameters)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add($"@Id", direction: ParameterDirection.Output);
            _params.AddDynamicParams(parameters);

            await _connection.ExecuteAsync(storedProcedure, _params,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
            var retVal = _params.Get<Q>("Id");

            return retVal;
        }

        public async Task<List<T>> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            var result = await _connection.QueryAsync<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
            return result.ToList();
        }

        public void StartTransaction()
        {
            string connectionString = _connectionString.MySQLDefault;

            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();
        }

        public void RollBackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();
        }

        public void Dispose()
        {
            CommitTransaction();
        }
    }
}
