using System;
using System.Diagnostics;

namespace Aries.Reporting.Utils
{
    public static class ExcelShell
    {
        public static bool OpenAfterSave { get; set; } =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ARIES_SKIP_OPEN_EXCEL"));

        public static void Open(string path)
        {
            if (!OpenAfterSave || string.IsNullOrWhiteSpace(path))
                return;

            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}
