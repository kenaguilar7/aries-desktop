using System;
using System.Windows.Forms;
using AriesContador.Core.Services;
using AriesContador.Services;
using Microsoft.Extensions.DependencyInjection;
namespace CapaPresentacion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GlobalConfig globalConfig = new GlobalConfig();

            var services = new ServiceCollection();
            //services.AddSingleton<IFinancialService, FinancialService>();
            services.AddSingleton<IHttpAdministrationService, HttpAdministrationService>();
            services.AddSingleton<FrameMenu>();
            
            var serviceProvider = services.BuildServiceProvider();
            var form = serviceProvider.GetService<FrameMenu>();
            Application.Run(form);

            //Application.Run(new FrameMenu());
        }

    }
}
//to do: la referencia de usuario en companies en la base de datos permmite insertar valores nulos corregir.