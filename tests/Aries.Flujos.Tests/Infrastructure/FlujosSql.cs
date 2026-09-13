using System;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace Aries.Flujos.Tests.Infrastructure
{
    internal static class FlujosSql
    {
        public static T Scalar<T>(string connectionString, string sql, params (string name, object value)[] args)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var arg in args)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = arg.name;
                        parameter.Value = arg.value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    var result = command.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        return default;

                    return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
                }
            }
        }

        public static int Execute(string connectionString, string sql, params (string name, object value)[] args)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var arg in args)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = arg.name;
                        parameter.Value = arg.value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
