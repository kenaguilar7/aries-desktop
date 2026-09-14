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
            Assert.Contains("014_StandardizeReportContract", ids);
            Assert.Contains("015_AccountNameViaGetAccountName", ids);
            Assert.Contains("016_ReportMonthRangeYyyymm", ids);
            Assert.Contains("017_AccountAndJournalInfoViews", ids);
            Assert.Contains("018_AccountPathFunctions", ids);
            Assert.Contains("019_PosTables", ids);
            Assert.Contains("020_PosAccounting", ids);
        }

        [Fact]
        public void M020_adds_pos_accounting_tables()
        {
            var m020 = SchemaMigrations.All.Single(m => m.Id == "020_PosAccounting");
            Assert.Equal(20, m020.Version);
            Assert.Contains("ALTER TABLE `products`", m020.Sql);
            Assert.Contains("`cost`", m020.Sql);
            Assert.Contains("`tax_exempt`", m020.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `pos_account_maps`", m020.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `pos_session_postings`", m020.Sql);
            Assert.Contains("uk_pos_session_postings_session", m020.Sql);
        }

        [Fact]
        public void M019_creates_pos_tables()
        {
            var m019 = SchemaMigrations.All.Single(m => m.Id == "019_PosTables");
            Assert.Equal(19, m019.Version);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `products`", m019.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `sales_registers`", m019.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `sales_register_sessions`", m019.Sql);
            Assert.Contains("uk_session_one_open", m019.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `sales`", m019.Sql);
            Assert.Contains("CREATE TABLE IF NOT EXISTS `sale_lines`", m019.Sql);
        }

        [Fact]
        public void M018_functions_do_not_pin_definer()
        {
            var m018 = SchemaMigrations.All.Single(m => m.Id == "018_AccountPathFunctions");
            Assert.Equal(18, m018.Version);
            Assert.Contains("CREATE FUNCTION `F_GetAccountPathForReport`", m018.Sql);
            Assert.Contains("CREATE FUNCTION `GetAccountName`", m018.Sql);
            Assert.Contains("CREATE FUNCTION `GETFULLPATH`", m018.Sql);
            Assert.DoesNotContain("DEFINER", m018.Sql);
            Assert.DoesNotContain("`aries`.", m018.Sql);
        }

        [Fact]
        public void M017_views_do_not_pin_schema_or_definer()
        {
            var m017 = SchemaMigrations.All.Single(m => m.Id == "017_AccountAndJournalInfoViews");
            Assert.Equal(17, m017.Version);
            Assert.Contains("CREATE VIEW `account_info`", m017.Sql);
            Assert.Contains("CREATE VIEW `accounting_entries_info`", m017.Sql);
            Assert.DoesNotContain("`aries`.", m017.Sql);
            Assert.DoesNotContain("DEFINER", m017.Sql);
            Assert.DoesNotContain("SQL SECURITY DEFINER", m017.Sql);
        }

        [Fact]
        public void M014_aligns_account_tag_case_and_dual_deb_aliases()
        {
            var m014 = SchemaMigrations.All.Single(m => m.Id == "014_StandardizeReportContract");
            Assert.Equal(14, m014.Version);
            Assert.Contains("WHEN T0.`account_type`+ 0 = 4 THEN 'Ingreso'", m014.Sql);
            Assert.Contains("WHEN T0.`account_type`+ 0 = 5 THEN 'CostoVenta'", m014.Sql);
            Assert.DoesNotContain("WHEN T0.`account_type`+ 0 = 4 THEN 'CostoVenta'", m014.Sql);
            Assert.Contains("AS 'DebOrCred'", m014.Sql);
            Assert.Contains("AS 'DebOCred'", m014.Sql);
            Assert.Contains("AS 'RateAmount'", m014.Sql);
            Assert.Contains("AS 'Rate'", m014.Sql);
            Assert.Contains("SP_GetAccountsByCompanyId", m014.Sql);
            Assert.Contains("SP_GetAccountById", m014.Sql);
            Assert.Contains("SP_AuxiliaryAccountsWithBalanceByDateRange", m014.Sql);
            Assert.Contains("SP_EstadoResultadoIntegralReport", m014.Sql);
            Assert.Contains("SP_GetAllJournalEntyLineByAccoudIdAndPostingPeriodId", m014.Sql);
        }

        [Fact]
        public void Each_migration_has_stable_sha256_checksum()
        {
            foreach (var migration in SchemaMigrations.All)
            {
                Assert.Equal(64, migration.Checksum.Length);
                Assert.Equal(migration.Checksum, migration.Checksum);
                Assert.Matches("^[0-9a-f]{64}$", migration.Checksum);
            }
        }

        [Fact]
        public void Checksum_changes_when_sql_changes()
        {
            Assert.NotEqual(new BatchProbe().Checksum, new BatchProbeAlt().Checksum);
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

        private sealed class BatchProbeAlt : SqlMigration
        {
            public override int Version => 99;
            public override string Id => "probe";
            public override string Description => "probe";
            public override string Sql => "DROP PROCEDURE IF EXISTS `Y`;";
        }
    }
}
