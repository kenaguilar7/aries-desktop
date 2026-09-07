-- Apply only on a copy of `aries`, never on RDS production.
-- Same predicate as CuentaDao.Deleted (active lines in open, active months).

DROP PROCEDURE IF EXISTS `SP_AccountHasOpenPeriodMovements`;

DELIMITER ;;
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
END ;;
DELIMITER ;
