using System;
using MySql.Data.MySqlClient;
using Xunit;

namespace Aries.Data.Tests
{
    internal static class MySqlTestConnection
    {
        public static string ConnectionString { get; } =
            Environment.GetEnvironmentVariable("ARIES_MYSQL_CONNECTION")
            ?? "Server=127.0.0.1;Port=3307;User id=kenneth;Password=1234;Database=aries;Allow User Variables=True";

        public static bool IsAvailable { get; } = Probe();

        private static bool Probe()
        {
            try
            {
                var cs = ConnectionString.Trim().TrimEnd(';') + ";Connection Timeout=3";
                using (var connection = new MySqlConnection(cs))
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
    }

    public sealed class MySqlFactAttribute : FactAttribute
    {
        public MySqlFactAttribute()
        {
            if (!MySqlTestConnection.IsAvailable)
                Skip = "MySQL local no disponible (aries_mysql_local :3307 o ARIES_MYSQL_CONNECTION)";
        }
    }

    public sealed class MySqlSchemaFixture
    {
        public MySqlSchemaFixture()
        {
            if (!MySqlTestConnection.IsAvailable)
                return;

            new AriesContador.Data.Migrations.DatabaseMigrator(MySqlTestConnection.ConnectionString).ApplyPending();
        }
    }

    [CollectionDefinition("mysql-schema")]
    public sealed class MySqlSchemaCollection : ICollectionFixture<MySqlSchemaFixture>
    {
    }
}
