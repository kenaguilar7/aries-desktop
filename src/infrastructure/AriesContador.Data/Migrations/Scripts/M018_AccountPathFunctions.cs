namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Recrea las funciones de ruta/nombre en la base de la conexión.
    /// El dump trae <c>DEFINER=kenneth</c>; eso no crea la rutina en
    /// <c>aries-backup-report-test</c> ni en otra schema. Sin DEFINER.
    /// </summary>
    public sealed class M018_AccountPathFunctions : SqlMigration
    {
        public override int Version => 18;
        public override string Id => "018_AccountPathFunctions";
        public override string Description => "Funciones F_GetAccountPathForReport, GETFULLPATH, GetAccountName sin DEFINER";

        public override string Sql => @"
DROP FUNCTION IF EXISTS `F_GetAccountPathForReport`;
-- BATCH
CREATE FUNCTION `F_GetAccountPathForReport`(accountid INT) RETURNS TEXT CHARSET utf8mb4
    READS SQL DATA
BEGIN
    DECLARE idbuscar INT DEFAULT accountid;
    DECLARE fullname TEXT;
    SET fullname = '';
    WHILE idbuscar <> 0 DO
        SET fullname = CONCAT(
            '¡',
            (SELECT T1.name
             FROM accounts T0
             LEFT JOIN accounts_names T1 ON T0.account_name_id = T1.account_name_id
             WHERE T0.account_id = idbuscar
             LIMIT 1),
            fullname);
        SET idbuscar = (SELECT IFNULL(father_account, 0) FROM accounts WHERE account_id = idbuscar LIMIT 1);
    END WHILE;
    RETURN fullname;
END
-- BATCH
DROP FUNCTION IF EXISTS `GetAccountName`;
-- BATCH
CREATE FUNCTION `GetAccountName`(AccountName VARCHAR(50)) RETURNS INT
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE AccountNameId INT;

    SELECT `account_name_id` INTO AccountNameId
    FROM `accounts_names`
    WHERE `name` = AccountName
    LIMIT 1;

    IF AccountNameId IS NULL THEN
        INSERT INTO `accounts_names` (`name`) VALUES (AccountName);
        SET AccountNameId = LAST_INSERT_ID();
    END IF;

    RETURN AccountNameId;
END
-- BATCH
DROP FUNCTION IF EXISTS `GETFULLPATH`;
-- BATCH
CREATE FUNCTION `GETFULLPATH`(accountid INT) RETURNS TEXT CHARSET utf8mb4
    READS SQL DATA
BEGIN
    DECLARE idbuscar INT DEFAULT accountid;
    DECLARE fullname TEXT;
    SET fullname = '';
    WHILE idbuscar <> 0 DO
        SET fullname = CONCAT(
            '/',
            (SELECT T1.name
             FROM accounts T0
             LEFT JOIN accounts_names T1 ON T0.account_name_id = T1.account_name_id
             WHERE T0.account_id = idbuscar
             LIMIT 1),
            fullname);
        SET idbuscar = (SELECT IFNULL(father_account, 0) FROM accounts WHERE account_id = idbuscar LIMIT 1);
    END WHILE;
    RETURN fullname;
END
";
    }
}
