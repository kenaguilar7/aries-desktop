using System.Text.Json;
using Aries.WebAPI.Endpoints;
using Aries.WebAPI.Infrastructure;
using AriesContador.Core;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Data.Migrations;
using AriesContador.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                 ?? Array.Empty<string>();
if (corsOrigins.Length == 0)
    corsOrigins = new[] { "http://localhost:8080" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Falta Jwt:Key (env Jwt__Key, user-secrets o appsettings.Local.json).");
if (builder.Environment.IsProduction()
    && jwtKey.IndexOf("change-me", StringComparison.OrdinalIgnoreCase) >= 0)
    throw new InvalidOperationException("Jwt:Key de producción no puede ser el placeholder.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Aries.WebAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Aries.Desktop";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IConnectionString, ConfigConnectionString>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdministrationService, AdministrationService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<IFinancialReportService, FinancialReportService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    var mysql = app.Configuration.GetConnectionString("MySQLDefault");
    if (string.IsNullOrWhiteSpace(mysql))
    {
        throw new InvalidOperationException(
            "Falta ConnectionStrings:MySQLDefault. Usa appsettings.Development.json, copia appsettings.Local.json.example o docker compose.");
    }

    var server = ConfigConnectionString.ReadPart(mysql, "Server")
                 ?? ConfigConnectionString.ReadPart(mysql, "Host")
                 ?? "(sin Server)";
    var port = ConfigConnectionString.ReadPart(mysql, "Port") ?? "3306";
    var database = ConfigConnectionString.ReadPart(mysql, "Database") ?? "(sin Database)";
    app.Logger.LogInformation(
        "Ambiente {Environment}: MySQL {Server}:{Port} / {Database}",
        app.Environment.EnvironmentName, server, port, database);

    var connection = app.Services.GetRequiredService<IConnectionString>();
    try
    {
        var status = await new DatabaseMigrator(connection.MySQLDefault).GetStatusAsync();
        if (status.IsUpToDate)
        {
            app.Logger.LogInformation(
                "MySQL esquema al día ({Count} migraciones). No se aplica SQL al arranque.",
                status.Applied.Count);
        }
        else
        {
            var pending = status.Pending.Count == 0
                ? "(sin historial)"
                : string.Join(", ", status.Pending.Select(m => m.Id));
            app.Logger.LogWarning(
                "MySQL esquema pendiente: {Pending}. Un administrador debe aplicarlas en Sistema > Actualizaciones.",
                pending);
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudo leer el esquema MySQL al arranque (no se aplica SQL).");
    }
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Error interno",
            status = 500,
            detail = "Error interno"
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    });
});

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.RoutePrefix = string.Empty);
}

var updatesRoot = app.Configuration["Updates:Root"];
if (string.IsNullOrWhiteSpace(updatesRoot))
    updatesRoot = Path.Combine(app.Environment.ContentRootPath, "updates");
Directory.CreateDirectory(updatesRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(updatesRoot),
    RequestPath = "/updates",
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "no-cache";
    }
});
app.Logger.LogInformation("Feed Squirrel: /updates  (carpeta {UpdatesRoot})", updatesRoot);

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", async (IConnectionString cs) =>
{
    try
    {
        using var conn = new MySql.Data.MySqlClient.MySqlConnection(cs.MySQLDefault);
        await conn.OpenAsync();
        return Results.Ok(new { status = "ok" });
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "unhealthy", detail = ex.Message }, statusCode: 503);
    }
}).AllowAnonymous();

app.MapGet("/updates", () =>
{
    var names = Directory.Exists(updatesRoot)
        ? Directory.GetFiles(updatesRoot).Select(Path.GetFileName).OrderBy(n => n).ToArray()
        : Array.Empty<string>();
    return Results.Ok(new { path = "/updates", files = names });
}).AllowAnonymous();

app.MapAuthEndpoints();
app.MapCompanyEndpoints();
app.MapUserEndpoints();
app.MapAccountEndpoints();
app.MapPostingPeriodEndpoints();
app.MapJournalEntryEndpoints();
app.MapJournalEntryLineEndpoints();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls.Count > 0 ? string.Join(", ", app.Urls) : "(sin URL)";
    app.Logger.LogInformation("Aries.WebAPI escuchando en {Urls}", urls);
});

app.Run();

public partial class Program
{
}
