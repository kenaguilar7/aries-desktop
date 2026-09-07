using System.Linq;
using System.Threading.Tasks;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    [Collection("mysql-schema")]
    public class DatabaseMigratorTests
    {
        [MySqlFact]
        public async Task ApplyPending_is_idempotent()
        {
            var migrator = new DatabaseMigrator(MySqlTestConnection.ConnectionString);
            var first = await migrator.ApplyPendingAsync();
            var second = await migrator.ApplyPendingAsync();

            Assert.Empty(second.AppliedIds);
            Assert.Equal(SchemaMigrations.All.Count, second.AlreadyAppliedIds.Count);
            Assert.Equal(SchemaMigrations.All.Select(m => m.Id), first.AlreadyAppliedIds.Concat(first.AppliedIds));
        }

        [MySqlFact]
        public async Task History_table_has_every_migration_id()
        {
            var applied = await new DatabaseMigrator(MySqlTestConnection.ConnectionString).ReadAppliedIdsAsync();
            foreach (var migration in SchemaMigrations.All)
                Assert.Contains(migration.Id, applied);
        }
    }
}
