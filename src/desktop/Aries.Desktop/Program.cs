using System;
using System.Configuration;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.Email;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Data.Migrations;
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
            GlobalConfig globalConfig = new GlobalConfig();

            try
            {
                new DatabaseMigrator(GlobalConfig.ConnectionString.MySQLDefault).ApplyPending();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar la base de datos. La aplicación no puede continuar.\n\n" + ex.Message,
                    "Aries Contador",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

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
            Application.Run(form);
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
