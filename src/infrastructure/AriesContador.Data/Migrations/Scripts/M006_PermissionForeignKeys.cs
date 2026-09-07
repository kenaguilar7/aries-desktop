namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M006_PermissionForeignKeys : SqlMigration
    {
        public override int Version => 6;
        public override string Id => "006_PermissionForeignKeys";
        public override string Description => "Limpia huérfanos y agrega FKs de permisos";

        public override string Sql => @"
DELETE cp FROM companies_permission cp
LEFT JOIN users u ON u.user_id = cp.user_id
WHERE u.user_id IS NULL;
-- BATCH
DELETE cp FROM companies_permission cp
LEFT JOIN companies c ON c.company_id = cp.company_id
WHERE c.company_id IS NULL;
-- BATCH
DELETE wp FROM windows_permission wp
LEFT JOIN users u ON u.user_id = wp.user_id
WHERE u.user_id IS NULL;
-- BATCH
DELETE wp FROM windows_permission wp
LEFT JOIN modules m ON m.module_id = wp.module_id
WHERE m.module_id IS NULL;
-- BATCH
DELETE wp FROM windows_permission wp
LEFT JOIN windows w ON w.window_id = wp.window_id
WHERE w.window_id IS NULL;
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'companies_permission'
    AND CONSTRAINT_NAME = 'fk_companies_permission_user'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `companies_permission` ADD CONSTRAINT `fk_companies_permission_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'companies_permission'
    AND CONSTRAINT_NAME = 'fk_companies_permission_company'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `companies_permission` ADD CONSTRAINT `fk_companies_permission_company` FOREIGN KEY (`company_id`) REFERENCES `companies` (`company_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'windows_permission'
    AND CONSTRAINT_NAME = 'fk_windows_permission_user'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `windows_permission` ADD CONSTRAINT `fk_windows_permission_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'windows_permission'
    AND CONSTRAINT_NAME = 'fk_windows_permission_module'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `windows_permission` ADD CONSTRAINT `fk_windows_permission_module` FOREIGN KEY (`module_id`) REFERENCES `modules` (`module_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- BATCH
SET @exists := (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'windows_permission'
    AND CONSTRAINT_NAME = 'fk_windows_permission_window'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
SET @sql := IF(@exists = 0,
  'ALTER TABLE `windows_permission` ADD CONSTRAINT `fk_windows_permission_window` FOREIGN KEY (`window_id`) REFERENCES `windows` (`window_id`)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
";
    }
}
