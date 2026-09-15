namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Facturas de compra y líneas. Soft delete; no asienta (posteo en T4).
    /// Unique (company_id, supplier_id, document_number).
    /// </summary>
    public sealed class M022_Purchases : SqlMigration
    {
        public override int Version => 22;
        public override string Id => "022_Purchases";
        public override string Description => "Tablas purchases y purchase_lines: facturas de compra e inventario";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `purchases` (
  `purchase_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `supplier_id` INT UNSIGNED NOT NULL,
  `document_number` VARCHAR(80) NOT NULL,
  `purchased_at` DATETIME NOT NULL,
  `payment_method` ENUM('efectivo','tarjeta','transferencia','credito') NOT NULL,
  `payment_reference` VARCHAR(80) NULL,
  `total` DECIMAL(18,2) NOT NULL,
  `net_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `tax_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `notes` VARCHAR(500) NULL,
  `status` ENUM('draft','confirmed') NOT NULL DEFAULT 'confirmed',
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`purchase_id`),
  UNIQUE KEY `uk_purchases_company_supplier_doc` (`company_id`, `supplier_id`, `document_number`),
  KEY `ix_purchases_company` (`company_id`),
  KEY `ix_purchases_supplier` (`supplier_id`),
  KEY `ix_purchases_purchased_at` (`company_id`, `purchased_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `purchase_lines` (
  `purchase_line_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `purchase_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `product_name` VARCHAR(120) NOT NULL,
  `quantity` DECIMAL(18,3) NOT NULL DEFAULT 0,
  `unit_price` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `line_total` DECIMAL(18,2) NOT NULL,
  `net_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `tax_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `tax_exempt` TINYINT(1) NOT NULL DEFAULT 0,
  `cost_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`purchase_line_id`),
  KEY `ix_purchase_lines_purchase` (`purchase_id`),
  KEY `ix_purchase_lines_product` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
