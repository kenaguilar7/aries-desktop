using System;
using System.Configuration;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.Email;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aries.Desktop
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ApplicationExit += OnApplicationExit;

            FrameSplash splash = null;
            try
            {
                splash = new FrameSplash();
                splash.Show();
                Application.DoEvents();

                splash.SetStatus("Leyendo configuración…");
                new GlobalConfig();
                StartupLog.Write(
                    "ambiente=" + GlobalConfig.EnvironmentName
                    + " server=" + GlobalConfig.MySqlServer
                    + " database=" + GlobalConfig.MySqlDatabase
                    + " update=" + (string.IsNullOrWhiteSpace(GlobalConfig.UpdateUrl) ? "(ninguno)" : GlobalConfig.UpdateUrl));

                splash.SetStatus("Buscando actualizaciones…");
                ApplyStartupAppUpdate();

                splash.SetStatus("Iniciando…");
                var services = new ServiceCollection();
                services.AddSingleton<IConnectionString>(GlobalConfig.ConnectionString);
                services.AddSingleton(ReadSmtpOptions());
                services.AddTransient<IUnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<IConnectionString>()));
                services.AddTransient<IAdministrationService, AdministrationService>();
                services.AddTransient<IFinancialService, FinancialService>();
                services.AddTransient<IFinancialReportService, FinancialReportService>();
                services.AddTransient<IPermissionService, PermissionService>();
                services.AddTransient<IEmailService, EmailService>();
                services.AddTransient<FrameMenu>();

                var serviceProvider = services.BuildServiceProvider();
                GlobalConfig.Services = serviceProvider;
                var form = serviceProvider.GetRequiredService<FrameMenu>();
                splash.Close();
                splash.Dispose();
                splash = null;
                Application.Run(form);
            }
            catch (Exception ex)
            {
                StartupLog.Write("Arranque: " + ex);
                if (splash != null)
                {
                    splash.Close();
                    splash.Dispose();
                }
                MessageBox.Show(
                    "No se pudo iniciar Aries Contador.\n\n" + ex.GetBaseException().Message,
                    "Aries Contador",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            StartupLog.Write("ThreadException: " + e.Exception);
            MessageBox.Show(
                e.Exception.GetBaseException().Message,
                "Aries Contador",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StartupLog.Write("UnhandledException: " + e.ExceptionObject);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            var disposable = GlobalConfig.Services as IDisposable;
            if (disposable != null)
            {
                try { disposable.Dispose(); }
                catch (Exception ex) { StartupLog.Write("Dispose DI: " + ex.Message); }
            }
            GlobalConfig.Services = null;
        }

        /// <summary>
        /// Al arrancar solo se busca el exe (Squirrel). El esquema MySQL
        /// lo aplica un administrador en Sistema → Actualizaciones.
        /// </summary>
        private static void ApplyStartupAppUpdate()
        {
            if (string.IsNullOrWhiteSpace(GlobalConfig.UpdateUrl))
            {
                StartupLog.Write("Squirrel omitido (sin UpdateUrl). Esquema: Sistema > Actualizaciones.");
                return;
            }

            try
            {
                var result = AppUpdater.ApplyAsync().GetAwaiter().GetResult();
                if (result == null || !result.Applied)
                {
                    StartupLog.Write("Squirrel: sin actualización. feed=" + GlobalConfig.UpdateUrl);
                    return;
                }

                StartupLog.Write("Squirrel aplicó " + result.Version + " desde " + GlobalConfig.UpdateUrl);
                MessageBox.Show(
                    "Se instaló la versión " + result.Version + ". Aries se reiniciará.",
                    "Aries Contador",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                AppUpdater.Restart();
            }
            catch (Exception ex)
            {
                StartupLog.Write("Squirrel: " + ex.GetBaseException().Message);
            }
        }

        private static SmtpOptions ReadSmtpOptions()
        {
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out port))
                port = 587;

            return new SmtpOptions
            {
                Host = ConfigurationManager.AppSettings["SmtpHost"],
                Port = port,
                UserName = ConfigurationManager.AppSettings["SmtpUser"],
                Password = ConfigurationManager.AppSettings["SmtpPassword"],
                FromAddress = ConfigurationManager.AppSettings["SmtpFrom"],
                FromDisplayName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "Sistemas Aries"
            };
        }
    }
}
