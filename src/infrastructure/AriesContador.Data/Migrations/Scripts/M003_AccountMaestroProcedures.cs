namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M003_AccountMaestroProcedures : SqlMigration
    {
        public override int Version => 3;
        public override string Id => "003_AccountMaestroProcedures";
        public override string Description => "SPs del maestro de cuentas y vista account_info";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_InsertChildAccount`;
-- BATCH
CREATE PROCEDURE `SP_InsertChildAccount`(
    IN Name VARCHAR(50),
    IN PriorBalance DECIMAL(12,2),
    IN PriorBalanceForeign DECIMAL(12,2),
    IN FatherAccount INT,
    IN CompanyId VARCHAR(5),
    IN AccountType INT,
    IN AccountTag INT,
    IN Memo VARCHAR(50),
    IN Editable TINYINT(1),
    IN UpdatedBy INT,
    OUT Id INT
)
BEGIN
    INSERT IGNORE INTO `accounts_names`(`name`) VALUES(Name);

    INSERT INTO `accounts` (
        `account_name_id`, `previous_balance_c`, `previous_balance_d`,
        `father_account`, `company_id`, `account_type`, `account_guide`,
        `detail`, `editable`, `updated_by`, `active`, `created_at`, `updated_at`
    )
    VALUES (
        (SELECT `account_name_id` FROM `accounts_names` WHERE `name` = Name LIMIT 1),
        PriorBalance,
        PriorBalanceForeign,
        NULLIF(FatherAccount, 0),
        CompanyId,
        AccountTag,
        AccountType,
        Memo,
        Editable,
        UpdatedBy,
        1,
        NOW(),
        NOW()
    );

    SET Id = LAST_INSERT_ID();

    IF FatherAccount IS NOT NULL AND FatherAccount <> 0
       AND (SELECT `account_guide` + 0 FROM `accounts` WHERE `account_id` = FatherAccount LIMIT 1) = 3 THEN
        UPDATE `transactions_accounting`
        SET `account_id` = Id
        WHERE `account_id` = FatherAccount;

        UPDATE `accounts`
        SET `account_guide` = 2
        WHERE `account_id` = FatherAccount
        LIMIT 1;
    END IF;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_UpdateAccountNameInfo`;
-- BATCH
CREATE PROCEDURE `SP_UpdateAccountNameInfo`(
    IN Id INT,
    IN Name VARCHAR(50),
    IN Memo VARCHAR(50),
    IN CompanyId VARCHAR(5),
    IN UpdatedBy INT
)
BEGIN
    INSERT IGNORE INTO `accounts_names`(`name`) VALUES(Name);

    UPDATE `accounts`
    SET `account_name_id` = (SELECT `account_name_id` FROM `accounts_names` WHERE `name` = Name LIMIT 1),
        `detail` = Memo,
        `updated_by` = UpdatedBy
    WHERE `account_id` = Id AND `company_id` = CompanyId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_AccountNameTaken`;
-- BATCH
CREATE PROCEDURE `SP_AccountNameTaken`(
    IN AccountId INT,
    IN CompanyId VARCHAR(5),
    IN Name VARCHAR(50)
)
BEGIN
    SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS Taken
    FROM `accounts` AS ac
    JOIN `accounts_names` AS an ON ac.`account_name_id` = an.`account_name_id`
    WHERE ac.`account_id` <> AccountId
      AND ac.`company_id` = CompanyId
      AND ac.`account_type` = 1
      AND an.`name` = Name;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_AccountHasOpenPeriodMovements`;
-- BATCH
CREATE PROCEDURE `SP_AccountHasOpenPeriodMovements`(
    IN AccountId INT
)
BEGIN
    SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS HasMovements
    FROM `transactions_accounting` T0
    LEFT JOIN `accounting_entries` T1 USING(`accounting_entry_id`)
    LEFT JOIN `accounting_months` T2 USING(`accounting_months_id`)
    WHERE T2.`active` = 1
      AND T2.`closed` = 0
      AND T1.`active` = 1
      AND T0.`active` = 1
      AND T0.`account_id` = AccountId;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_GetAccountBalancesFromAccountInfo`;
-- BATCH
CREATE PROCEDURE `SP_GetAccountBalancesFromAccountInfo`(
    IN CompanyId VARCHAR(5),
    IN FromPeriod CHAR(6),
    IN ToPeriod CHAR(6)
)
BEGIN
    SELECT
        `account_id` AS Id,
        SUM(`debito`) AS DebitBalance,
        SUM(`credito`) AS CreditBalance,
        SUM(`debito_USD`) AS DebitBalanceForeign,
        SUM(`credito_USD`) AS CreditBalanceForeign
    FROM `account_info`
    WHERE `company_id` = CompanyId
      AND `month_report` BETWEEN FromPeriod AND ToPeriod
    GROUP BY `account_id`;
END
-- BATCH
CREATE OR REPLACE VIEW `account_info` AS
SELECT
    `T0`.`account_id` AS `account_id`,
    `T3`.`account_type` AS `account_type`,
    `T3`.`company_id` AS `company_id`,
    SUM(IF((`T0`.`balance_type` + 0) = 1, `T0`.`balance`, 0)) AS `debito`,
    SUM(IF((`T0`.`balance_type` + 0) = 2, `T0`.`balance`, 0)) AS `credito`,
    SUM(IF(((`T0`.`money_type` + 0) = 2) AND ((`T0`.`balance_type` + 0) = 1), (`T0`.`balance` / `T0`.`money_chance`), 0)) AS `debito_USD`,
    SUM(IF(((`T0`.`money_type` + 0) = 2) AND ((`T0`.`balance_type` + 0) = 2), (`T0`.`balance` / `T0`.`money_chance`), 0)) AS `credito_USD`,
    DATE_FORMAT(`T2`.`month_report`, '%Y%m') AS `month_report`,
    IF(SUM(IF((`T1`.`status` + 0) = 1, 1, 0)) > 0, 0, 1) AS `cuadrado`
FROM `transactions_accounting` `T0`
LEFT JOIN `accounting_entries` `T1` ON `T0`.`accounting_entry_id` = `T1`.`accounting_entry_id`
LEFT JOIN `accounting_months` `T2` ON `T1`.`accounting_months_id` = `T2`.`accounting_months_id`
LEFT JOIN `accounts` `T3` ON `T0`.`account_id` = `T3`.`account_id`
WHERE `T0`.`active` <> 0
  AND `T1`.`active` <> 0
  AND `T2`.`active` <> 0
GROUP BY
    `T0`.`account_id`,
    `T3`.`account_type`,
    `T3`.`company_id`,
    DATE_FORMAT(`T2`.`month_report`, '%Y%m');
";
    }
}
