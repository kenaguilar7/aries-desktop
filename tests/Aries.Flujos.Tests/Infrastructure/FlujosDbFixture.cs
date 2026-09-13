using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using AriesContador.Core.Models.Users;
using AriesContador.Data;
using AriesContador.Data.Migrations;
using MySql.Data.MySqlClient;
using Xunit;

namespace Aries.Flujos.Tests.Infrastructure
{
    public sealed class FlujosDbFixture
    {
        public const string AdminUserName = "flujos.admin";
        public const string AdminPassword = "FlujosAdmin.1";

        public static string DefaultConnectionString { get; } =
            Environment.GetEnvironmentVariable("ARIES_FLUJOS_MYSQL_CONNECTION")
            ?? "Server=127.0.0.1;Port=3308;User id=kenneth;Password=1234;Database=aries_flujos;Allow User Variables=True";

        public static bool IsAvailable { get; }
        public static string SkipReason { get; }
        public static int AdminUserId { get; }

        public string ConnectionString => DefaultConnectionString;
        public IConnectionString Connection { get; }
        public DesktopServices Services { get; }

        static FlujosDbFixture()
        {
            try
            {
                Environment.SetEnvironmentVariable("ARIES_SKIP_OPEN_EXCEL", "1");
                EnsureMysql();
                ApplyBaselineTables();
                new DatabaseMigrator(DefaultConnectionString).ApplyPendingAsync().GetAwaiter().GetResult();
                EnsureUserReadProcedures();
                ApplySqlFile("01_journal_procedures.sql");
                AdminUserId = EnsureAdminUser();
                IsAvailable = true;
            }
            catch (Exception ex)
            {
                SkipReason = "MySQL de flujos no disponible (Docker :3308 o ARIES_FLUJOS_MYSQL_CONNECTION): "
                             + ex.GetBaseException().Message;
                IsAvailable = false;
            }
        }

        public FlujosDbFixture()
        {
            Connection = new FlujosConnectionString(DefaultConnectionString);
            Services = IsAvailable ? DesktopServices.Create(Connection) : null;
        }

        private static void EnsureMysql()
        {
            if (CanConnect(TimeSpan.FromSeconds(3)))
                return;

            var root = FindRepoRoot();
            var composeFile = Path.Combine(root, "docker-compose.test.yml");
            if (!File.Exists(composeFile))
                throw new InvalidOperationException("No está docker-compose.test.yml en " + root);

            RunDockerCompose(root, composeFile);

            var deadline = DateTime.UtcNow.AddMinutes(4);
            while (DateTime.UtcNow < deadline)
            {
                if (CanConnect(TimeSpan.FromSeconds(5)))
                    return;
                Thread.Sleep(3000);
            }

            throw new TimeoutException("aries_mysql_flujos no respondió en :3308. ¿Está Docker Desktop?");
        }

        private static bool CanConnect(TimeSpan timeout)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(DefaultConnectionString)
                {
                    ConnectionTimeout = (uint)Math.Max(1, timeout.TotalSeconds)
                };
                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void RunDockerCompose(string root, string composeFile)
        {
            var start = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "compose -f \"" + composeFile + "\" up -d --wait --wait-timeout 240",
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(start))
            {
                if (process == null)
                    throw new InvalidOperationException("No se pudo iniciar docker compose.");

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                if (!process.WaitForExit(270000))
                {
                    try { process.Kill(); } catch { /* ignore */ }
                    throw new TimeoutException("docker compose up superó 2 minutos.");
                }

                if (process.ExitCode != 0)
                    throw new InvalidOperationException(
                        "docker compose up falló (" + process.ExitCode + "): " + error + output);
            }
        }

        private static void ApplyBaselineTables()
        {
            ApplySqlFile("00_baseline_tables.sql");
        }

        private static void ApplySqlFile(string fileName)
        {
            ExecuteBatches(File.ReadAllText(ResolveSchemaPath(fileName), Encoding.UTF8));
        }

        private static string ResolveSchemaPath(string fileName)
        {
            var fromOutput = Path.Combine(AppContext.BaseDirectory, "Schema", fileName);
            if (File.Exists(fromOutput))
                return fromOutput;

            var fromRepo = Path.Combine(FindRepoRoot(), "tests", "Aries.Flujos.Tests", "Schema", fileName);
            if (File.Exists(fromRepo))
                return fromRepo;

            throw new FileNotFoundException("No está Schema/" + fileName);
        }

