using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AriesContador.Data.Internal.DataAccess
{
    internal class MySqlDataAccess : IDisposable
    {
        private readonly IConnectionString _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _transactionOpen;
        private bool _completed;

        public MySqlDataAccess(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public List<T> LoadData<T>(string storedProcedure)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, commandType:
                    CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public List<T> ExecuteQuery<T, U>(string query, U parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                return connection.Query<T>(query, parameters, commandType: CommandType.Text).ToList();
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public Q SaveData<T, Q>(string storedProcedure, T parameters)
        {
            string connectionString = _connectionString.MySQLDefault;

            DynamicParameters _params = new DynamicParameters();
            _params.Add($"@Id", direction: ParameterDirection.Output);
            _params.AddDynamicParams(parameters);

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, _params,
                    commandType: CommandType.StoredProcedure);
                var retVal = _params.Get<Q>("Id");

                return retVal;
            }
        }

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

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }

        public void StartTransaction()
        {
            string connectionString = _connectionString.MySQLDefault;

            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            _transactionOpen = true;
            _completed = false;
        }

        public void CommitTransaction()
        {
            if (_transactionOpen && !_completed)
            {
                _transaction?.Commit();
                _completed = true;
                _transactionOpen = false;
            }
            CloseConnection();
        }

        public void RollBackTransaction()
        {
            if (_transactionOpen && !_completed)
            {
                _transaction?.Rollback();
                _completed = true;
                _transactionOpen = false;
            }
            CloseConnection();
        }

        public void Dispose()
        {
            if (_transactionOpen && !_completed)
                RollBackTransaction();
            else
                CloseConnection();
        }

        private void CloseConnection()
        {
            _connection?.Close();
            _connection = null;
            _transaction = null;
        }
    }
}
