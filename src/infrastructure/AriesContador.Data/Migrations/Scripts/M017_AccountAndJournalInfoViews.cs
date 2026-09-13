namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Recrea <c>account_info</c> y <c>accounting_entries_info</c> en la base
    /// de la conexión (DATABASE()), no en un schema fijo <c>aries</c>.
    /// El dump de Workbench trae <c>DEFINER=kenneth</c> y tablas calificadas
    /// con <c>aries.</c>; eso falla si la base se llama distinto.
    /// </summary>
    public sealed class M017_AccountAndJournalInfoViews : SqlMigration
    {
        public override int Version => 17;
        public override string Id => "017_AccountAndJournalInfoViews";
        public override string Description => "Vistas account_info y accounting_entries_info sin schema ni DEFINER";

        public override string Sql => @"
DROP VIEW IF EXISTS `account_info`;
-- BATCH
CREATE VIEW `account_info` AS
SELECT
    `T0`.`account_id` AS `account_id`,
    `T3`.`account_type` AS `account_type`,
    `T3`.`company_id` AS `company_id`,
    SUM(IF(((`T0`.`balance_type` + 0) = 1), `T0`.`balance`, 0)) AS `debito`,
    SUM(IF(((`T0`.`balance_type` + 0) = 2), `T0`.`balance`, 0)) AS `credito`,
    SUM(IF((((`T0`.`money_type` + 0) = 2) AND ((`T0`.`balance_type` + 0) = 1)),
        (`T0`.`balance` / `T0`.`money_chance`), 0)) AS `debito_USD`,
    SUM(IF((((`T0`.`money_type` + 0) = 2) AND ((`T0`.`balance_type` + 0) = 2)),
        (`T0`.`balance` / `T0`.`money_chance`), 0)) AS `credito_USD`,
    DATE_FORMAT(`T2`.`month_report`, '%Y%m') AS `month_report`,
    IF((SUM(IF(((`T1`.`status` + 0) = 1), 1, 0)) > 0), 0, 1) AS `cuadrado`
FROM `transactions_accounting` `T0`
LEFT JOIN `accounting_entries` `T1` ON (`T0`.`accounting_entry_id` = `T1`.`accounting_entry_id`)
LEFT JOIN `accounting_months` `T2` ON (`T1`.`accounting_months_id` = `T2`.`accounting_months_id`)
LEFT JOIN `accounts` `T3` ON (`T0`.`account_id` = `T3`.`account_id`)
WHERE (`T0`.`active` <> 0)
  AND (`T1`.`active` <> 0)
  AND (`T2`.`active` <> 0)
GROUP BY
    `T0`.`account_id`,
    `T3`.`account_type`,
    `T3`.`company_id`,
    DATE_FORMAT(`T2`.`month_report`, '%Y%m');
-- BATCH
DROP VIEW IF EXISTS `accounting_entries_info`;
-- BATCH
CREATE VIEW `accounting_entries_info` AS
SELECT
    `T2`.`company_id` AS `company_id`,
    `T2`.`month_report` AS `month_report`,
    `T1`.`entry_id` AS `entry_id`,
    `T4`.`name` AS `account_name`,
    `T0`.`transaction_accounting_id` AS `JournalEntryLineId`,
    `T0`.`reference` AS `reference`,
    `T0`.`detail` AS `detail`,
    `T0`.`bill_date` AS `bill_date`,
    IF(((`T0`.`balance_type` + 0) = 1), `T0`.`balance`, 0) AS `debit`,
    IF(((`T0`.`balance_type` + 0) = 2), `T0`.`balance`, 0) AS `credit`,
    `T0`.`money_type` AS `money_type`,
    `T0`.`money_chance` AS `money_chance`,
    `T0`.`foreign_amount` AS `balance_usd`
FROM `transactions_accounting` `T0`
LEFT JOIN `accounting_entries` `T1` ON (`T0`.`accounting_entry_id` = `T1`.`accounting_entry_id`)
LEFT JOIN `accounting_months` `T2` ON (`T1`.`accounting_months_id` = `T2`.`accounting_months_id`)
LEFT JOIN `accounts` `T3` ON (`T0`.`account_id` = `T3`.`account_id`)
LEFT JOIN `accounts_names` `T4` ON (`T3`.`account_name_id` = `T4`.`account_name_id`)
WHERE (`T0`.`active` <> 0)
  AND (`T1`.`active` <> 0)
  AND (`T2`.`active` <> 0);
";
    }
}
