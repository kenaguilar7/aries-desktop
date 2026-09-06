-- Apply only on a copy of `aries`, never on RDS production.
-- Mirrors CapaDatos UsuarioDao.Update. Password remains plaintext (hash is a later phase).

DROP PROCEDURE IF EXISTS `SP_UpdateUser`;

DELIMITER ;;
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
    IN Password VARCHAR(50),
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
END ;;
DELIMITER ;
