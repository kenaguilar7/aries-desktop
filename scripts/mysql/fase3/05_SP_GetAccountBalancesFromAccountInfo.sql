-- Apply only on a copy of `aries`, never on RDS production.
-- Maestro de cuentas panel A/B: same view and YYYYMM bounds as CuentaDao.CuentaConSaldos.
-- FromPeriod/ToPeriod are concatenated year+month (e.g. 202401). Do not swap for SP_AuxiliaryAccountsWithBalanceByDateRange.

DROP PROCEDURE IF EXISTS `SP_GetAccountBalancesFromAccountInfo`;

DELIMITER ;;
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
END ;;
DELIMITER ;
