namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Anulación de compras: status cancelled + unique de documento solo mientras active=1
    /// (permite reutilizar el número de factura tras anular).
    /// </summary>
    public sealed class M026_PurchaseCancel : SqlMigration
    {
        public override int Version => 26;
        public override string Id => "026_PurchaseCancel";
        public override string Description => "Anulación de compras: status cancelled y unique de documento condicionado a active";

        public override string Sql => @"
ALTER TABLE `purchases`
  DROP INDEX `uk_purchases_company_supplier_doc`;
-- BATCH
ALTER TABLE `purchases`
  ADD COLUMN `document_number_key` VARCHAR(80)
    GENERATED ALWAYS AS (IF(`active` = 1, `document_number`, NULL)) STORED,
  ADD UNIQUE KEY `uk_purchases_company_supplier_doc` (`company_id`, `supplier_id`, `document_number_key`);
-- BATCH
ALTER TABLE `purchases`
  MODIFY COLUMN `status` ENUM('draft','confirmed','cancelled') NOT NULL DEFAULT 'confirmed';
";
    }
}
