namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Pagos a proveedores: liquidación total de facturas a crédito ya asentadas.
    /// </summary>
    public sealed class M024_SupplierPayments : SqlMigration
    {
        public override int Version => 24;
        public override string Id => "024_SupplierPayments";
        public override string Description => "Tabla supplier_payments: pagos a proveedores (factura a crédito)";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `supplier_payments` (
  `supplier_payment_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `supplier_id` INT UNSIGNED NOT NULL,
  `purchase_id` INT UNSIGNED NOT NULL,
  `amount` DECIMAL(18,2) NOT NULL,
  `paid_at` DATETIME NOT NULL,
  `payment_method` ENUM('efectivo','tarjeta','transferencia') NOT NULL,
  `payment_reference` VARCHAR(80) NULL,
  `journal_entry_id` INT UNSIGNED NOT NULL,
  `notes` VARCHAR(500) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`supplier_payment_id`),
  UNIQUE KEY `uk_supplier_payments_purchase` (`purchase_id`),
  KEY `ix_supplier_payments_company` (`company_id`),
  KEY `ix_supplier_payments_supplier` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
