-- Apply only on a copy of `aries`, never on RDS production.
-- Same tree walk as F_GetAccountPathForReport, separator `/` (journal-line SPs).

DROP FUNCTION IF EXISTS `GETFULLPATH`;

DELIMITER ;;
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
END ;;
DELIMITER ;
