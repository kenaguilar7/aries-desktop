namespace AriesContador.Data.Query
{
    public static class PosAccountingQuery
    {
        public const string MapColumns = @"
T0.pos_account_map_id AS Id,
T0.company_id AS CompanyId,
T0.cash_account_id AS CashAccountId,
T0.card_account_id AS CardAccountId,
T0.transfer_account_id AS TransferAccountId,
T0.sales_account_id AS SalesAccountId,
T0.tax_account_id AS TaxAccountId,
T0.inventory_account_id AS InventoryAccountId,
T0.cogs_account_id AS CogsAccountId,
T0.cash_short_account_id AS CashShortAccountId,
T0.cash_over_account_id AS CashOverAccountId,
T0.tax_rate AS TaxRate,
T0.prices_include_tax AS PricesIncludeTax,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectMapByCompany = @"
SELECT " + MapColumns + @"
FROM pos_account_maps AS T0
WHERE T0.company_id = @CompanyId
LIMIT 1";

        public const string InsertMap = @"
INSERT INTO pos_account_maps
  (company_id, cash_account_id, card_account_id, transfer_account_id, sales_account_id, tax_account_id,
   inventory_account_id, cogs_account_id, cash_short_account_id, cash_over_account_id, tax_rate, prices_include_tax,
   created_by, updated_by, active)
VALUES
  (@CompanyId, @CashAccountId, @CardAccountId, @TransferAccountId, @SalesAccountId, @TaxAccountId,
   @InventoryAccountId, @CogsAccountId, @CashShortAccountId, @CashOverAccountId, @TaxRate, @PricesIncludeTax,
   @CreatedBy, @UpdatedBy, 1)";

        public const string UpdateMap = @"
UPDATE pos_account_maps SET
  cash_account_id = @CashAccountId,
  card_account_id = @CardAccountId,
  transfer_account_id = @TransferAccountId,
  sales_account_id = @SalesAccountId,
  tax_account_id = @TaxAccountId,
  inventory_account_id = @InventoryAccountId,
  cogs_account_id = @CogsAccountId,
  cash_short_account_id = @CashShortAccountId,
  cash_over_account_id = @CashOverAccountId,
  tax_rate = @TaxRate,
  prices_include_tax = @PricesIncludeTax,
  updated_by = @UpdatedBy,
  active = 1
WHERE company_id = @CompanyId";

        public const string PostingColumns = @"
T0.pos_session_posting_id AS Id,
T0.session_id AS SessionId,
T0.company_id AS CompanyId,
T0.journal_entry_id AS JournalEntryId,
T0.posted_at AS PostedAt,
T0.totals_hash AS TotalsHash,
T0.created_at AS CreatedAt,
T0.updated_at AS UpdateAt,
T0.created_by AS CreatedBy,
T0.updated_by AS UpdatedBy,
T0.active AS Active";

        public const string SelectPostingBySession = @"
SELECT " + PostingColumns + @"
FROM pos_session_postings AS T0
WHERE T0.session_id = @SessionId
LIMIT 1";

        public const string SelectPostingsByCompany = @"
SELECT " + PostingColumns + @"
FROM pos_session_postings AS T0
WHERE T0.company_id = @CompanyId AND T0.active = 1
ORDER BY T0.posted_at DESC";

        public const string InsertPosting = @"
INSERT INTO pos_session_postings
  (session_id, company_id, journal_entry_id, posted_at, totals_hash, created_by, updated_by, active)
VALUES
  (@SessionId, @CompanyId, @JournalEntryId, @PostedAt, @TotalsHash, @CreatedBy, @UpdatedBy, 1)";
    }
}
