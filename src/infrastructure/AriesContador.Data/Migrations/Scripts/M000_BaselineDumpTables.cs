namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Tablas del dump (vacías). Las migraciones 001+ asumen este esquema.
    /// CREATE TABLE IF NOT EXISTS: no toca una copia restaurada.
    /// </summary>
    public sealed class M000_BaselineDumpTables : SqlMigration
    {
        public override int Version => 0;
        public override string Id => "000_BaselineDumpTables";
        public override string Description => "Tablas base del dump si la base está vacía";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `users` (
  `user_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_name` VARCHAR(20) NOT NULL,
  `user_type` ENUM('Usuario','Administrador') NOT NULL,
  `number_id` VARCHAR(20) NOT NULL,
  `name` VARCHAR(50) NULL,
  `lastname_p` VARCHAR(50) NULL,
  `lastname_m` VARCHAR(50) NULL,
  `phone_number` VARCHAR(50) NULL,
  `mail` VARCHAR(50) NULL,
  `notes` VARCHAR(100) NULL,
  `password` VARCHAR(50) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` INT NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uk_users_user_name` (`user_name`),
  UNIQUE KEY `uk_users_number_id` (`number_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `companies` (
  `company_id` VARCHAR(5) NOT NULL,
  `type_id` INT UNSIGNED NOT NULL,
  `number_id` VARCHAR(20) NOT NULL,
  `name` VARCHAR(50) NULL,
  `money_type` ENUM('Colones y Dolares','Colones','Dolares') NOT NULL,
  `op1` VARCHAR(50) NULL,
  `op2` VARCHAR(50) NULL,
  `address` VARCHAR(100) NULL,
  `website` VARCHAR(50) NULL,
  `mail` VARCHAR(50) NULL,
  `phone_number1` VARCHAR(50) NULL,
  `phone_number2` VARCHAR(50) NULL,
  `notes` VARCHAR(100) NULL,
  `user_id` INT UNSIGNED NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`company_id`),
  UNIQUE KEY `uk_companies_number_id` (`number_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `accounts_names` (
  `account_name_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`account_name_id`),
  UNIQUE KEY `uk_accounts_names_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `accounts` (
  `account_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_name_id` INT UNSIGNED NOT NULL,
  `father_account` INT UNSIGNED NULL,
  `previous_balance_c` DOUBLE(12,2) NOT NULL DEFAULT 0,
  `previous_balance_d` DOUBLE(12,2) NOT NULL DEFAULT 0,
  `company_id` VARCHAR(4) NOT NULL,
  `account_type` ENUM('ACTIVO','PASIVO','PATRIMONIO','INGRESO','COSTO VENTA','EGRESO') NOT NULL,
  `account_guide` ENUM('TITULO','CUENTA DE MAYOR','CUENTA AUXILIAR') NOT NULL,
  `editable` TINYINT(1) NOT NULL DEFAULT 1,
  `detail` VARCHAR(50) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `accounting_months` (
  `accounting_months_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `month_report` DATE NOT NULL,
  `closed` TINYINT(1) NOT NULL DEFAULT 0,
  `company_id` VARCHAR(4) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` INT UNSIGNED NOT NULL DEFAULT 0,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`accounting_months_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `accounting_entries` (
  `accounting_entry_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `entry_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `accounting_months_id` INT UNSIGNED NOT NULL,
  `convalidated` TINYINT(1) NOT NULL DEFAULT 0,
  `convalidated_at` TIMESTAMP NULL,
  `status` ENUM('In Progress','Approved','Convalidated') NOT NULL DEFAULT 'In Progress',
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`accounting_entry_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `transactions_accounting` (
  `transaction_accounting_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` INT UNSIGNED NOT NULL,
  `accounting_entry_id` INT UNSIGNED NOT NULL,
  `reference` VARCHAR(100) NULL,
  `detail` VARCHAR(100) NULL,
  `balance` DOUBLE(18,2) NOT NULL DEFAULT 0,
  `foreign_amount` DOUBLE(18,2) NOT NULL DEFAULT 0,
  `balance_type` ENUM('Debito','Credito') NOT NULL,
  `money_type` ENUM('Colones','Dolares') NOT NULL,
  `money_chance` DOUBLE(8,2) NOT NULL DEFAULT 1,
  `bill_date` DATE NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`transaction_accounting_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `posting_period_end_closing` (
  `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(4) NOT NULL,
  `from_period_id` INT UNSIGNED NULL,
  `to_period_id` INT UNSIGNED NULL,
  `from_period` VARCHAR(50) NULL,
  `to_period` VARCHAR(50) NULL,
  `amount` DOUBLE(18,2) NOT NULL DEFAULT 0,
  `user_notes` VARCHAR(100) NULL,
  `updated_by` INT UNSIGNED NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `modules` (
  `module_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `internal_name` VARCHAR(50) NOT NULL,
  `external_name` VARCHAR(50) NULL,
  `users` ENUM('Usuario','Administrador') NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  `deleted` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`module_id`),
  UNIQUE KEY `uk_modules_internal_name` (`internal_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `windows` (
  `window_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `module_id` INT UNSIGNED NOT NULL,
  `internal_name` VARCHAR(50) NOT NULL,
  `external_name` VARCHAR(50) NULL,
  `comments` VARCHAR(100) NULL,
  `is_report` TINYINT(1) NOT NULL DEFAULT 0,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  `deleted` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`window_id`),
  UNIQUE KEY `uk_windows_internal_name` (`internal_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `windows_permission` (
  `permission_manager_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED NOT NULL,
  `module_id` INT UNSIGNED NOT NULL,
  `window_id` INT UNSIGNED NOT NULL,
  `u_insert` TINYINT(1) NOT NULL DEFAULT 0,
  `u_update` TINYINT(1) NOT NULL DEFAULT 0,
  `u_remove` TINYINT(1) NOT NULL DEFAULT 0,
  `u_list` TINYINT(1) NOT NULL DEFAULT 0,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  `deleted` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`permission_manager_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `companies_permission` (
  `companies_permission_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED NOT NULL,
  `company_id` VARCHAR(4) NOT NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  `deleted` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`companies_permission_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
CREATE TABLE IF NOT EXISTS `usuarios_correo` (
  `mailusuario_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NULL,
  `apellido` VARCHAR(50) NULL,
  `correo_electronico` VARCHAR(100) NULL,
  `correo_copia` VARCHAR(100) NULL,
  `asunto` VARCHAR(100) NULL,
  `titulo` VARCHAR(100) NULL,
  `mensaje` TEXT NULL,
  `ultimo_envio` TIMESTAMP NULL,
  `estado` ENUM('CORRECTO','FALLIDO') NULL,
  PRIMARY KEY (`mailusuario_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
-- BATCH
SET @empty := (SELECT COUNT(*) FROM `users`);
SET @sql := IF(@empty = 0,
  'INSERT INTO `users` (`user_name`, `user_type`, `number_id`, `name`, `lastname_p`, `lastname_m`, `password`, `active`) VALUES (''ci_bootstrap'', ''Administrador'', ''CI000'', ''CI'', ''Bootstrap'', '''', ''x'', 1)',
  'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
";
    }
}
