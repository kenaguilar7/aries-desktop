using System;
using System.Collections.Generic;
using System.Linq;

namespace AriesContador.Data.Migrations
{
    /// <summary>
    /// Una migración es una clase con SQL versionado. El corredor la aplica
    /// una sola vez y la registra en <c>__schema_migrations</c>.
    /// Separa lotes con la línea <c>-- BATCH</c> (necesario para CREATE PROCEDURE).
    /// </summary>
    public abstract class SqlMigration
    {
        public abstract int Version { get; }

        public abstract string Id { get; }

        public abstract string Description { get; }

        public abstract string Sql { get; }

        public IReadOnlyList<string> SqlBatches
        {
            get
            {
                return Sql
                    .Split(new[] { "\r\n-- BATCH\r\n", "\n-- BATCH\n", "\r\n-- BATCH\n", "\n-- BATCH\r\n" }, StringSplitOptions.None)
                    .Select(batch => batch.Trim())
                    .Where(batch => batch.Length > 0)
                    .ToArray();
            }
        }
    }
}
