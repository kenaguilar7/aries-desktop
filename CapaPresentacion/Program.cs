using System;
using System.Windows.Forms;
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

            //var services = new ServiceCollection();

            //ConfigureServices(services);

            //using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            //{
            //    var form1 = serviceProvider.GetRequiredService<FrameMenu>();
            //    Application.Run(form1);
            //}
            Application.Run(new FrameMenu());
        }

    }
}
//to do: la referencia de usuario en companies en la base de datos permmite insertar valores nulos corregir.