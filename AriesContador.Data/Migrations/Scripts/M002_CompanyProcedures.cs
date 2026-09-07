namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M002_CompanyProcedures : SqlMigration
    {
        public override int Version => 2;
        public override string Id => "002_CompanyProcedures";
        public override string Description => "SP_InsertCompany (OUT = Code) y SP_UpdateCompany";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_InsertCompany`;
-- BATCH
CREATE PROCEDURE `SP_InsertCompany`(
    IN Code VARCHAR(5),
    IN TypeId INT(10),
    IN NumberId VARCHAR(20),
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
    IN IsActive TINYINT(1),
    OUT NewCompanyId VARCHAR(5)
)
BEGIN
    INSERT INTO `companies` (
        `company_id`, `type_id`, `number_id`, `name`,
        `money_type`, `op1`, `op2`, `address`,
        `website`, `mail`, `phone_number1`, `phone_number2`,
        `notes`, `user_id`, `created_at`, `updated_at`, `active`
    )
    VALUES (
        Code, TypeId, NumberId, CompanyName,
        MoneyType, Op1, Op2, Address,
        Website, Mail, PhoneNumber1, PhoneNumber2,
        Notes, UserId, NOW(), NOW(), IsActive
    );

    SET NewCompanyId = Code;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_UpdateCompany`;
-- BATCH
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
END
";
    }
}
