using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Aries.Contabilidad;
using Aries.Contabilidad.Services;
using Aries.Contabilidad.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var environment = builder.HostEnvironment.Environment;
builder.Configuration.AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true);

var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();

builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientCompanyService, ClientCompanyService>();
builder.Services.AddScoped<IJournalEntryService, JournalEntryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPostingPeriodService, PostingPeriodService>();
builder.Services.AddScoped<IJournalEntryLineService, JournalEntryLineService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

var apiBaseUrl = apiSettings?.BaseUrl ?? "http://localhost:5088/";
Console.WriteLine($"Environment: {environment}");
Console.WriteLine($"API Base URL: {apiBaseUrl}");

builder.Services.AddHttpClient("AriesAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(apiSettings?.Timeout ?? 30);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddScoped<AuthenticationHeaderHandler>();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

await builder.Build().RunAsync();
