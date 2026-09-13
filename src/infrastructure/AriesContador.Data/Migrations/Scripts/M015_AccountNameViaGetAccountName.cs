namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// <c>WHERE name = Name</c> en MySQL resuelve a la columna, no al parámetro:
    /// la cuenta nueva quedaba con el primer nombre del catálogo (ACTIVO).
    /// Secuencia correcta: GetAccountName primero, luego el INSERT de accounts.
    /// </summary>
    public sealed class M015_AccountNameViaGetAccountName : SqlMigration
    {
        public override int Version => 15;
        public override string Id => "015_AccountNameViaGetAccountName";
        public override string Description => "Insert/update de cuenta usa GetAccountName para no chocar con la columna name";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_InsertChildAccount`;
-- BATCH
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
    INSERT INTO `accounts` (
        `account_name_id`, `previous_balance_c`, `previous_balance_d`,
        `father_account`, `company_id`, `account_type`, `account_guide`,
        `detail`, `editable`, `updated_by`, `active`, `created_at`, `updated_at`
    )
    VALUES (
        GetAccountName(Name),
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
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_UpdateAccountNameInfo`;
-- BATCH
CREATE PROCEDURE `SP_UpdateAccountNameInfo`(
    IN Id INT,
    IN Name VARCHAR(50),
    IN Memo VARCHAR(50),
    IN CompanyId VARCHAR(5),
    IN UpdatedBy INT
)
BEGIN
    UPDATE `accounts`
    SET `account_name_id` = GetAccountName(Name),
        `detail` = Memo,
        `updated_by` = UpdatedBy
    WHERE `account_id` = Id AND `company_id` = CompanyId;
END
";
    }
}
