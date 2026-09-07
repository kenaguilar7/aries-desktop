using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace AriesContador.Data.Migrations
{
    public sealed class DatabaseMigrator
    {
        public const string HistoryTableName = "__schema_migrations";

        private readonly string _connectionString;
        private readonly IReadOnlyList<SqlMigration> _migrations;

        public DatabaseMigrator(string connectionString)
            : this(connectionString, SchemaMigrations.All)
        {
        }

        public DatabaseMigrator(string connectionString, IEnumerable<SqlMigration> migrations)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Falta la cadena de conexión a MySQL.", nameof(connectionString));
            if (migrations == null)
                throw new ArgumentNullException(nameof(migrations));

            _connectionString = EnsureAllowUserVariables(connectionString);
            var copy = new List<SqlMigration>(migrations);
            copy.Sort((a, b) =>
            {
                var byVersion = a.Version.CompareTo(b.Version);
                return byVersion != 0 ? byVersion : string.CompareOrdinal(a.Id, b.Id);
            });
            _migrations = copy;
        }

        public IReadOnlyList<SqlMigration> Migrations => _migrations;

        public MigrationResult ApplyPending()
        {
            var appliedNow = new List<string>();
            var already = new List<string>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                EnsureHistoryTable(connection);

                var applied = GetAppliedIds(connection);
                foreach (var migration in _migrations)
                {
                    if (applied.Contains(migration.Id))
                    {
                        already.Add(migration.Id);
                        continue;
                    }

                    try
                    {
                        foreach (var batch in migration.SqlBatches)
                            Execute(connection, batch);

                        Record(connection, migration);
                        appliedNow.Add(migration.Id);
                    }
                    catch (Exception ex)
                    {
                        var hint = string.Empty;
                        if (ex.Message != null && ex.Message.IndexOf("SYSTEM_USER", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            hint = " El usuario de conexión no puede reemplazar rutinas restauradas por root. En Docker local: scripts/mysql/grant_routine_replace.sql (start-local.ps1 lo aplica).";
                        }

                        throw new InvalidOperationException(
                            "Falló la migración " + migration.Id + " (" + migration.Description + "): " + ex.Message + hint, ex);
                    }
                }
            }

            return new MigrationResult(appliedNow, already);
        }

        public IReadOnlyCollection<string> ReadAppliedIds()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                EnsureHistoryTable(connection);
                return GetAppliedIds(connection);
            }
        }

        internal static string EnsureAllowUserVariables(string connectionString)
        {
            if (connectionString.IndexOf("Allow User Variables", StringComparison.OrdinalIgnoreCase) >= 0)
                return connectionString;
            return connectionString.Trim().TrimEnd(';') + ";Allow User Variables=True";
        }

        private static void EnsureHistoryTable(MySqlConnection connection)
        {
            Execute(connection, @"
CREATE TABLE IF NOT EXISTS `" + HistoryTableName + @"` (
  `migration_id` VARCHAR(128) NOT NULL,
  `version` INT NOT NULL,
  `description` VARCHAR(255) NOT NULL,
  `applied_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`migration_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
        }

        private static HashSet<string> GetAppliedIds(MySqlConnection connection)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            using (var command = new MySqlCommand("SELECT `migration_id` FROM `" + HistoryTableName + "`", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    ids.Add(reader.GetString(0));
            }
            return ids;
        }

        private static void Record(MySqlConnection connection, SqlMigration migration)
        {
            using (var command = new MySqlCommand(
                "INSERT INTO `" + HistoryTableName + "` (`migration_id`, `version`, `description`) VALUES (@id, @ver, @desc)",
                connection))
            {
                command.Parameters.AddWithValue("@id", migration.Id);
                command.Parameters.AddWithValue("@ver", migration.Version);
                command.Parameters.AddWithValue("@desc", migration.Description ?? string.Empty);
                command.ExecuteNonQuery();
            }
        }

        private static void Execute(MySqlConnection connection, string sql)
        {
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 180;
                command.ExecuteNonQuery();
            }
        }
    }
}
