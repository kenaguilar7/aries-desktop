namespace AriesContador.Data.Query
{
    public static class PosQuery
    {
        public const string ProductColumns = @"
T0.product_id AS Id,
T0.company_id AS CompanyId,
T0.barcode AS Barcode,
T0.name AS Name,
T0.category AS Category,
T0.price AS Price,
T0.cost AS Cost,
T0.stock AS Stock,
T0.sold_by_weight AS SoldByWeight,
T0.price_per_kilo AS PricePerKilo,
T0.tax_exempt AS TaxExempt,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectProductById = @"
SELECT " + ProductColumns + @"
FROM products AS T0
WHERE T0.product_id = @Id
LIMIT 1";

        public const string SelectProductsByCompany = @"
SELECT " + ProductColumns + @"
FROM products AS T0
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.name";

        public const string SelectProductByBarcode = @"
SELECT " + ProductColumns + @"
FROM products AS T0
WHERE T0.company_id = @CompanyId AND T0.barcode = @Barcode AND T0.active = 1
LIMIT 1";

        public const string SelectLowStock = @"
SELECT " + ProductColumns + @"
FROM products AS T0
WHERE T0.company_id = @CompanyId AND T0.active = 1 AND T0.stock <= @Minimum
ORDER BY T0.stock, T0.name";

        public const string CountActiveProducts = @"
SELECT COUNT(*) FROM products WHERE company_id = @CompanyId AND active = 1";

        public const string InsertProduct = @"
INSERT INTO products
  (company_id, barcode, name, category, price, cost, stock, sold_by_weight, price_per_kilo, tax_exempt, created_by, updated_by, active)
VALUES
  (@CompanyId, @Barcode, @Name, @Category, @Price, @Cost, @Stock, @SoldByWeight, @PricePerKilo, @TaxExempt, @CreatedBy, @UpdatedBy, @ActiveMySQL)";

        public const string UpdateProduct = @"
UPDATE products SET
  barcode = @Barcode,
  name = @Name,
  category = @Category,
  price = @Price,
  cost = @Cost,
  stock = @Stock,
  sold_by_weight = @SoldByWeight,
  price_per_kilo = @PricePerKilo,
  tax_exempt = @TaxExempt,
  updated_by = @UpdatedBy,
  active = @ActiveMySQL
WHERE product_id = @Id AND company_id = @CompanyId";

        public const string DeactivateProduct = @"
UPDATE products SET active = 0, updated_by = @UpdatedBy WHERE product_id = @Id";

        public const string RegisterColumns = @"
T0.sales_register_id AS Id,
T0.company_id AS CompanyId,
T0.code AS Code,
T0.name AS Name,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectRegisterById = @"
SELECT " + RegisterColumns + @"
FROM sales_registers AS T0
WHERE T0.sales_register_id = @Id
LIMIT 1";

        public const string SelectRegistersByCompany = @"
SELECT " + RegisterColumns + @"
FROM sales_registers AS T0
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.code, T0.name";

        public const string InsertRegister = @"
INSERT INTO sales_registers
  (company_id, code, name, created_by, updated_by, active)
VALUES
  (@CompanyId, @Code, @Name, @CreatedBy, @UpdatedBy, @ActiveMySQL)";

        public const string UpdateRegister = @"
UPDATE sales_registers SET
  code = @Code,
  name = @Name,
  updated_by = @UpdatedBy,
  active = @ActiveMySQL
WHERE sales_register_id = @Id AND company_id = @CompanyId";

        public const string DeactivateRegister = @"
UPDATE sales_registers SET active = 0, updated_by = @UpdatedBy WHERE sales_register_id = @Id";

        public const string SessionColumns = @"
T0.session_id AS Id,
T0.sales_register_id AS SalesRegisterId,
T0.company_id AS CompanyId,
T0.opened_at AS OpenedAt,
T0.closed_at AS ClosedAt,
T0.opening_amount AS OpeningAmount,
T0.opening_notes AS OpeningNotes,
T0.cash_sales AS CashSales,
T0.card_sales AS CardSales,
T0.transfer_sales AS TransferSales,
T0.expected_closing_amount AS ExpectedClosingAmount,
T0.declared_closing_amount AS DeclaredClosingAmount,
T0.difference AS Difference,
T0.closing_notes AS ClosingNotes,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active,
T1.code AS RegisterCode,
T1.name AS RegisterName";

        public const string SelectOpenSession = @"
SELECT " + SessionColumns + @"
FROM sales_register_sessions AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.sales_register_id = @RegisterId AND T0.closed_at IS NULL
LIMIT 1";

        public const string SelectSessionById = @"
SELECT " + SessionColumns + @"
FROM sales_register_sessions AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.session_id = @Id
LIMIT 1";

        public const string SelectSessionHistory = @"
SELECT " + SessionColumns + @"
FROM sales_register_sessions AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.company_id = @CompanyId
ORDER BY T0.opened_at DESC";

        public const string InsertSession = @"
INSERT INTO sales_register_sessions
  (sales_register_id, company_id, opened_at, opening_amount, opening_notes,
   cash_sales, card_sales, transfer_sales, expected_closing_amount, created_by, updated_by, active)
VALUES
  (@SalesRegisterId, @CompanyId, @OpenedAt, @OpeningAmount, @OpeningNotes,
   0, 0, 0, @ExpectedClosingAmount, @CreatedBy, @UpdatedBy, 1)";

        public const string CloseSession = @"
UPDATE sales_register_sessions SET
  closed_at = @ClosedAt,
  expected_closing_amount = @ExpectedClosingAmount,
  declared_closing_amount = @DeclaredClosingAmount,
  difference = @Difference,
  closing_notes = @ClosingNotes,
  updated_by = @UpdatedBy
WHERE session_id = @Id AND closed_at IS NULL";

        public const string UpdateSessionTotals = @"
UPDATE sales_register_sessions SET
  cash_sales = @CashSales,
  card_sales = @CardSales,
  transfer_sales = @TransferSales,
  expected_closing_amount = @ExpectedClosingAmount,
  updated_by = @UpdatedBy
WHERE session_id = @Id AND closed_at IS NULL";

        public const string SaleColumns = @"
T0.sale_id AS Id,
T0.company_id AS CompanyId,
T0.sales_register_id AS SalesRegisterId,
T0.session_id AS SessionId,
CASE T0.payment_method
  WHEN 'tarjeta' THEN 2
  WHEN 'transferencia' THEN 3
  ELSE 1
END AS PaymentMethod,
T0.payment_reference AS PaymentReference,
T0.total AS Total,
T0.net_amount AS NetAmount,
T0.tax_amount AS TaxAmount,
T0.cost_amount AS CostAmount,
T0.sold_at AS SoldAt,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active,
T1.code AS RegisterCode,
T1.name AS RegisterName";

        public const string SelectSaleById = @"
SELECT " + SaleColumns + @"
FROM sales AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.sale_id = @Id
LIMIT 1";

        public const string SelectSalesByCompany = @"
SELECT " + SaleColumns + @"
FROM sales AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.sold_at DESC";

        public const string SelectSalesBySession = @"
SELECT " + SaleColumns + @"
FROM sales AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.session_id = @SessionId AND T0.active = 1
ORDER BY T0.sold_at DESC";

        public const string SelectSalesByDateRange = @"
SELECT " + SaleColumns + @"
FROM sales AS T0
INNER JOIN sales_registers AS T1 ON T1.sales_register_id = T0.sales_register_id
WHERE T0.company_id = @CompanyId AND T0.active = 1
  AND T0.sold_at >= @FromInclusive AND T0.sold_at < @ToExclusive
ORDER BY T0.sold_at DESC";

        public const string SelectSaleLines = @"
SELECT
  sale_line_id AS Id,
  sale_id AS SaleId,
  product_id AS ProductId,
  product_name AS ProductName,
  quantity AS Quantity,
  unit_price AS UnitPrice,
  sold_by_weight AS SoldByWeight,
  price_per_kilo AS PricePerKilo,
  weight_grams AS WeightGrams,
  line_total AS LineTotal,
  net_amount AS NetAmount,
  tax_amount AS TaxAmount,
  cost_amount AS CostAmount,
  tax_exempt AS TaxExempt,
  created_at AS CreatedAt,
  updated_at AS UpdateAt,
  created_by AS CreatedBy,
  updated_by AS UpdatedBy,
  active AS Active
FROM sale_lines
WHERE sale_id = @SaleId";

        public const string InsertSale = @"
INSERT INTO sales
  (company_id, sales_register_id, session_id, payment_method, payment_reference, total, net_amount, tax_amount, cost_amount, sold_at, created_by, updated_by, active)
VALUES
  (@CompanyId, @SalesRegisterId, @SessionId, @PaymentMethodDb, @PaymentReference, @Total, @NetAmount, @TaxAmount, @CostAmount, @SoldAt, @CreatedBy, @UpdatedBy, 1)";

        public const string InsertSaleLine = @"
INSERT INTO sale_lines
  (sale_id, product_id, product_name, quantity, unit_price, sold_by_weight, price_per_kilo, weight_grams, line_total, net_amount, tax_amount, cost_amount, tax_exempt, created_by, updated_by, active)
VALUES
  (@SaleId, @ProductId, @ProductName, @Quantity, @UnitPrice, @SoldByWeight, @PricePerKilo, @WeightGrams, @LineTotal, @NetAmount, @TaxAmount, @CostAmount, @TaxExempt, @CreatedBy, @UpdatedBy, 1)";

        public const string DecrementStock = @"
UPDATE products SET stock = stock - @Quantity, updated_by = @UpdatedBy
WHERE product_id = @ProductId AND company_id = @CompanyId AND active = 1 AND stock >= @Quantity";

        public const string LockOpenSession = @"
SELECT session_id AS Id
FROM sales_register_sessions
WHERE sales_register_id = @SalesRegisterId AND closed_at IS NULL
FOR UPDATE";
    }
}
