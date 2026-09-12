using System;
using System.Configuration;

namespace Aries.Desktop.Conf
{
    internal static class AppSettingReader
    {
        public static string Read(string key, string legacyConnectionStringName = null)
        {
            var fromApp = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(fromApp))
                return fromApp.Trim();

            if (string.IsNullOrWhiteSpace(legacyConnectionStringName))
                return null;

            var legacy = ConfigurationManager.ConnectionStrings[legacyConnectionStringName];
            if (legacy != null && !string.IsNullOrWhiteSpace(legacy.ConnectionString))
                return legacy.ConnectionString.Trim();

            return null;
        }

        public static bool ReadFlag(string key, string legacyConnectionStringName, bool defaultValue)
        {
            var raw = Read(key, legacyConnectionStringName);
            if (string.IsNullOrWhiteSpace(raw))
                return defaultValue;

            if (raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (raw == "0" || raw.Equals("false", StringComparison.OrdinalIgnoreCase) || raw.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;
            return defaultValue;
        }
    }
}
