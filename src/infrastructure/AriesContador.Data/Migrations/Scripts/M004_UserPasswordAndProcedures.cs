namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M004_UserPasswordAndProcedures : SqlMigration
    {
        public override int Version => 4;
        public override string Id => "004_UserPasswordAndProcedures";
        public override string Description => "users.password VARCHAR(255) y SP_InsertUser / SP_UpdateUser";

        public override string Sql => @"
ALTER TABLE `users`
    MODIFY COLUMN `password` VARCHAR(255) NOT NULL;
-- BATCH
DROP PROCEDURE IF EXISTS `SP_InsertUser`;
-- BATCH
CREATE PROCEDURE `SP_InsertUser`(
    IN UserName VARCHAR(20),
    IN UserType INT,
    IN IdNumber VARCHAR(20),
    IN Name VARCHAR(50),
    IN LastName VARCHAR(50),
    IN MiddleName VARCHAR(50),
    IN PhoneNumber VARCHAR(50),
    IN Mail VARCHAR(50),
    IN Memo VARCHAR(100),
    IN Password VARCHAR(255),
    IN UpdatedBy INT,
    IN Active TINYINT(1),
    OUT Id INT
)
BEGIN
    INSERT INTO `users` (
        `user_name`, `user_type`, `number_id`, `name`,
        `lastname_p`, `lastname_m`, `phone_number`, `mail`,
        `notes`, `password`, `updated_by`, `active`,
        `created_at`, `updated_at`
    )
    VALUES (
        UserName, UserType, IdNumber, Name,
        LastName, MiddleName, PhoneNumber, Mail,
        Memo, Password, UpdatedBy, Active,
        NOW(), NOW()
    );

    SET Id = LAST_INSERT_ID();
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_UpdateUser`;
-- BATCH
CREATE PROCEDURE `SP_UpdateUser`(
    IN Id INT,
    IN UserName VARCHAR(20),
    IN UserType INT,
    IN IdNumber VARCHAR(20),
    IN Name VARCHAR(50),
    IN LastName VARCHAR(50),
    IN MiddleName VARCHAR(50),
    IN PhoneNumber VARCHAR(50),
    IN Mail VARCHAR(50),
    IN Memo VARCHAR(100),
    IN Password VARCHAR(255),
    IN UpdatedBy INT,
    IN Active TINYINT(1)
)
BEGIN
    UPDATE `users` SET
        `user_name` = UserName,
        `user_type` = UserType,
        `number_id` = IdNumber,
        `name` = Name,
        `lastname_p` = LastName,
        `lastname_m` = MiddleName,
        `phone_number` = PhoneNumber,
        `mail` = Mail,
        `notes` = Memo,
        `password` = Password,
        `updated_by` = UpdatedBy,
        `active` = Active,
        `updated_at` = NOW()
    WHERE `user_id` = Id;
END
";
    }
}
