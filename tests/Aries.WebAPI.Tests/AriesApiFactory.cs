using System.Linq;
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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
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

        private static void Remove<T>(IServiceCollection services)
        {
            foreach (var descriptor in services.Where(d => d.ServiceType == typeof(T)).ToList())
                services.Remove(descriptor);
        }
    }
}
