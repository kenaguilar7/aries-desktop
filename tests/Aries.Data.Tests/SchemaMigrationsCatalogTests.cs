using System.Linq;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    public class SchemaMigrationsCatalogTests
    {
        [Fact]
        public void All_has_unique_versions_and_ids_in_order()
        {
            var all = SchemaMigrations.All;
            Assert.NotEmpty(all);
            Assert.Equal(all.Select(m => m.Version).Distinct().Count(), all.Count);
            Assert.Equal(all.Select(m => m.Id).Distinct().Count(), all.Count);
            Assert.Equal(all.OrderBy(m => m.Version).Select(m => m.Id), all.Select(m => m.Id));
        }

        [Fact]
        public void Each_migration_has_sql_batches()
        {
            foreach (var migration in SchemaMigrations.All)
            {
                Assert.False(string.IsNullOrWhiteSpace(migration.Description), migration.Id);
                Assert.NotEmpty(migration.SqlBatches);
                foreach (var batch in migration.SqlBatches)
                    Assert.False(string.IsNullOrWhiteSpace(batch), migration.Id);
            }
        }

        [Fact]
        public void Expected_schema_lists_are_unique()
        {
            Assert.Equal(ExpectedSchema.Tables.Distinct().Count(), ExpectedSchema.Tables.Length);
            Assert.Equal(ExpectedSchema.ProceduresCalledByCode.Distinct().Count(), ExpectedSchema.ProceduresCalledByCode.Length);
            Assert.Equal(ExpectedSchema.Functions.Distinct().Count(), ExpectedSchema.Functions.Length);
        }

        [Fact]
        public void Initial_set_includes_scripts_missing_from_dump()
        {
            var ids = SchemaMigrations.All.Select(m => m.Id).ToArray();
            Assert.Contains("002_CompanyProcedures", ids);
            Assert.Contains("003_AccountMaestroProcedures", ids);
            Assert.Contains("004_UserPasswordAndProcedures", ids);
            Assert.Contains("005_UnifyCompanyIdVarchar5", ids);
            Assert.Contains("006_PermissionForeignKeys", ids);
            Assert.Contains("009_AccountPathFunctions", ids);
            Assert.Contains("010_WidenCompanyIdOnDumpProcedures", ids);
            Assert.Contains("013_CopyCompanyChart", ids);
        }

        [Fact]
        public void Sql_batches_split_on_marker()
        {
            var batches = new BatchProbe().SqlBatches;
            Assert.Equal(2, batches.Count);
            Assert.Equal("DROP PROCEDURE IF EXISTS `X`;", batches[0]);
            Assert.StartsWith("CREATE PROCEDURE", batches[1]);
        }

        private sealed class BatchProbe : SqlMigration
        {
            public override int Version => 99;
            public override string Id => "probe";
            public override string Description => "probe";
            public override string Sql => "DROP PROCEDURE IF EXISTS `X`;\n-- BATCH\nCREATE PROCEDURE `X`() BEGIN SELECT 1; END";
        }
    }
}
