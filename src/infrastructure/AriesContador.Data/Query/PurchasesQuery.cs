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

        public const string PurchaseColumns = @"
T0.purchase_id AS Id,
T0.company_id AS CompanyId,
T0.supplier_id AS SupplierId,
T0.document_number AS DocumentNumber,
T0.purchased_at AS PurchasedAt,
T0.payment_method AS PaymentMethodDb,
T0.payment_reference AS PaymentReference,
T0.total AS Total,
T0.net_amount AS NetAmount,
T0.tax_amount AS TaxAmount,
T0.notes AS Notes,
T0.status AS StatusDb,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectPurchaseById = @"
SELECT " + PurchaseColumns + @",
  S.name AS SupplierName
FROM purchases AS T0
LEFT JOIN suppliers AS S ON S.supplier_id = T0.supplier_id
WHERE T0.purchase_id = @Id
LIMIT 1";

        public const string SelectPurchasesByCompany = @"
SELECT " + PurchaseColumns + @",
  S.name AS SupplierName
FROM purchases AS T0
LEFT JOIN suppliers AS S ON S.supplier_id = T0.supplier_id
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.purchased_at DESC, T0.purchase_id DESC";

        public const string SelectPurchaseByDocument = @"
SELECT " + PurchaseColumns + @"
FROM purchases AS T0
WHERE T0.company_id = @CompanyId
  AND T0.supplier_id = @SupplierId
  AND T0.document_number = @DocumentNumber
LIMIT 1";

        public const string SelectPurchaseLines = @"
SELECT
  purchase_line_id AS Id,
  purchase_id AS PurchaseId,
  product_id AS ProductId,
  product_name AS ProductName,
  quantity AS Quantity,
  unit_price AS UnitPrice,
  line_total AS LineTotal,
  net_amount AS NetAmount,
  tax_amount AS TaxAmount,
  tax_exempt AS TaxExempt,
  cost_amount AS CostAmount,
  created_at AS CreatedAt,
  updated_at AS UpdateAt,
  created_by AS CreatedBy,
  updated_by AS UpdatedBy,
  active AS Active
FROM purchase_lines
WHERE purchase_id = @PurchaseId";

        public const string InsertPurchase = @"
INSERT INTO purchases
  (company_id, supplier_id, document_number, purchased_at, payment_method, payment_reference,
   total, net_amount, tax_amount, notes, status, created_by, updated_by, active)
VALUES
  (@CompanyId, @SupplierId, @DocumentNumber, @PurchasedAt, @PaymentMethodDb, @PaymentReference,
   @Total, @NetAmount, @TaxAmount, @Notes, @StatusDb, @CreatedBy, @UpdatedBy, 1)";

        public const string InsertPurchaseLine = @"
INSERT INTO purchase_lines
  (purchase_id, product_id, product_name, quantity, unit_price, line_total,
   net_amount, tax_amount, tax_exempt, cost_amount, created_by, updated_by, active)
VALUES
  (@PurchaseId, @ProductId, @ProductName, @Quantity, @UnitPrice, @LineTotal,
   @NetAmount, @TaxAmount, @TaxExempt, @CostAmount, @CreatedBy, @UpdatedBy, 1)";

        public const string IncrementStockAndCost = @"
UPDATE products SET
  stock = stock + @Quantity,
  cost = @Cost,
  updated_by = @UpdatedBy
WHERE product_id = @ProductId AND company_id = @CompanyId AND active = 1";
    }
}
