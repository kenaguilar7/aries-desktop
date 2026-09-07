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

            group.MapGet("/GetPostingPeriods/{companyId}", (string companyId, IFinancialService svc) =>
                Results.Ok(svc.GetPostingPeriods(companyId)));

            group.MapGet("/GetAvailablePostingPeriodsForBeCreated/{companyId}", (string companyId, IFinancialService svc) =>
                Results.Ok(svc.GetAvailablePostingPeriodsForBeCreated(companyId)));

            group.MapPost("/Create", (HttpContext http, PostingPeriod period, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && period.CreatedBy == 0)
                        period.CreatedBy = userId.Value;
                    svc.CreatePostingPeriod(period);
                    return Results.Ok();
                }));

            group.MapPost("/Update", (PostingPeriod period, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.UpdatePostingPeriod(period);
                    return Results.Ok();
                }));

            group.MapPost("/Delete", (PostingPeriod period, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.DeletePostingPeriod(period);
                    return Results.Ok();
                }));

            group.MapPost("/ClosePostingPeriod", (HttpContext http, PostingPeriodEndClosing closing, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && closing.CreatedBy == 0)
                        closing.CreatedBy = userId.Value;
                    svc.ClosePostingPeriod(closing);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
