using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AriesContador.Data.Internal.DataAccess
{
    internal class MySqlDataAccess : IDisposable
    {
        private readonly IConnectionString _connectionString;
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
                var id = connection.Execute(storedProcedure, _params,
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

        public Q SaveDataInTransaction<T,Q>(string storedProcedure, T parameters)
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
