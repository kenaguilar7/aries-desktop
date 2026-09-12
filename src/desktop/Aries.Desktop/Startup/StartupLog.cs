using System;
using System.IO;

namespace Aries.Desktop
{
    internal static class StartupLog
    {
        public static string FilePath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AriesContador",
                    "startup.log");
            }
        }

        public static void Write(string message)
        {
            try
            {
                var directory = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.AppendAllText(
                    FilePath,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + (message ?? string.Empty) + Environment.NewLine);
            }
            catch
            {
                // El log no debe impedir el arranque.
            }
        }
    }
}
