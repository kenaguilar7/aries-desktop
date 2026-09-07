namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M007_CompaniesUserNotNull : SqlMigration
    {
        public override int Version => 7;
        public override string Id => "007_CompaniesUserNotNull";
        public override string Description => "companies.user_id NOT NULL (asigna admin a nulos)";

        public override string Sql => @"
UPDATE companies c
JOIN (
    SELECT user_id FROM users
    WHERE user_type = 'Administrador' AND active = 1
    ORDER BY user_id
    LIMIT 1
) a
SET c.user_id = a.user_id
WHERE c.user_id IS NULL;
-- BATCH
SET @nullable := (
  SELECT IF(IS_NULLABLE = 'YES', 1, 0)
  FROM information_schema.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'companies'
    AND COLUMN_NAME = 'user_id');
SET @sql := IF(@nullable = 1,
  'ALTER TABLE `companies` MODIFY COLUMN `user_id` INT UNSIGNED NOT NULL',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
";
    }
}
