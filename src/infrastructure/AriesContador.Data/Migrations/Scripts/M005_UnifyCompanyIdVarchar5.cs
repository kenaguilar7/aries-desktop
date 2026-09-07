namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M005_UnifyCompanyIdVarchar5 : SqlMigration
    {
        public override int Version => 5;
        public override string Id => "005_UnifyCompanyIdVarchar5";
        public override string Description => "company_id VARCHAR(5) en tablas hijas";

        public override string Sql => @"
ALTER TABLE `accounts`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;
-- BATCH
ALTER TABLE `accounting_months`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;
-- BATCH
ALTER TABLE `companies_permission`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;
-- BATCH
ALTER TABLE `posting_period_end_closing`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;
";
    }
}
