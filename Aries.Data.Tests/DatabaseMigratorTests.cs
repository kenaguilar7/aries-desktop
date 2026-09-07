using System.Linq;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    [Collection("mysql-schema")]
    public class DatabaseMigratorTests
    {
        [MySqlFact]
        public void ApplyPending_is_idempotent()
        {
            var migrator = new DatabaseMigrator(MySqlTestConnection.ConnectionString);
            var first = migrator.ApplyPending();
            var second = migrator.ApplyPending();

            Assert.Empty(second.AppliedIds);
            Assert.Equal(SchemaMigrations.All.Count, second.AlreadyAppliedIds.Count);
            Assert.Equal(SchemaMigrations.All.Select(m => m.Id), first.AlreadyAppliedIds.Concat(first.AppliedIds));
        }

        [MySqlFact]
        public void History_table_has_every_migration_id()
        {
            var applied = new DatabaseMigrator(MySqlTestConnection.ConnectionString).ReadAppliedIds();
            foreach (var migration in SchemaMigrations.All)
                Assert.Contains(migration.Id, applied);
        }
    }
}
