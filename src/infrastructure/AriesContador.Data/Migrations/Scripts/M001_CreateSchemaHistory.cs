namespace AriesContador.Data.Migrations.Scripts
{
    public sealed class M001_CreateSchemaHistory : SqlMigration
    {
        public override int Version => 1;
        public override string Id => "001_CreateSchemaHistory";
        public override string Description => "Tabla de historial de migraciones";

        public override string Sql => @"
CREATE TABLE IF NOT EXISTS `__schema_migrations` (
  `migration_id` VARCHAR(128) NOT NULL,
  `version` INT NOT NULL,
  `description` VARCHAR(255) NOT NULL,
  `applied_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`migration_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
";
    }
}
