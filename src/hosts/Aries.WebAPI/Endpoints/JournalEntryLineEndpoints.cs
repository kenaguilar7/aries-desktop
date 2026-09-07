using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class JournalEntryLineEndpoints
    {
        public static IEndpointRouteBuilder MapJournalEntryLineEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/journalEntryLine").RequireAuthorization();

            group.MapPost("/CreateJournalEntryLine", async (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && line.CreatedBy == 0)
                        line.CreatedBy = userId.Value;
                    await svc.CreateJournalEntryLineAsync(line, http.RequestAborted);
                    return Results.Ok(line.Id);
                }));

            group.MapPost("/UpdateJournalEntryLine", async (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        line.UpdatedBy = userId.Value;
                    await svc.UpdateJournalEntryLineAsync(line, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/DeleteJournalEntryLine", async (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteJournalEntryLineAsync(line, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapGet("/FindJournalEntryLine/{journalEntryId:int}", async (HttpContext http, int journalEntryId, IFinancialService svc) =>
                Results.Ok(await svc.GetJournalEntryLineByJournalEntryIdAsync(journalEntryId, http.RequestAborted)));

            group.MapPost("/RestoreJournalEntryLine", async (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
            {
                await svc.RestoreJournalEntryLineAsync(line, http.RequestAborted);
                return Results.Ok();
            });

            return app;
        }
    }
}
