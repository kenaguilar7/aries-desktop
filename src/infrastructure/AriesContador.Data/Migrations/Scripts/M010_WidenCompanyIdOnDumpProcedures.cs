namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M010_WidenCompanyIdOnDumpProcedures : SqlMigration
    {
        public override int Version => 10;
        public override string Id => "010_WidenCompanyIdOnDumpProcedures";
        public override string Description => "SPs del dump: CompanyId VARCHAR(5) alineado a las tablas";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_InsertAccount`;
-- BATCH
CREATE PROCEDURE `SP_InsertAccount`(
    IN Name INT,
    IN FatherAccount INT,
    IN CompanyId VARCHAR(5),
    IN AccountType INT(1),
    IN AccountTag INT(1),
    IN Editable TINYINT(1),
    IN Memo VARCHAR(50),
    IN UpdatedBy INT,
    OUT Id INT
)
BEGIN
    INSERT INTO `accounts` (
        `account_name_id`, `father_account`, `company_id`, `account_type`,
        `account_guide`, `editable`, `detail`,
        `created_at`, `updated_at`, `updated_by`
    )
    VALUES (
        Name, FatherAccount, CompanyId, AccountTag,
        AccountType, Editable, Memo,
        NOW(), NOW(), UpdatedBy
    );

    SET Id = LAST_INSERT_ID();
END
-- BATCH
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
            WHEN T0.`account_type`+ 0 = 4 THEN 'CostoVenta'
            WHEN T0.`account_type`+ 0 = 5 THEN 'Ingreso'
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
DROP PROCEDURE IF EXISTS `SP_GetAllPostingPeriod`;
-- BATCH
CREATE PROCEDURE `SP_GetAllPostingPeriod`(IN CompanyId VARCHAR(5))
BEGIN
    SELECT
        T0.`accounting_months_id` AS 'Id',
        T0.`month_report` AS 'Date',
        YEAR(`month_report`) AS 'Year',
        MONTH(`month_report`) AS 'Month',
        T0.`closed` AS 'Closed',
        T0.`company_id` AS 'CompanyId',
        T0.`created_at` AS 'CreatedAt',
        T0.`updated_at` AS 'UpdateAt',
        T0.`updated_by` AS 'UpdatedBy',
        T0.`active` AS 'Active'
    FROM `accounting_months` T0
    WHERE T0.`active` = true AND T0.`company_id` = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetClosingPostingPeriodReport`;
-- BATCH
CREATE PROCEDURE `SP_GetClosingPostingPeriodReport`(IN CompanyId VARCHAR(5))
BEGIN
    SELECT
        T0.Id,
        T0.company_id AS 'CompanyId',
        T0.from_period_id AS 'FromPeriodId',
        T0.to_period_id AS 'ToPeriodId',
        T0.from_period AS 'FromPeriod',
        T0.to_period AS 'ToPeriod',
        T0.amount AS 'Amount',
        T0.user_notes AS 'UserNotes',
        T1.user_name AS 'CreatedBy',
        T0.created_at AS 'CreatedAt',
        T0.updated_at AS 'UpdateAt'
    FROM posting_period_end_closing T0
    LEFT OUTER JOIN users T1 ON T0.updated_by = T1.user_id
    WHERE T0.company_id = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetPostingPeriodReport`;
-- BATCH
CREATE PROCEDURE `SP_GetPostingPeriodReport`(IN CompanyId VARCHAR(5))
BEGIN
    SET lc_time_names = 'es_ES';

    SELECT DATE_FORMAT(ac.month_report,'%M %Y') AS 'PostingPeriodDateString',
           IF(ac.closed, 'Cerrado','Abierto') AS 'Status',
           DATE_FORMAT(ac.created_at,'%d %M %Y') AS 'CreatedDateString',
           IF(ac.closed, DATE_FORMAT(ac.updated_at,'%d %M %Y'), '') AS 'ClosedDateString',
           (SELECT user_name FROM users us WHERE us.user_id = ac.updated_by LIMIT 1) AS 'UserName'
    FROM accounting_months ac
    WHERE ac.company_id = CompanyId AND ac.active = 1
    ORDER BY ac.month_report DESC;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_InsertClosingPostingPeriod`;
-- BATCH
CREATE PROCEDURE `SP_InsertClosingPostingPeriod`(
    IN CompanyId VARCHAR(5),
    IN FromPeriodId INT(10),
    IN ToPeriodId INT(10),
    IN FromPeriod VARCHAR(50),
    IN ToPeriod VARCHAR(50),
    IN Amount DOUBLE(18,2),
    IN UserNotes VARCHAR(100),
    IN UpdatedBy INT(10),
    OUT Id INT(10)
)
BEGIN
    INSERT INTO posting_period_end_closing (company_id, from_period_id, to_period_id, from_period, to_period, amount, user_notes, updated_by)
    VALUES (companyId, fromPeriodId, toPeriodId, fromPeriod, toPeriod, amount, userNotes, updatedBy);

    SET Id = (SELECT LAST_INSERT_ID());
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_InsertPostingPeriod`;
-- BATCH
CREATE PROCEDURE `SP_InsertPostingPeriod`(
    IN Date DATE,
    IN ClosedMySQL INT(1),
    IN CompanyId VARCHAR(5),
    IN UpdatedBy INT(10),
    OUT Id INT(10)
)
BEGIN
    INSERT INTO `accounting_months`(`month_report`,`closed`,`company_id`,`updated_by`)
    VALUES (Date, ClosedMySQL, CompanyId, UpdatedBy);

    SET Id = (SELECT LAST_INSERT_ID());
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_AccountHasMovements`;
-- BATCH
CREATE PROCEDURE `SP_AccountHasMovements`(
    IN AccountId INT(10),
    IN CompanyId VARCHAR(5)
)
BEGIN
    SELECT IF(COUNT(*) > 0, 1, 0)
    FROM accounting_entries T2
    INNER JOIN transactions_accounting T0 ON T2.accounting_entry_id = T0.accounting_entry_id
    INNER JOIN accounts T1 ON T1.account_id = T0.account_id
    WHERE T0.active = 1 AND T2.active = 1 AND T1.account_id = AccountId AND T1.company_id = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_UpdatePartlyAccount`;
-- BATCH
CREATE PROCEDURE `SP_UpdatePartlyAccount`(
    IN Id INT(10),
    IN NameMySql VARCHAR(50),
    IN Memo VARCHAR(50),
    IN UpdatedBy INT(10),
    IN CompanyId VARCHAR(5)
)
BEGIN
    SET @accountNameId = 0;

    INSERT IGNORE INTO accounts_names(name) VALUE(NameMySql);

    SET @accountNameId = (SELECT account_name_id FROM accounts_names WHERE name = NameMySql LIMIT 1);

    UPDATE accounts
    SET account_name_id = @accountNameId,
        detail = Memo,
        updated_by = UpdatedBy
    WHERE account_id = Id
      AND company_id = CompanyId;
END
";
    }
}
