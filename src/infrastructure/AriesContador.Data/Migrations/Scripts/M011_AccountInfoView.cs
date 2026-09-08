namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Recrea <c>account_info</c> en la base de la conexión.
    /// El dump de Workbench usa <c>aries.account_info</c> y DEFINER kenneth;
    /// eso falla en <c>aries-test</c> u otra schema. Sin calificar tablas.
    /// GROUP BY alineado a DATE_FORMAT (MySQL 8 ONLY_FULL_GROUP_BY);
    /// el dump agrupa por YEAR()/MONTH() y rompe.
    /// </summary>
    public sealed class M011_AccountInfoView : SqlMigration
    {
        public override int Version => 11;
        public override string Id => "011_AccountInfoView";
        public override string Description => "Vista account_info sin schema aries ni DEFINER";

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
";
    }
}
