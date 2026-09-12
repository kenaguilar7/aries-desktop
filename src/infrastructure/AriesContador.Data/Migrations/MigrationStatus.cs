using System;
using System.Collections.Generic;

namespace AriesContador.Data.Migrations
{
    public sealed class MigrationStatus
    {
        public MigrationStatus(
            bool historyTableExists,
            int catalogVersion,
            int appliedVersion,
            IReadOnlyList<SqlMigration> applied,
            IReadOnlyList<SqlMigration> pending,
            IReadOnlyList<string> checksumMismatches)
        {
            HistoryTableExists = historyTableExists;
            CatalogVersion = catalogVersion;
            AppliedVersion = appliedVersion;
            Applied = applied;
            Pending = pending;
            ChecksumMismatches = checksumMismatches;
        }

        public bool HistoryTableExists { get; }

        public int CatalogVersion { get; }

        public int AppliedVersion { get; }

        public IReadOnlyList<SqlMigration> Applied { get; }

        public IReadOnlyList<SqlMigration> Pending { get; }

        public IReadOnlyList<string> ChecksumMismatches { get; }

        public bool IsUpToDate => HistoryTableExists && Pending.Count == 0;

        public static MigrationStatus From(
            IReadOnlyList<SqlMigration> catalog,
            IReadOnlyDictionary<string, string> appliedChecksums,
            bool historyTableExists)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));
            if (appliedChecksums == null)
                throw new ArgumentNullException(nameof(appliedChecksums));

            var applied = new List<SqlMigration>();
            var pending = new List<SqlMigration>();
            var mismatches = new List<string>();
            var catalogVersion = 0;
            var appliedVersion = 0;

            foreach (var migration in catalog)
            {
                if (migration.Version > catalogVersion)
                    catalogVersion = migration.Version;

                string storedChecksum;
                if (appliedChecksums.TryGetValue(migration.Id, out storedChecksum))
                {
                    applied.Add(migration);
                    if (migration.Version > appliedVersion)
                        appliedVersion = migration.Version;

                    if (!string.IsNullOrWhiteSpace(storedChecksum)
                        && !storedChecksum.Equals(migration.Checksum, StringComparison.OrdinalIgnoreCase))
                    {
                        mismatches.Add(migration.Id);
                    }
                }
                else
                {
                    pending.Add(migration);
                }
            }

            return new MigrationStatus(
                historyTableExists,
                catalogVersion,
                appliedVersion,
                applied,
                pending,
                mismatches);
        }
    }
}
