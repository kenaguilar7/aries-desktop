-- Apply only on a copy of `aries`, never on RDS production.
-- Desktop quirk: unique name is checked only among ACTIVO (`account_type = 1`). Preserve it.

DROP PROCEDURE IF EXISTS `SP_AccountNameTaken`;

DELIMITER ;;
CREATE PROCEDURE `SP_AccountNameTaken`(
    IN AccountId INT,
    IN CompanyId VARCHAR(5),
    IN Name VARCHAR(50)
)
BEGIN
    SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS Taken
    FROM `accounts` AS ac
    JOIN `accounts_names` AS an ON ac.`account_name_id` = an.`account_name_id`
    WHERE ac.`account_id` <> AccountId
      AND ac.`company_id` = CompanyId
      AND ac.`account_type` = 1
      AND an.`name` = Name;
END ;;
DELIMITER ;
