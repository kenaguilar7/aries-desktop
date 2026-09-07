-- Apply only on a copy of `aries`, never on RDS production.
-- Missing from local restore: dump `aries_routines.sql`.
-- Walks father_account and concatenates names with `¡` (reports / SP_GetAccountById).
-- Local Docker: MySQL 8 needs log_bin_trust_function_creators=1 (see docker-compose.yml).

DROP FUNCTION IF EXISTS `F_GetAccountPathForReport`;

DELIMITER ;;
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
END ;;
DELIMITER ;
