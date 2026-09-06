using AriesContador.Data;

namespace Aries.WebAPI.Infrastructure
{
    public class ConfigConnectionString : IConnectionString
    {
        public ConfigConnectionString(IConfiguration configuration)
        {
            MySQLDefault = configuration.GetConnectionString("MySQLDefault") ?? string.Empty;
        }

        public string MySQLDefault { get; }

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
    }
}
