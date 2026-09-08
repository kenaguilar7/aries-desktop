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
SET @fk := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.KEY_COLUMN_USAGE
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'companies'
    AND COLUMN_NAME = 'user_id'
    AND REFERENCED_TABLE_NAME IS NOT NULL
  LIMIT 1);
SET @sql := IF(@fk IS NULL,
  'SELECT 1',
  CONCAT('ALTER TABLE `companies` DROP FOREIGN KEY `', @fk, '`'));
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
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
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'companies'
    AND CONSTRAINT_NAME = 'fk_companies_user'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `companies` ADD CONSTRAINT `fk_companies_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
";
    }
}
