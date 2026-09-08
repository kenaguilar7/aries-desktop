namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Recrea <c>accounting_entries_info</c> en la base de la conexión.
    /// El dump usa <c>aries.accounting_entries_info</c> y DEFINER kenneth.
    /// Sin calificar tablas y sin DEFINER. ORDER BY del dump se omite
    /// (MySQL 8 lo ignora en vistas sin LIMIT).
    /// </summary>
    public sealed class M012_AccountingEntriesInfoView : SqlMigration
    {
        public override int Version => 12;
        public override string Id => "012_AccountingEntriesInfoView";
        public override string Description => "Vista accounting_entries_info sin schema aries ni DEFINER";

        public override string Sql => @"
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
