using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public string Checksum
        {
            get
            {
                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(Sql ?? string.Empty));
                    var text = new StringBuilder(hash.Length * 2);
                    foreach (var value in hash)
                        text.Append(value.ToString("x2"));
                    return text.ToString();
                }
            }
        }

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
