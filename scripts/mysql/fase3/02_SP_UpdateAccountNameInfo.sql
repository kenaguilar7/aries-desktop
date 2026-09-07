-- Apply only on a copy of `aries`, never on RDS production.
-- Mirrors CuentaDao.UpdateNameInfo (INSERT IGNORE accounts_names + update name/detail).

DROP PROCEDURE IF EXISTS `SP_UpdateAccountNameInfo`;

DELIMITER ;;
CREATE PROCEDURE `SP_UpdateAccountNameInfo`(
    IN Id INT,
    IN Name VARCHAR(50),
    IN Memo VARCHAR(50),
    IN CompanyId VARCHAR(5),
    IN UpdatedBy INT
)
BEGIN
    INSERT IGNORE INTO `accounts_names`(`name`) VALUES(Name);

    UPDATE `accounts`
    SET `account_name_id` = (SELECT `account_name_id` FROM `accounts_names` WHERE `name` = Name LIMIT 1),
        `detail` = Memo,
        `updated_by` = UpdatedBy
    WHERE `account_id` = Id AND `company_id` = CompanyId;
END ;;
DELIMITER ;
