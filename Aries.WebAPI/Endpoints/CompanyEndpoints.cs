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
                    var current = userId.HasValue ? svc.FinUserById(userId.Value) : null;
                    var list = await svc.GetAllCompanies(current);
                    return Results.Ok(list);
                }));

            group.MapGet("/BuildCode", async (IAdministrationService svc) =>
                Results.Ok(new Company { Code = await svc.GetCompanyConsecutive() }));

            group.MapPost("/Create", async (HttpContext http, Company company, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && company.CreatedBy == 0)
                        company.CreatedBy = userId.Value;
                    await svc.CreateCompany(company);
                    return Results.Ok(company);
                }));

            group.MapDelete("/delete/{code}", async (string code, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteCompany(new Company { Code = code });
                    return Results.Ok();
                }));

            return app;
        }
    }
}
