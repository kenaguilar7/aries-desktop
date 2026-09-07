using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace AriesContador.Data.Migrations
{
    public sealed class DatabaseSchemaReader
    {
        private readonly string _connectionString;

        public DatabaseSchemaReader(string connectionString)
        {
            _connectionString = DatabaseMigrator.EnsureAllowUserVariables(connectionString);
        }

        public HashSet<string> TableNames()
        {
            return ReadSet(
                "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE'");
        }

        public HashSet<string> ViewNames()
        {
            return ReadSet(
                "SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = DATABASE()");
        }

        public HashSet<string> ProcedureNames()
        {
            return ReadSet(
                "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = DATABASE() AND ROUTINE_TYPE = 'PROCEDURE'");
        }

        public HashSet<string> FunctionNames()
        {
            return ReadSet(
                "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = DATABASE() AND ROUTINE_TYPE = 'FUNCTION'");
        }

        public bool ColumnExists(string table, string column)
        {
            return ReadInt(
                "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND COLUMN_NAME = @c",
                "@t", table, "@c", column) > 0;
        }

        public ColumnSnapshot GetColumn(string table, string column)
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(
                "SELECT COLUMN_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS " +
                "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND COLUMN_NAME = @c LIMIT 1",
                connection))
            {
                command.Parameters.AddWithValue("@t", table);
                command.Parameters.AddWithValue("@c", column);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;
                    return new ColumnSnapshot
                    {
                        ColumnType = reader.GetString(0),
                        Nullable = string.Equals(reader.GetString(1), "YES", StringComparison.OrdinalIgnoreCase),
                        CharLength = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2)
                    };
                }
            }
        }

        public ParameterSnapshot GetParameter(string routine, string parameter)
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(
                "SELECT CAST(PARAMETER_MODE AS CHAR) AS PARAMETER_MODE, CAST(DTD_IDENTIFIER AS CHAR) AS DTD_IDENTIFIER " +
                "FROM information_schema.PARAMETERS " +
                "WHERE SPECIFIC_SCHEMA = DATABASE() AND SPECIFIC_NAME = @r AND PARAMETER_NAME = @p LIMIT 1",
                connection))
            {
                command.Parameters.AddWithValue("@r", routine);
                command.Parameters.AddWithValue("@p", parameter);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;
                    return new ParameterSnapshot
                    {
                        Mode = reader.IsDBNull(0) ? null : reader.GetString(0),
                        Type = reader.GetString(1)
                    };
                }
            }
        }

        public bool IndexExists(string table, string indexName)
        {
            return ReadInt(
                "SELECT COUNT(*) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND INDEX_NAME = @i",
                "@t", table, "@i", indexName) > 0;
        }

        public bool ForeignKeyExists(string table, string constraintName)
        {
            return ReadInt(
                "SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA = DATABASE() " +
                "AND TABLE_NAME = @t AND CONSTRAINT_NAME = @c AND CONSTRAINT_TYPE = 'FOREIGN KEY'",
                "@t", table, "@c", constraintName) > 0;
        }

        public int DuplicateCompanyMonthCount()
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(
                "SELECT COUNT(*) FROM (SELECT 1 FROM accounting_months GROUP BY company_id, month_report HAVING COUNT(*) > 1) d",
                connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private MySqlConnection Open()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private HashSet<string> ReadSet(string sql)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var connection = Open())
            using (var command = new MySqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    set.Add(reader.GetString(0));
            }
            return set;
        }

        private int ReadInt(string sql, string name1, string value1, string name2, string value2)
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(name1, value1);
                command.Parameters.AddWithValue(name2, value2);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    public sealed class ColumnSnapshot
    {
        public string ColumnType { get; set; }
        public bool Nullable { get; set; }
        public long? CharLength { get; set; }
    }

    public sealed class ParameterSnapshot
    {
        public string Mode { get; set; }
        public string Type { get; set; }
    }
}
