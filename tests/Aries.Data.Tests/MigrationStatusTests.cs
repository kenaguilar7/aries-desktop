using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    public class MigrationStatusTests
    {
        [Fact]
        public void From_with_no_history_lists_the_whole_catalog_as_pending()
        {
            var catalog = new SqlMigration[] { new Probe(1, "001_A"), new Probe(2, "002_B") };
            var status = MigrationStatus.From(
                catalog,
                new Dictionary<string, string>(),
                historyTableExists: false);

            Assert.False(status.IsUpToDate);
            Assert.False(status.HistoryTableExists);
            Assert.Equal(2, status.CatalogVersion);
            Assert.Equal(0, status.AppliedVersion);
            Assert.Empty(status.Applied);
            Assert.Equal(new[] { "001_A", "002_B" }, status.Pending.Select(m => m.Id));
        }

        [Fact]
        public void From_reports_pending_and_checksum_mismatch()
        {
            var first = new Probe(1, "001_A");
            var second = new Probe(2, "002_B");
            var applied = new Dictionary<string, string>
            {
                { first.Id, "not-the-real-checksum" }
            };

            var status = MigrationStatus.From(new SqlMigration[] { first, second }, applied, historyTableExists: true);

            Assert.False(status.IsUpToDate);
            Assert.Equal(1, status.AppliedVersion);
            Assert.Equal(2, status.CatalogVersion);
            Assert.Equal(new[] { "001_A" }, status.Applied.Select(m => m.Id));
            Assert.Equal(new[] { "002_B" }, status.Pending.Select(m => m.Id));
            Assert.Equal(new[] { "001_A" }, status.ChecksumMismatches);
        }

        [Fact]
        public void From_is_up_to_date_when_every_id_is_applied()
        {
            var catalog = new SqlMigration[] { new Probe(1, "001_A"), new Probe(2, "002_B") };
            var applied = catalog.ToDictionary(m => m.Id, m => m.Checksum);

            var status = MigrationStatus.From(catalog, applied, historyTableExists: true);

            Assert.True(status.IsUpToDate);
            Assert.Empty(status.Pending);
            Assert.Empty(status.ChecksumMismatches);
            Assert.Equal(2, status.AppliedVersion);
        }

        [MySqlFact]
        public async Task GetStatus_after_apply_is_up_to_date()
        {
            var migrator = new DatabaseMigrator(MySqlTestConnection.ConnectionString);
            await migrator.ApplyPendingAsync();

            var status = await migrator.GetStatusAsync();

            Assert.True(status.HistoryTableExists);
            Assert.True(status.IsUpToDate);
            Assert.Empty(status.Pending);
            Assert.Equal(SchemaMigrations.All.Count, status.Applied.Count);
            Assert.Equal(SchemaMigrations.All.Max(m => m.Version), status.AppliedVersion);
        }

        [MySqlFact]
        public async Task GetStatus_reports_pending_when_catalog_is_ahead()
        {
            await new DatabaseMigrator(MySqlTestConnection.ConnectionString).ApplyPendingAsync();

            var ahead = SchemaMigrations.All.Concat(new SqlMigration[] { new Probe(99, "099_Extra") }).ToArray();
            var status = await new DatabaseMigrator(MySqlTestConnection.ConnectionString, ahead).GetStatusAsync();

            Assert.False(status.IsUpToDate);
            Assert.Equal(99, status.CatalogVersion);
            Assert.Contains("099_Extra", status.Pending.Select(m => m.Id));
        }

        private sealed class Probe : SqlMigration
        {
            public Probe(int version, string id)
            {
                Version = version;
                Id = id;
            }

            public override int Version { get; }
            public override string Id { get; }
            public override string Description => Id;
            public override string Sql => "SELECT 1;";
        }
    }
}
