namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Dump SP_DesactivateAccount is not in the migration catalog. Pin the Dapper
    /// contract to Id + UpdatedBy so WinForms no longer sends the full Account graph.
    /// </summary>
    public sealed class M022_DesactivateAccountProcedure : SqlMigration
    {
        public override int Version => 22;
        public override string Id => "022_DesactivateAccountProcedure";
        public override string Description => "SP_DesactivateAccount: Id + UpdatedBy";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_DesactivateAccount`;
-- BATCH
CREATE PROCEDURE `SP_DesactivateAccount`(
    IN Id INT,
    IN UpdatedBy INT
)
BEGIN
    UPDATE `accounts`
    SET `active` = 0,
        `updated_by` = COALESCE(UpdatedBy, `updated_by`),
        `updated_at` = NOW()
    WHERE `account_id` = Id;
END
";
    }
}
