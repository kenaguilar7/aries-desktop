namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Costo e IVA en POS, mapeo de cuentas y vínculo sesión → asiento.
    /// </summary>
    public sealed class M020_PosAccounting : SqlMigration
    {
        public override int Version => 20;
        public override string Id => "020_PosAccounting";
        public override string Description => "POS: costo, IVA, mapeo de cuentas y posteo de sesiones";

        public override string Sql => @"
ALTER TABLE `products`
  ADD COLUMN `cost` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `price`,
  ADD COLUMN `tax_exempt` TINYINT(1) NOT NULL DEFAULT 0 AFTER `price_per_kilo`;
-- BATCH
ALTER TABLE `sales`
  ADD COLUMN `net_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `total`,
  ADD COLUMN `tax_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `net_amount`,
  ADD COLUMN `cost_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `tax_amount`;
-- BATCH
ALTER TABLE `sale_lines`
  ADD COLUMN `net_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `line_total`,
  ADD COLUMN `tax_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `net_amount`,
  ADD COLUMN `cost_amount` DECIMAL(18,2) NOT NULL DEFAULT 0 AFTER `tax_amount`,
  ADD COLUMN `tax_exempt` TINYINT(1) NOT NULL DEFAULT 0 AFTER `cost_amount`;
-- BATCH
UPDATE `sales` SET `net_amount` = `total` WHERE `net_amount` = 0 AND `total` <> 0;
-- BATCH
UPDATE `sale_lines` SET `net_amount` = `line_total` WHERE `net_amount` = 0 AND `line_total` <> 0;
-- BATCH
CREATE TABLE IF NOT EXISTS `pos_account_maps` (
  `pos_account_map_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `cash_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `card_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `transfer_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `sales_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `tax_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `inventory_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `cogs_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `cash_short_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `cash_over_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `tax_rate` DECIMAL(8,4) NOT NULL DEFAULT 0.1300,
  `prices_include_tax` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`pos_account_map_id`),
  UNIQUE KEY `uk_pos_account_maps_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `pos_session_postings` (
  `pos_session_posting_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `session_id` INT UNSIGNED NOT NULL,
  `company_id` VARCHAR(5) NOT NULL,
  `journal_entry_id` INT UNSIGNED NOT NULL,
  `posted_at` DATETIME NOT NULL,
  `totals_hash` VARCHAR(120) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`pos_session_posting_id`),
  UNIQUE KEY `uk_pos_session_postings_session` (`session_id`),
  KEY `ix_pos_session_postings_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
