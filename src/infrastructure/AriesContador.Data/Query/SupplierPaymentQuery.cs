namespace AriesContador.Data.Query
{
    public static class SupplierPaymentQuery
    {
        public const string Columns = @"
T0.supplier_payment_id AS Id,
T0.company_id AS CompanyId,
T0.supplier_id AS SupplierId,
T0.purchase_id AS PurchaseId,
T0.amount AS Amount,
T0.paid_at AS PaidAt,
T0.payment_method AS PaymentMethodDb,
T0.payment_reference AS PaymentReference,
T0.journal_entry_id AS JournalEntryId,
T0.notes AS Notes,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectById = @"
SELECT " + Columns + @",
  S.name AS SupplierName,
  P.document_number AS DocumentNumber
FROM supplier_payments AS T0
LEFT JOIN suppliers AS S ON S.supplier_id = T0.supplier_id
LEFT JOIN purchases AS P ON P.purchase_id = T0.purchase_id
WHERE T0.supplier_payment_id = @Id
LIMIT 1";

        public const string SelectByPurchase = @"
SELECT " + Columns + @",
  S.name AS SupplierName,
  P.document_number AS DocumentNumber
FROM supplier_payments AS T0
LEFT JOIN suppliers AS S ON S.supplier_id = T0.supplier_id
LEFT JOIN purchases AS P ON P.purchase_id = T0.purchase_id
WHERE T0.purchase_id = @PurchaseId
LIMIT 1";

        public const string SelectByCompany = @"
SELECT " + Columns + @",
  S.name AS SupplierName,
  P.document_number AS DocumentNumber
FROM supplier_payments AS T0
LEFT JOIN suppliers AS S ON S.supplier_id = T0.supplier_id
LEFT JOIN purchases AS P ON P.purchase_id = T0.purchase_id
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.paid_at DESC, T0.supplier_payment_id DESC";

        public const string Insert = @"
INSERT INTO supplier_payments
  (company_id, supplier_id, purchase_id, amount, paid_at, payment_method, payment_reference,
   journal_entry_id, notes, created_by, updated_by, active)
VALUES
  (@CompanyId, @SupplierId, @PurchaseId, @Amount, @PaidAt, @PaymentMethodDb, @PaymentReference,
   @JournalEntryId, @Notes, @CreatedBy, @UpdatedBy, 1)";
    }
}
