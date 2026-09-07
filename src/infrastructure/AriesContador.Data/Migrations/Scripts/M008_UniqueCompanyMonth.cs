namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M008_UniqueCompanyMonth : SqlMigration
    {
        public override int Version => 8;
        public override string Id => "008_UniqueCompanyMonth";
        public override string Description => "UNIQUE (company_id, month_report) si no hay duplicados";

        public override string Sql => @"
SET @dupes := (
  SELECT COUNT(*) FROM (
    SELECT 1 FROM accounting_months
    GROUP BY company_id, month_report
    HAVING COUNT(*) > 1
  ) d);
SET @exists := (
  SELECT COUNT(*) FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'accounting_months'
    AND INDEX_NAME = 'uk_accounting_months_company_month');
SET @sql := IF(@dupes = 0 AND @exists = 0,
  'ALTER TABLE `accounting_months` ADD UNIQUE KEY `uk_accounting_months_company_month` (`company_id`, `month_report`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
";
    }
}
