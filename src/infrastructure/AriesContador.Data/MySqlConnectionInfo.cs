using System;

namespace AriesContador.Data
{
    public static class MySqlConnectionInfo
    {
        public static string ReadPart(string connectionString, string key)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(key))
                return null;

            foreach (var part in connectionString.Split(';'))
            {
                var trimmed = part.Trim();
                var eq = trimmed.IndexOf('=');
                if (eq <= 0)
                    continue;
                var name = trimmed.Substring(0, eq).Trim();
                if (name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring(eq + 1).Trim();
            }

            return null;
        }

        public static string Server(string connectionString)
        {
            return ReadPart(connectionString, "Server")
                ?? ReadPart(connectionString, "Data Source")
                ?? ReadPart(connectionString, "Host");
        }

        public static bool IsLoopbackHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return false;

            var value = host.Trim().Trim('[', ']');
            return value.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || value.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
                || value.Equals("::1", StringComparison.OrdinalIgnoreCase)
                || value.Equals("(local)", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsLoopback(string connectionString)
        {
            return IsLoopbackHost(Server(connectionString));
        }

        /// <summary>
        /// En Debug no se auto-migran hosts remotos (RDS de prueba, etc.) salvo
        /// <c>ARIES_APPLY_MIGRATIONS=1</c>. Release siempre aplicaría.
        /// El escritorio y el API ya no auto-migran: un administrador aplica
        /// el esquema desde Sistema → Actualizaciones.
        /// </summary>
        public static bool ShouldAutoMigrate(string connectionString, bool debugBuild, string applyMigrationsEnv)
        {
            if (!debugBuild)
                return true;
            if (IsLoopback(connectionString))
                return true;

            if (string.IsNullOrWhiteSpace(applyMigrationsEnv))
                return false;

            var flag = applyMigrationsEnv.Trim();
            return flag == "1"
                || flag.Equals("true", StringComparison.OrdinalIgnoreCase)
                || flag.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
