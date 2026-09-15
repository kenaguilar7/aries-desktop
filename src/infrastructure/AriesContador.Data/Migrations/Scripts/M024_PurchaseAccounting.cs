namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Mapeo de cuentas de compras y vínculo factura → asiento.
    /// </summary>
    public sealed class M024_PurchaseAccounting : SqlMigration
    {
        public override int Version => 24;
        public override string Id => "024_PurchaseAccounting";
        public override string Description => "Compras: mapeo de cuentas y posteo a asientos";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `purchase_account_maps` (
  `purchase_account_map_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `inventory_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `tax_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `payable_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `cash_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `card_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `transfer_account_id` INT UNSIGNED NOT NULL DEFAULT 0,
  `tax_rate` DECIMAL(8,4) NOT NULL DEFAULT 0.1300,
  `prices_include_tax` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`purchase_account_map_id`),
  UNIQUE KEY `uk_purchase_account_maps_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `purchase_postings` (
  `purchase_posting_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `purchase_id` INT UNSIGNED NOT NULL,
  `company_id` VARCHAR(5) NOT NULL,
  `journal_entry_id` INT UNSIGNED NOT NULL,
  `posted_at` DATETIME NOT NULL,
  `totals_hash` VARCHAR(120) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`purchase_posting_id`),
  UNIQUE KEY `uk_purchase_postings_purchase` (`purchase_id`),
  KEY `ix_purchase_postings_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
