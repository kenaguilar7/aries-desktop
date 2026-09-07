using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AriesContador.Core;
using AriesContador.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Aries.WebAPI.Tests
{
    public class AriesApiFactory : WebApplicationFactory<Program>
    {
        public StubAdministrationService Admin { get; } = new StubAdministrationService();
        public StubFinancialService Financial { get; } = new StubFinancialService();
        public string UpdatesRoot { get; } =
            Path.Combine(Path.GetTempPath(), "aries-squirrel-" + Guid.NewGuid().ToString("N"));

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Directory.CreateDirectory(UpdatesRoot);
            builder.UseEnvironment("Testing");
            builder.UseSetting("Updates:Root", UpdatesRoot);
            builder.ConfigureServices(services =>
            {
                Remove<IAdministrationService>(services);
                Remove<IFinancialService>(services);
                Remove<IUnitOfWork>(services);

                services.AddSingleton(Admin);
                services.AddSingleton<IAdministrationService>(sp => Admin);
                services.AddSingleton(Financial);
                services.AddSingleton<IFinancialService>(sp => Financial);
            });
        }

        public string WriteMinimalFeed(string nupkgName = "CapaPresentacion-1.0.0-full.nupkg")
        {
            Directory.CreateDirectory(UpdatesRoot);
            var body = Encoding.UTF8.GetBytes("squirrel-nupkg-fixture");
            File.WriteAllBytes(Path.Combine(UpdatesRoot, nupkgName), body);
            var sha1 = Convert.ToHexString(SHA1.HashData(body)).ToLowerInvariant();
            var releases = sha1 + " " + nupkgName + " " + body.Length + "\n";
            File.WriteAllText(Path.Combine(UpdatesRoot, "RELEASES"), releases, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            return nupkgName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (Directory.Exists(UpdatesRoot))
                        Directory.Delete(UpdatesRoot, true);
                }
                catch
                {
                    // temp
                }
            }

            base.Dispose(disposing);
        }

        private static void Remove<T>(IServiceCollection services)
        {
            foreach (var descriptor in services.Where(d => d.ServiceType == typeof(T)).ToList())
                services.Remove(descriptor);
        }
    }
}
