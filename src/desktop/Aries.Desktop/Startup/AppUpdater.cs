using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel;

namespace Aries.Desktop
{
    internal sealed class AppUpdateCheck
    {
        public string InstalledVersion { get; set; }
        public string FeedUrl { get; set; }
        public string AvailableVersion { get; set; }
        public bool HasFeed { get; set; }
        public bool HasUpdate { get; set; }
        public string Error { get; set; }
    }

    internal sealed class AppUpdateApplyResult
    {
        public bool Applied { get; set; }
        public string Version { get; set; }
    }

    internal static class AppUpdater
    {
        public static string FileVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return string.IsNullOrWhiteSpace(info.FileVersion) ? "(desconocida)" : info.FileVersion;
            }
        }

        public static async Task<AppUpdateCheck> CheckAsync()
        {
            var installed = FileVersion;
            var updateUrl = GlobalConfig.UpdateUrl;
            if (string.IsNullOrWhiteSpace(updateUrl))
            {
                return new AppUpdateCheck
                {
                    InstalledVersion = installed,
                    HasFeed = false
                };
            }

            if (!FeedReachable(updateUrl))
            {
                return new AppUpdateCheck
                {
                    InstalledVersion = installed,
                    FeedUrl = updateUrl,
                    HasFeed = true,
                    Error = "El feed no responde (" + updateUrl + ")."
                };
            }

            try
            {
                using (var manager = new UpdateManager(updateUrl))
                {
                    var info = await manager.CheckForUpdate().ConfigureAwait(false);
                    var squirrelCurrent = manager.CurrentlyInstalledVersion();
                    if (squirrelCurrent != null)
                        installed = squirrelCurrent.ToString();

                    var available = info != null && info.FutureReleaseEntry != null
                        ? info.FutureReleaseEntry.Version.ToString()
                        : null;
                    var hasUpdate = info != null
                        && info.ReleasesToApply != null
                        && info.ReleasesToApply.Count > 0;

                    return new AppUpdateCheck
                    {
                        InstalledVersion = installed,
                        FeedUrl = updateUrl,
                        AvailableVersion = available,
                        HasFeed = true,
                        HasUpdate = hasUpdate
                    };
                }
            }
            catch (Exception ex)
            {
                return new AppUpdateCheck
                {
                    InstalledVersion = installed,
                    FeedUrl = updateUrl,
                    HasFeed = true,
                    Error = ex.GetBaseException().Message
                };
            }
        }

        public static async Task<AppUpdateApplyResult> ApplyAsync()
        {
            var updateUrl = GlobalConfig.UpdateUrl;
            if (string.IsNullOrWhiteSpace(updateUrl))
                throw new InvalidOperationException("No hay canal de actualización configurado (UpdateUrl).");

            if (!FeedReachable(updateUrl))
                throw new InvalidOperationException("El feed no responde (" + updateUrl + ").");

            using (var manager = new UpdateManager(updateUrl))
            {
                var release = await manager.UpdateApp().ConfigureAwait(false);
                if (release == null)
                    return new AppUpdateApplyResult();

                return new AppUpdateApplyResult
                {
                    Applied = true,
                    Version = release.Version.ToString()
                };
            }
        }

        public static void Restart()
        {
            UpdateManager.RestartApp();
        }

        /// <summary>
        /// Squirrel usa HttpClient (~100 s). Si el API/S3 no está, el splash se queda
        /// en "Buscando actualizaciones…". Un GET corto a RELEASES falla ya.
        /// 404 cuenta como alcanzable (feed vacío).
        /// </summary>
        private static bool FeedReachable(string updateUrl)
        {
            try
            {
                var releases = updateUrl.TrimEnd('/') + "/RELEASES";
                var request = (HttpWebRequest)WebRequest.Create(releases);
                request.Method = "GET";
                request.Timeout = 4000;
                request.ReadWriteTimeout = 4000;
                request.AllowAutoRedirect = false;
                using (var response = request.GetResponse())
                    return true;
            }
            catch (WebException ex)
            {
                var http = ex.Response as HttpWebResponse;
                return http != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
