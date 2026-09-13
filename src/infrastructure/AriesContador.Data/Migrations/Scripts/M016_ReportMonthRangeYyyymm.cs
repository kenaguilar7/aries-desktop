namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// <c>STR_TO_DATE('202012','%Y%m')</c> da NULL con <c>NO_ZERO_IN_DATE</c> (MySQL 8).
    /// <c>account_info.month_report</c> ya es <c>YYYYMM</c>; se compara directo.
    /// </summary>
    public sealed class M016_ReportMonthRangeYyyymm : SqlMigration
    {
        public override int Version => 16;
        public override string Id => "016_ReportMonthRangeYyyymm";
        public override string Description => "Filtro de meses YYYYMM sin STR_TO_DATE nulo";

        public override string Sql => @"
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
          AND T1.month_report BETWEEN FirstDate AND EndDate
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
          AND T1.month_report BETWEEN FirstDate AND EndDate
          AND T1.account_type IN ('INGRESO','EGRESO','COSTO VENTA')
        GROUP BY T1.account_id
    ) T1 USING (account_id)
    WHERE T0.company_id = CompanyId AND T0.account_type IN ('INGRESO','EGRESO','COSTO VENTA');
END
";
    }
}
