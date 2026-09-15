namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Maestro de proveedores por compañía. Soft delete; no asienta.
    /// Unique (company_id, number_id) solo cuando number_id no está vacío.
    /// </summary>
    public sealed class M021_Suppliers : SqlMigration
    {
        public override int Version => 21;
        public override string Id => "021_Suppliers";
        public override string Description => "Tabla suppliers: maestro de proveedores por compañía";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `suppliers` (
  `supplier_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `company_id` VARCHAR(5) NOT NULL,
  `name` VARCHAR(160) NOT NULL,
  `id_type` TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `number_id` VARCHAR(40) NOT NULL DEFAULT '',
  `number_id_key` VARCHAR(40) GENERATED ALWAYS AS (IF(`number_id` = '', NULL, `number_id`)) STORED,
  `email` VARCHAR(120) NULL,
  `phone` VARCHAR(40) NULL,
  `address` VARCHAR(255) NULL,
  `notes` VARCHAR(500) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  `updated_by` INT UNSIGNED NULL,
  `active` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`supplier_id`),
  UNIQUE KEY `uk_suppliers_company_number_id` (`company_id`, `number_id_key`),
  KEY `ix_suppliers_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
    }
}
