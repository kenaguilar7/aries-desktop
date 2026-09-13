namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Tablas del POS operativo: inventario, n cajas por compañía, sesiones y ventas.
    /// No asienta en contabilidad.
    /// </summary>
    public sealed class M019_PosTables : SqlMigration
    {
        public override int Version => 19;
        public override string Id => "019_PosTables";
        public override string Description => "Tablas POS: products, sales_registers, sessions, sales, sale_lines";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `products` (
  `product_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `barcode` VARCHAR(64) NOT NULL,
  `name` VARCHAR(120) NOT NULL,
  `category` VARCHAR(80) NOT NULL,
  `price` DECIMAL(18,2) NOT NULL,
  `stock` DECIMAL(18,3) NOT NULL DEFAULT 0,
  `sold_by_weight` TINYINT(1) NOT NULL DEFAULT 0,
  `price_per_kilo` DECIMAL(18,2) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `uk_products_company_barcode` (`company_id`, `barcode`),
  KEY `ix_products_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `sales_registers` (
  `sales_register_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `name` VARCHAR(80) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`sales_register_id`),
  UNIQUE KEY `uk_sales_registers_company_code` (`company_id`, `code`),
  KEY `ix_sales_registers_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `sales_register_sessions` (
  `session_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sales_register_id` INT UNSIGNED NOT NULL,
  `company_id` VARCHAR(5) NOT NULL,
  `opened_at` DATETIME NOT NULL,
  `closed_at` DATETIME NULL,
  `opening_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `opening_notes` VARCHAR(255) NULL,
  `cash_sales` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `card_sales` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `transfer_sales` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `expected_closing_amount` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `declared_closing_amount` DECIMAL(18,2) NULL,
  `difference` DECIMAL(18,2) NULL,
  `closing_notes` VARCHAR(255) NULL,
  `open_flag` TINYINT GENERATED ALWAYS AS (IF(`closed_at` IS NULL, 1, NULL)) STORED,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`session_id`),
  UNIQUE KEY `uk_session_one_open` (`sales_register_id`, `open_flag`),
  KEY `ix_sessions_company` (`company_id`),
  KEY `ix_sessions_register` (`sales_register_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `sales` (
  `sale_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `sales_register_id` INT UNSIGNED NOT NULL,
  `session_id` INT UNSIGNED NOT NULL,
  `payment_method` ENUM('efectivo','tarjeta','transferencia') NOT NULL,
  `payment_reference` VARCHAR(80) NULL,
  `total` DECIMAL(18,2) NOT NULL,
  `sold_at` DATETIME NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`sale_id`),
  KEY `ix_sales_company` (`company_id`),
  KEY `ix_sales_session` (`session_id`),
  KEY `ix_sales_register` (`sales_register_id`),
  KEY `ix_sales_sold_at` (`company_id`, `sold_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- BATCH
CREATE TABLE IF NOT EXISTS `sale_lines` (
  `sale_line_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `product_name` VARCHAR(120) NOT NULL,
  `quantity` DECIMAL(18,3) NOT NULL DEFAULT 0,
  `unit_price` DECIMAL(18,2) NOT NULL DEFAULT 0,
  `sold_by_weight` TINYINT(1) NOT NULL DEFAULT 0,
  `price_per_kilo` DECIMAL(18,2) NULL,
  `weight_grams` DECIMAL(18,3) NOT NULL DEFAULT 0,
  `line_total` DECIMAL(18,2) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`sale_line_id`),
  KEY `ix_sale_lines_sale` (`sale_id`),
  KEY `ix_sale_lines_product` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
