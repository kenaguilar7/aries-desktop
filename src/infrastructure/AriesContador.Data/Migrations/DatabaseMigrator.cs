using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AriesContador.Data.Migrations
{
    public sealed class DatabaseMigrator
    {
        public const string HistoryTableName = "__schema_migrations";
        public const string SchemaLockName = "aries_schema_migrate";

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

        public async Task<MigrationResult> ApplyPendingAsync(CancellationToken cancellationToken = default)
        {
            var appliedNow = new List<string>();
            var already = new List<string>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                if (!await TryAcquireLockAsync(connection, cancellationToken).ConfigureAwait(false))
                {
                    throw new InvalidOperationException(
                        "Otra instancia está actualizando el esquema MySQL. Cierre esa copia de Aries o el API e intente de nuevo.");
                }

                try
                {
                    await EnsureHistoryTableAsync(connection, cancellationToken).ConfigureAwait(false);

                    var applied = await GetAppliedAsync(connection, cancellationToken).ConfigureAwait(false);
                    foreach (var migration in _migrations)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string storedChecksum;
                        if (applied.TryGetValue(migration.Id, out storedChecksum))
                        {
                            already.Add(migration.Id);
                            await EnsureChecksumMatchesAsync(connection, migration, storedChecksum, cancellationToken)
                                .ConfigureAwait(false);
                            continue;
                        }

                        try
                        {
                            foreach (var batch in migration.SqlBatches)
                                await ExecuteAsync(connection, batch, cancellationToken).ConfigureAwait(false);

                            await RecordAsync(connection, migration, cancellationToken).ConfigureAwait(false);
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
                finally
                {
                    await ReleaseLockAsync(connection, CancellationToken.None).ConfigureAwait(false);
                }
            }

            return new MigrationResult(appliedNow, already);
        }

        public async Task<IReadOnlyCollection<string>> ReadAppliedIdsAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await EnsureHistoryTableAsync(connection, cancellationToken).ConfigureAwait(false);
                return (await GetAppliedAsync(connection, cancellationToken).ConfigureAwait(false)).Keys;
            }
        }

        public async Task<MigrationStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var historyExists = await HistoryTableExistsAsync(connection, cancellationToken).ConfigureAwait(false);
                IReadOnlyDictionary<string, string> applied = historyExists
                    ? await GetAppliedAsync(connection, cancellationToken).ConfigureAwait(false)
                    : new Dictionary<string, string>(StringComparer.Ordinal);
                return MigrationStatus.From(_migrations, applied, historyExists);
            }
        }

        internal static string EnsureAllowUserVariables(string connectionString)
        {
            if (connectionString.IndexOf("Allow User Variables", StringComparison.OrdinalIgnoreCase) >= 0)
                return connectionString;
            return connectionString.Trim().TrimEnd(';') + ";Allow User Variables=True";
        }

        private static async Task<bool> HistoryTableExistsAsync(
            MySqlConnection connection,
            CancellationToken cancellationToken)
        {
            using (var command = new MySqlCommand(
                "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @name",
                connection))
            {
                command.Parameters.AddWithValue("@name", HistoryTableName);
                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return result != null && result != DBNull.Value && Convert.ToInt64(result) > 0;
            }
        }

        private static async Task EnsureHistoryTableAsync(MySqlConnection connection, CancellationToken cancellationToken)
        {
            await ExecuteAsync(connection, @"
CREATE TABLE IF NOT EXISTS `" + HistoryTableName + @"` (
  `migration_id` VARCHAR(128) NOT NULL,
  `version` INT NOT NULL,
  `description` VARCHAR(255) NOT NULL,
  `checksum` CHAR(64) NULL,
  `applied_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`migration_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4", cancellationToken).ConfigureAwait(false);

            await ExecuteAsync(connection, @"
SET @aries_checksum_col := (
  SELECT COUNT(*) FROM information_schema.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = '" + HistoryTableName + @"'
    AND COLUMN_NAME = 'checksum');
SET @aries_checksum_sql := IF(@aries_checksum_col = 0,
  'ALTER TABLE `" + HistoryTableName + @"` ADD COLUMN `checksum` CHAR(64) NULL',
  'SELECT 1');
PREPARE aries_checksum_stmt FROM @aries_checksum_sql;
EXECUTE aries_checksum_stmt;
DEALLOCATE PREPARE aries_checksum_stmt;", cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Dictionary<string, string>> GetAppliedAsync(
            MySqlConnection connection,
            CancellationToken cancellationToken)
        {
            var ids = new Dictionary<string, string>(StringComparer.Ordinal);
            using (var command = new MySqlCommand(
                "SELECT `migration_id`, `checksum` FROM `" + HistoryTableName + "`", connection))
            using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var checksum = reader.IsDBNull(1) ? null : reader.GetString(1);
                    ids[reader.GetString(0)] = checksum;
                }
            }
            return ids;
        }

        private static async Task EnsureChecksumMatchesAsync(
            MySqlConnection connection,
            SqlMigration migration,
            string storedChecksum,
            CancellationToken cancellationToken)
        {
            var current = migration.Checksum;
            if (string.IsNullOrWhiteSpace(storedChecksum))
            {
                using (var command = new MySqlCommand(
                    "UPDATE `" + HistoryTableName + "` SET `checksum` = @sum WHERE `migration_id` = @id AND (`checksum` IS NULL OR `checksum` = '')",
                    connection))
                {
                    command.Parameters.AddWithValue("@sum", current);
                    command.Parameters.AddWithValue("@id", migration.Id);
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
                return;
            }

            if (!storedChecksum.Equals(current, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "La migración " + migration.Id + " ya está aplicada pero el SQL cambió (checksum distinto). "
                    + "No se reaplica sola. Revise el script o borre la fila en `" + HistoryTableName + "` si el cambio es intencional.");
            }
        }

        private static async Task RecordAsync(MySqlConnection connection, SqlMigration migration, CancellationToken cancellationToken)
        {
            using (var command = new MySqlCommand(
                "INSERT INTO `" + HistoryTableName + "` (`migration_id`, `version`, `description`, `checksum`) VALUES (@id, @ver, @desc, @sum)",
                connection))
            {
                command.Parameters.AddWithValue("@id", migration.Id);
                command.Parameters.AddWithValue("@ver", migration.Version);
                command.Parameters.AddWithValue("@desc", migration.Description ?? string.Empty);
                command.Parameters.AddWithValue("@sum", migration.Checksum);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<bool> TryAcquireLockAsync(MySqlConnection connection, CancellationToken cancellationToken)
        {
            using (var command = new MySqlCommand("SELECT GET_LOCK(@name, 60)", connection))
            {
                command.Parameters.AddWithValue("@name", SchemaLockName);
                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return result != null && result != DBNull.Value && Convert.ToInt64(result) == 1;
            }
        }

        private static async Task ReleaseLockAsync(MySqlConnection connection, CancellationToken cancellationToken)
        {
            try
            {
                using (var command = new MySqlCommand("SELECT RELEASE_LOCK(@name)", connection))
                {
                    command.Parameters.AddWithValue("@name", SchemaLockName);
                    await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                // El proceso está saliendo; el lock muere con la conexión.
            }
        }

        private static async Task ExecuteAsync(MySqlConnection connection, string sql, CancellationToken cancellationToken)
        {
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 180;
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
