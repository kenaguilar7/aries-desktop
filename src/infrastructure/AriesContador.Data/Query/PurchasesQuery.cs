namespace AriesContador.Data.Query
{
    public static class PurchasesQuery
    {
        public const string SupplierColumns = @"
T0.supplier_id AS Id,
T0.company_id AS CompanyId,
T0.name AS Name,
T0.id_type+0 AS IdType,
T0.number_id AS NumberId,
T0.email AS Email,
T0.phone AS Phone,
T0.address AS Address,
T0.notes AS Notes,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectSupplierById = @"
SELECT " + SupplierColumns + @"
FROM suppliers AS T0
WHERE T0.supplier_id = @Id
LIMIT 1";

        public const string SelectSuppliersByCompany = @"
SELECT " + SupplierColumns + @"
FROM suppliers AS T0
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.name";

        public const string SelectSupplierByNumberId = @"
SELECT " + SupplierColumns + @"
FROM suppliers AS T0
WHERE T0.company_id = @CompanyId AND T0.number_id = @NumberId
LIMIT 1";

        public const string InsertSupplier = @"
INSERT INTO suppliers
  (company_id, name, id_type, number_id, email, phone, address, notes, created_by, updated_by, active)
VALUES
  (@CompanyId, @Name, @IdType, @NumberId, @Email, @Phone, @Address, @Notes, @CreatedBy, @UpdatedBy, @ActiveMySQL)";

        public const string UpdateSupplier = @"
UPDATE suppliers SET
  name = @Name,
  id_type = @IdType,
  number_id = @NumberId,
  email = @Email,
  phone = @Phone,
  address = @Address,
  notes = @Notes,
  updated_by = @UpdatedBy,
  active = @ActiveMySQL
WHERE supplier_id = @Id AND company_id = @CompanyId";

        public const string DeactivateSupplier = @"
UPDATE suppliers SET active = 0, updated_by = @UpdatedBy WHERE supplier_id = @Id";
    }
}
