-- Apply only on a copy of `aries`, never on RDS production.
-- Dump function: find or insert accounts_names and return account_name_id.

DROP FUNCTION IF EXISTS `GetAccountName`;

DELIMITER ;;
CREATE FUNCTION `GetAccountName`(AccountName VARCHAR(50)) RETURNS INT
    DETERMINISTIC
    MODIFIES SQL DATA
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
END ;;
DELIMITER ;