        private static void EnsureUserReadProcedures()
        {
            ExecuteBatches(@"
DROP PROCEDURE IF EXISTS `SP_GetAllUsers`;
-- BATCH
CREATE PROCEDURE `SP_GetAllUsers`()
BEGIN
    SELECT
        T0.user_id AS Id,
        T0.user_name AS UserName,
        T0.user_type+0 AS UserType,
        T0.number_id AS IdNumber,
        T0.name AS Name,
        T0.lastname_p AS LastName,
        T0.lastname_m AS MiddleName,
        T0.phone_number AS PhoneNumber,
        T0.mail AS Mail,
        T0.notes AS Memo,
        T0.password AS Password,
        T0.created_at AS CreatedAt,
        T0.updated_at AS UpdateAt,
        T0.updated_by AS UpdatedBy,
        T0.active AS Active
    FROM users AS T0;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_FindUserById`;
-- BATCH
CREATE PROCEDURE `SP_FindUserById`(IN Id INT)
BEGIN
    SELECT
        T0.user_id AS Id,
        T0.user_name AS UserName,
        T0.user_type+0 AS UserType,
        T0.number_id AS IdNumber,
        T0.name AS Name,
        T0.lastname_p AS LastName,
        T0.lastname_m AS MiddleName,
        T0.phone_number AS PhoneNumber,
        T0.mail AS Mail,
        T0.notes AS Memo,
        T0.password AS Password,
        T0.created_at AS CreatedAt,
        T0.updated_at AS UpdateAt,
        T0.updated_by AS UpdatedBy,
        T0.active AS Active
    FROM users AS T0
    WHERE T0.user_id = Id
    LIMIT 1;
END
");
        }

        private static int EnsureAdminUser()
        {
            var existing = FlujosSql.Scalar<int>(
                DefaultConnectionString,
                "SELECT IFNULL((SELECT user_id FROM users WHERE user_name = @name LIMIT 1), 0)",
                ("@name", AdminUserName));
            if (existing > 0)
                return existing;

            var services = DesktopServices.Create(new FlujosConnectionString(DefaultConnectionString));
            services.Administration.CreateUserAsync(new User
            {
                UserName = AdminUserName,
                Password = AdminPassword,
                UserType = UserType.Administrador,
                IdNumber = "0-0000-0001",
                Name = "Admin",
                LastName = "Flujos",
                MiddleName = "Test",
                Mail = "flujos.admin@aries.test",
                PhoneNumber = "0000-0000",
                Memo = "semilla de Aries.Flujos.Tests",
                Active = true,
                UpdatedBy = 0
            }).GetAwaiter().GetResult();

            return FlujosSql.Scalar<int>(
                DefaultConnectionString,
                "SELECT user_id FROM users WHERE user_name = @name LIMIT 1",
                ("@name", AdminUserName));
        }

        private static void ExecuteBatches(string sql)
        {
            var batches = sql.Split(
                new[] { "\r\n-- BATCH\r\n", "\n-- BATCH\n", "\r\n-- BATCH\n", "\n-- BATCH\r\n" },
                StringSplitOptions.None);

            foreach (var raw in batches)
            {
                var batch = raw.Trim();
                if (batch.Length == 0)
                    continue;

                using (var connection = new MySqlConnection(DefaultConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = batch;
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "Aries.sln"))
                    && File.Exists(Path.Combine(dir.FullName, "docker-compose.test.yml")))
                    return dir.FullName;
                dir = dir.Parent;
            }

            throw new InvalidOperationException("No se encontró la raíz del repo (Aries.sln + docker-compose.test.yml).");
        }
    }

    [CollectionDefinition("flujos-db")]
    public sealed class FlujosDbCollection : ICollectionFixture<FlujosDbFixture>
    {
    }

    public sealed class DockerFactAttribute : FactAttribute
    {
        public DockerFactAttribute()
        {
            if (!FlujosDbFixture.IsAvailable)
                Skip = FlujosDbFixture.SkipReason ?? "MySQL de flujos no disponible";
        }
    }
}
