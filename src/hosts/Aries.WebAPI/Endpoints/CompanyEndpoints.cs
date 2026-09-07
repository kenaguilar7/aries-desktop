using AriesContador.Core.Models.Companies;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class CompanyEndpoints
    {
        public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/company").RequireAuthorization();

            group.MapGet("/getAll", async (HttpContext http, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    var current = userId.HasValue
                        ? await svc.FinUserByIdAsync(userId.Value, http.RequestAborted)
                        : null;
                    var list = await svc.GetAllCompaniesAsync(current, http.RequestAborted);
                    return Results.Ok(list);
                }));

            group.MapGet("/BuildCode", async (HttpContext http, IAdministrationService svc) =>
                Results.Ok(new Company { Code = await svc.GetCompanyConsecutiveAsync(http.RequestAborted) }));

            group.MapPost("/Create", async (HttpContext http, Company company, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && company.CreatedBy == 0)
                        company.CreatedBy = userId.Value;
                    await svc.CreateCompanyAsync(company, http.RequestAborted);
                    return Results.Ok(company);
                }));

            group.MapDelete("/delete/{code}", async (HttpContext http, string code, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteCompanyAsync(new Company { Code = code }, http.RequestAborted);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
