using System;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CapaPresentacion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Escritorio in-process (MySQL): login, maestros, cuentas, periodos y asientos.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GlobalConfig globalConfig = new GlobalConfig();

            var services = new ServiceCollection();
            services.AddSingleton<IConnectionString>(GlobalConfig.ConnectionString);
            services.AddSingleton<IUnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<IConnectionString>()));
            services.AddSingleton<IAdministrationService, AdministrationService>();
            services.AddSingleton<IFinancialService, FinancialService>();
            services.AddSingleton<IFinancialReportService, FinancialReportService>();
            services.AddSingleton<FrameMenu>();

            var serviceProvider = services.BuildServiceProvider();
            GlobalConfig.Services = serviceProvider;
            var form = serviceProvider.GetRequiredService<FrameMenu>();
            Application.Run(form);
        }
    }
}

//to do: la referencia de usuario en companies en la base de datos permmite insertar valores nulos corregir.
