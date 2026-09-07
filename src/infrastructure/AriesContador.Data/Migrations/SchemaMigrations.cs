using System;
using System.Collections.Generic;
using System.Linq;

namespace AriesContador.Data.Migrations
{
    public static class SchemaMigrations
    {
        public static IReadOnlyList<SqlMigration> All { get; } = Load();

        private static IReadOnlyList<SqlMigration> Load()
        {
            var list = typeof(SchemaMigrations).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(SqlMigration).IsAssignableFrom(type))
                .Select(type => (SqlMigration)Activator.CreateInstance(type))
                .OrderBy(migration => migration.Version)
                .ThenBy(migration => migration.Id, StringComparer.Ordinal)
                .ToArray();

            var duplicateVersions = list.GroupBy(m => m.Version).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
            if (duplicateVersions.Length > 0)
                throw new InvalidOperationException("Versiones de migración duplicadas: " + string.Join(", ", duplicateVersions));

            var duplicateIds = list.GroupBy(m => m.Id, StringComparer.Ordinal).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
            if (duplicateIds.Length > 0)
                throw new InvalidOperationException("Ids de migración duplicados: " + string.Join(", ", duplicateIds));

            return list;
        }
    }
}
