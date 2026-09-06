-- Apply only on a copy of `aries`, never on RDS production.
-- Mirrors CapaDatos UsuarioDao.Insert. Password remains plaintext (hash is a later phase).
-- OUT Id matches MySqlDataAccess.SaveData<T, Q> which binds @Id.

DROP PROCEDURE IF EXISTS `SP_InsertUser`;

DELIMITER ;;
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
    IN Password VARCHAR(50),
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
END ;;
DELIMITER ;
