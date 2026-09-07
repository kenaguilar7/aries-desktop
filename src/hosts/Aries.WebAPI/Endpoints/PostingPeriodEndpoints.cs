using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class PostingPeriodEndpoints
    {
        public static IEndpointRouteBuilder MapPostingPeriodEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/postingPeriod").RequireAuthorization();

            group.MapGet("/GetPostingPeriods/{companyId}", async (HttpContext http, string companyId, IFinancialService svc) =>
                Results.Ok(await svc.GetPostingPeriodsAsync(companyId, http.RequestAborted)));

            group.MapGet("/GetAvailablePostingPeriodsForBeCreated/{companyId}", async (HttpContext http, string companyId, IFinancialService svc) =>
                Results.Ok(await svc.GetAvailablePostingPeriodsForBeCreatedAsync(companyId, http.RequestAborted)));

            group.MapPost("/Create", async (HttpContext http, PostingPeriod period, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && period.CreatedBy == 0)
                        period.CreatedBy = userId.Value;
                    await svc.CreatePostingPeriodAsync(period, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/Update", async (HttpContext http, PostingPeriod period, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.UpdatePostingPeriodAsync(period, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/Delete", async (HttpContext http, PostingPeriod period, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeletePostingPeriodAsync(period, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/ClosePostingPeriod", async (HttpContext http, PostingPeriodEndClosing closing, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && closing.CreatedBy == 0)
                        closing.CreatedBy = userId.Value;
                    await svc.ClosePostingPeriodAsync(closing, http.RequestAborted);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
