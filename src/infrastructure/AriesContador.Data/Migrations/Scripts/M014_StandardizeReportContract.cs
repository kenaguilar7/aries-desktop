namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Contrato de reportes 1.2.0. No reescribe M010 (ya aplicada).
    /// CASE 4/5 alineado a CuentaDao. Alias dobles DebOrCred/DebOCred y Rate/RateAmount
    /// para que 1.1.15 y 1.2.0 convivan en la misma RDS. Nombres de SP con typo se quedan.
    /// </summary>
    public sealed class M014_StandardizeReportContract : SqlMigration
    {
        public override int Version => 14;
        public override string Id => "014_StandardizeReportContract";
        public override string Description => "CASE Ingreso/CostoVenta y alias DebOrCred/DebOCred para reportes";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_GetAccountsByCompanyId`;
-- BATCH
CREATE PROCEDURE `SP_GetAccountsByCompanyId`(IN CompanyId VARCHAR(5))
BEGIN
    SELECT T0.`account_id` AS 'Id',
        T1.`name` AS 'Name',
        T0.`father_account` AS 'FatherAccount',
        T0.`previous_balance_c` AS 'PriorBalance',
        T0.`previous_balance_d` AS 'PriorBalanceForeign',
        T0.`company_id` AS 'CompanyId',
        CASE
            WHEN T0.`account_type`+ 0 = 1 THEN 'Activo'
            WHEN T0.`account_type`+ 0 = 2 THEN 'Pasivo'
            WHEN T0.`account_type`+ 0 = 3 THEN 'Patrimonio'
            WHEN T0.`account_type`+ 0 = 4 THEN 'Ingreso'
            WHEN T0.`account_type`+ 0 = 5 THEN 'CostoVenta'
            WHEN T0.`account_type`+ 0 = 6 THEN 'Egreso'
        END AS 'AccountTag',
        CASE
            WHEN T0.`account_guide` + 0 = 1 THEN 'Cuenta_Titulo'
            WHEN T0.`account_guide` + 0 = 2 THEN 'Cuenta_De_Mayor'
            WHEN T0.`account_guide` + 0 = 3 THEN 'Cuenta_Auxiliar'
        END AS 'AccountType',
        T0.`editable` AS 'Editable',
        T0.`detail` AS 'Memo',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdatedAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active'
    FROM `accounts` T0
    INNER JOIN `accounts_names` T1 USING(account_name_id)
    WHERE T0.`active` = true AND T0.`company_id` = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetAccountById`;
-- BATCH
CREATE PROCEDURE `SP_GetAccountById`(IN AccountId INT)
BEGIN
    SELECT T0.`account_id` AS 'Id',
        T1.`name` AS 'Name',
        T0.`father_account` AS 'FatherAccount',
        T0.`previous_balance_c` AS 'PriorBalance',
        T0.`previous_balance_d` AS 'PriorBalanceForeign',
        T0.`company_id` AS 'CompanyId',
        CASE
            WHEN T0.`account_type`+ 0 = 1 THEN 'Activo'
            WHEN T0.`account_type`+ 0 = 2 THEN 'Pasivo'
            WHEN T0.`account_type`+ 0 = 3 THEN 'Patrimonio'
            WHEN T0.`account_type`+ 0 = 4 THEN 'Ingreso'
            WHEN T0.`account_type`+ 0 = 5 THEN 'CostoVenta'
            WHEN T0.`account_type`+ 0 = 6 THEN 'Egreso'
        END AS 'AccountTag',
        CASE
            WHEN T0.`account_guide` + 0 = 1 THEN 'Cuenta_Titulo'
            WHEN T0.`account_guide` + 0 = 2 THEN 'Cuenta_De_Mayor'
            WHEN T0.`account_guide` + 0 = 3 THEN 'Cuenta_Auxiliar'
        END AS 'AccountType',
        T0.`editable` AS 'Editable',
        T0.`detail` AS 'Memo',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdatedAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active',
        GETFULLPATH(T0.`account_id`) AS 'PathDirection'
    FROM `accounts` T0
    LEFT JOIN `accounts_names` T1 USING(account_name_id)
    WHERE T0.`active` = true AND T0.`account_id` = AccountId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_AuxiliaryAccountsWithBalanceByDateRange`;
-- BATCH
CREATE PROCEDURE `SP_AuxiliaryAccountsWithBalanceByDateRange`(
    IN CompanyId VARCHAR(5),
    IN FirstDate VARCHAR(20),
    IN EndDate VARCHAR(20)
)
BEGIN
    SELECT
        T0.account_id AS 'Id',
        T2.name AS 'Name',
        F_GetAccountPathForReport(T0.account_id) AS 'PathDirection',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 'Activo'
            WHEN T0.`account_type` + 0 = 2 THEN 'Pasivo'
            WHEN T0.`account_type` + 0 = 3 THEN 'Patrimonio'
            WHEN T0.`account_type` + 0 = 4 THEN 'Ingreso'
            WHEN T0.`account_type` + 0 = 5 THEN 'CostoVenta'
            WHEN T0.`account_type` + 0 = 6 THEN 'Egreso'
        END AS 'AccountTag',
        CASE
            WHEN T0.`account_guide` + 0 = 1 THEN 'Cuenta_Titulo'
            WHEN T0.`account_guide` + 0 = 2 THEN 'Cuenta_De_Mayor'
            WHEN T0.`account_guide` + 0 = 3 THEN 'Cuenta_Auxiliar'
        END AS 'AccountType',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 1
            WHEN T0.`account_type` + 0 = 2 THEN 2
            WHEN T0.`account_type` + 0 = 3 THEN 2
            WHEN T0.`account_type` + 0 = 4 THEN 2
            WHEN T0.`account_type` + 0 = 5 THEN 1
            WHEN T0.`account_type` + 0 = 6 THEN 1
        END AS 'DebOrCred',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 1
            WHEN T0.`account_type` + 0 = 2 THEN 2
            WHEN T0.`account_type` + 0 = 3 THEN 2
            WHEN T0.`account_type` + 0 = 4 THEN 2
            WHEN T0.`account_type` + 0 = 5 THEN 1
            WHEN T0.`account_type` + 0 = 6 THEN 1
        END AS 'DebOCred',
        T0.father_account AS 'FatherAccount',
        COALESCE(T0.previous_balance_c, 0) AS 'PriorBalance',
        COALESCE(T0.previous_balance_d, 0) AS 'PriorBalanceForeign',
        COALESCE(T1.debito, 0) AS 'DebitBalance',
        COALESCE(T1.debito_USD, 0) AS 'DebitBalanceForeign',
        COALESCE(T1.credito, 0) AS 'CreditBalance',
        COALESCE(T1.credito_USD, 0) AS 'CreditBalanceForeign'
    FROM accounts T0
    INNER JOIN accounts_names T2 USING (account_name_id)
    LEFT OUTER JOIN (
        SELECT
            T1.account_id,
            SUM(T1.debito) AS 'debito',
            SUM(T1.credito) AS 'credito',
            SUM(T1.debito_USD) AS 'debito_USD',
            SUM(T1.credito_USD) AS 'credito_USD'
        FROM account_info T1
        WHERE T1.company_id = CompanyId
          AND (T1.month_report BETWEEN DATE_FORMAT(STR_TO_DATE(FirstDate, '%Y%m'), '%Y%m')
            AND DATE_FORMAT(STR_TO_DATE(EndDate, '%Y%m'), '%Y%m'))
        GROUP BY T1.account_id
    ) T1 USING (account_id)
    WHERE T0.company_id = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_EstadoResultadoIntegralReport`;
-- BATCH
CREATE PROCEDURE `SP_EstadoResultadoIntegralReport`(
    IN CompanyId VARCHAR(5),
    IN FirstDate VARCHAR(20),
    IN EndDate VARCHAR(20)
)
BEGIN
    SELECT
        T0.account_id AS 'Id',
        T2.name AS 'Name',
        F_GetAccountPathForReport(T0.account_id) AS 'PathDirection',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 'Activo'
            WHEN T0.`account_type` + 0 = 2 THEN 'Pasivo'
            WHEN T0.`account_type` + 0 = 3 THEN 'Patrimonio'
            WHEN T0.`account_type` + 0 = 4 THEN 'Ingreso'
            WHEN T0.`account_type` + 0 = 5 THEN 'CostoVenta'
            WHEN T0.`account_type` + 0 = 6 THEN 'Egreso'
        END AS 'AccountTag',
        CASE
            WHEN T0.`account_guide` + 0 = 1 THEN 'Cuenta_Titulo'
            WHEN T0.`account_guide` + 0 = 2 THEN 'Cuenta_De_Mayor'
            WHEN T0.`account_guide` + 0 = 3 THEN 'Cuenta_Auxiliar'
        END AS 'AccountType',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 'Debito'
            WHEN T0.`account_type` + 0 = 2 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 3 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 4 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 5 THEN 'Debito'
            WHEN T0.`account_type` + 0 = 6 THEN 'Debito'
        END AS 'DebOrCred',
        CASE
            WHEN T0.`account_type` + 0 = 1 THEN 'Debito'
            WHEN T0.`account_type` + 0 = 2 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 3 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 4 THEN 'Credito'
            WHEN T0.`account_type` + 0 = 5 THEN 'Debito'
            WHEN T0.`account_type` + 0 = 6 THEN 'Debito'
        END AS 'DebOCred',
        T0.editable AS 'EditableMySql',
        T0.father_account AS 'FatherAccount',
        COALESCE(T0.previous_balance_c, 0) AS 'PriorBalance',
        COALESCE(T0.previous_balance_d, 0) AS 'PriorBalanceForeign',
        COALESCE(T1.debito, 0) AS 'DebitBalance',
        COALESCE(T1.debito_USD, 0) AS 'DebitBalanceForeign',
        COALESCE(T1.credito, 0) AS 'CreditBalance',
        COALESCE(T1.credito_USD, 0) AS 'CreditBalanceForeign'
    FROM accounts T0
    INNER JOIN accounts_names T2 USING (account_name_id)
    LEFT OUTER JOIN (
        SELECT
            T1.account_id,
            SUM(T1.debito) AS 'debito',
            SUM(T1.credito) AS 'credito',
            SUM(T1.debito_USD) AS 'debito_USD',
            SUM(T1.credito_USD) AS 'credito_USD'
        FROM account_info T1
        WHERE T1.company_id = CompanyId
          AND (T1.month_report BETWEEN DATE_FORMAT(STR_TO_DATE(FirstDate, '%Y%m'), '%Y%m')
            AND DATE_FORMAT(STR_TO_DATE(EndDate, '%Y%m'), '%Y%m'))
          AND T1.account_type IN ('INGRESO','EGRESO','COSTO VENTA')
        GROUP BY T1.account_id
    ) T1 USING (account_id)
    WHERE T0.company_id = CompanyId AND T0.account_type IN ('INGRESO','EGRESO','COSTO VENTA');
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetJournalEntryLineById`;
-- BATCH
CREATE PROCEDURE `SP_GetJournalEntryLineById`(IN Id INT)
BEGIN
    SELECT
        T0.`transaction_accounting_id` AS 'Id',
        T0.`account_id` AS 'AccountId',
        T0.`accounting_entry_id` AS 'JournalEntryId',
        GETFULLPATH(T0.`account_id`) AS 'AccountPath',
        T0.`reference` AS 'Reference',
        T0.`detail` AS 'Memo',
        T0.`balance` AS 'Amount',
        T0.`foreign_amount` AS 'ForeignAmount',
        T0.`balance_type` AS 'DebOrCred',
        T0.`balance_type` AS 'DebOCred',
        T0.`money_type` AS 'Currency',
        T0.`money_chance` AS 'RateAmount',
        T0.`money_chance` AS 'Rate',
        T0.`bill_date` AS 'Date',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdateAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active'
    FROM `transactions_accounting` T0
    WHERE T0.`active` = true AND T0.`transaction_accounting_id` = Id;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetJournalEntryLineByJournalEntryId`;
-- BATCH
CREATE PROCEDURE `SP_GetJournalEntryLineByJournalEntryId`(IN JournalEntryId INT)
BEGIN
    SELECT
        T0.`transaction_accounting_id` AS 'Id',
        T0.`account_id` AS 'AccountId',
        T0.`accounting_entry_id` AS 'JournalEntryId',
        GETFULLPATH(T0.`account_id`) AS 'AccountPath',
        T3.name AS 'AccountName',
        T0.`reference` AS 'Reference',
        T0.`detail` AS 'Memo',
        T0.`balance` AS 'Amount',
        T0.`foreign_amount` AS 'ForeignAmount',
        T0.`balance_type` AS 'DebOrCred',
        T0.`balance_type` AS 'DebOCred',
        T0.`money_type` AS 'Currency',
        T0.`money_chance` AS 'RateAmount',
        T0.`money_chance` AS 'Rate',
        T0.`bill_date` AS 'Date',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdateAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active'
    FROM `transactions_accounting` T0
    INNER JOIN `accounts` T1 USING(account_id)
    LEFT JOIN `accounts_names` T3 USING(account_name_id)
    WHERE T0.`active` = true AND T0.`accounting_entry_id` = JournalEntryId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetAllJournalEntyLineByAccoudIdAndPostingPeriodId`;
-- BATCH
CREATE PROCEDURE `SP_GetAllJournalEntyLineByAccoudIdAndPostingPeriodId`(
    IN accountId INT,
    IN postingPeriodId INT
)
BEGIN
    SELECT
        T0.`transaction_accounting_id` AS 'Id',
        T0.`account_id` AS 'AccountId',
        T0.`accounting_entry_id` AS 'JournalEntryId',
        GETFULLPATH(T0.`account_id`) AS 'AccountPath',
        T0.`reference` AS 'Reference',
        T0.`detail` AS 'Memo',
        T0.`balance` AS 'Amount',
        T0.`foreign_amount` AS 'ForeignAmount',
        T0.`balance_type` AS 'DebOrCred',
        T0.`balance_type` AS 'DebOCred',
        T0.`money_type` AS 'Currency',
        T0.`money_chance` AS 'RateAmount',
        T0.`money_chance` AS 'Rate',
        T0.`bill_date` AS 'Date',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdateAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active'
    FROM `transactions_accounting` T0
    INNER JOIN `accounting_entries` T1 USING (accounting_entry_id)
    INNER JOIN `accounting_months` T2 USING(accounting_months_id)
    WHERE (T0.`active` = true AND T1.`active` = true)
      AND T0.`account_id` = accountId
      AND T2.`accounting_months_id` = postingPeriodId;
END
";
    }
}
