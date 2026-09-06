-- Apply only on a copy of `aries`, never on RDS production.
-- Mirrors CapaDatos CompañiaDao.Update: does not change type_id or number_id.

DROP PROCEDURE IF EXISTS `SP_UpdateCompany`;

DELIMITER ;;
CREATE PROCEDURE `SP_UpdateCompany`(
    IN Code VARCHAR(5),
    IN CompanyName VARCHAR(50),
    IN MoneyType ENUM('Colones y Dolares', 'Colones', 'Dolares'),
    IN Op1 VARCHAR(50),
    IN Op2 VARCHAR(50),
    IN Address VARCHAR(100),
    IN Website VARCHAR(50),
    IN Mail VARCHAR(50),
    IN PhoneNumber1 VARCHAR(50),
    IN PhoneNumber2 VARCHAR(50),
    IN Notes VARCHAR(100),
    IN UserId INT(10),
    IN IsActive TINYINT(1)
)
BEGIN
    UPDATE `companies` SET
        `name` = CompanyName,
        `money_type` = MoneyType,
        `op1` = Op1,
        `op2` = Op2,
        `address` = Address,
        `website` = Website,
        `mail` = Mail,
        `phone_number1` = PhoneNumber1,
        `phone_number2` = PhoneNumber2,
        `notes` = Notes,
        `user_id` = UserId,
        `active` = IsActive,
        `updated_at` = NOW()
    WHERE `company_id` = Code;
END ;;
DELIMITER ;
