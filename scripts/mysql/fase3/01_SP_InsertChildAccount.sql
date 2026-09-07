-- Apply only on a copy of `aries`, never on RDS production.
-- Interactive create (FrameNuevaCuenta / CuentaDao.Insert).
-- Does NOT replace SP_InsertAccount (company chart copy).
-- AccountType → account_guide, AccountTag → account_type (same cross as SP_InsertAccount).

DROP PROCEDURE IF EXISTS `SP_InsertChildAccount`;

DELIMITER ;;
CREATE PROCEDURE `SP_InsertChildAccount`(
    IN Name VARCHAR(50),
    IN PriorBalance DECIMAL(12,2),
    IN PriorBalanceForeign DECIMAL(12,2),
    IN FatherAccount INT,
    IN CompanyId VARCHAR(5),
    IN AccountType INT,
    IN AccountTag INT,
    IN Memo VARCHAR(50),
    IN Editable TINYINT(1),
    IN UpdatedBy INT,
    OUT Id INT
)
BEGIN
    INSERT IGNORE INTO `accounts_names`(`name`) VALUES(Name);

    INSERT INTO `accounts` (
        `account_name_id`, `previous_balance_c`, `previous_balance_d`,
        `father_account`, `company_id`, `account_type`, `account_guide`,
        `detail`, `editable`, `updated_by`, `active`, `created_at`, `updated_at`
    )
    VALUES (
        (SELECT `account_name_id` FROM `accounts_names` WHERE `name` = Name LIMIT 1),
        PriorBalance,
        PriorBalanceForeign,
        NULLIF(FatherAccount, 0),
        CompanyId,
        AccountTag,
        AccountType,
        Memo,
        Editable,
        UpdatedBy,
        1,
        NOW(),
        NOW()
    );

    SET Id = LAST_INSERT_ID();

    IF FatherAccount IS NOT NULL AND FatherAccount <> 0
       AND (SELECT `account_guide` + 0 FROM `accounts` WHERE `account_id` = FatherAccount LIMIT 1) = 3 THEN
        UPDATE `transactions_accounting`
        SET `account_id` = Id
        WHERE `account_id` = FatherAccount;

        UPDATE `accounts`
        SET `account_guide` = 2
        WHERE `account_id` = FatherAccount
        LIMIT 1;
    END IF;
END ;;
DELIMITER ;
