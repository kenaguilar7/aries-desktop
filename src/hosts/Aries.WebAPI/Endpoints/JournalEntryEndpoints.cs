using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class JournalEntryEndpoints
    {
        public static IEndpointRouteBuilder MapJournalEntryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/journalEntry").RequireAuthorization();

            group.MapPost("/CreateJournalEntry", async (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && entry.CreatedBy == 0)
                        entry.CreatedBy = userId.Value;
                    await svc.CreateJournalEntryAsync(entry, http.RequestAborted);
                    return Results.Ok(entry.Id);
                }));

            group.MapPost("/UpdateJournalEntry", async (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        entry.UpdatedBy = userId.Value;
                    await svc.UpdateJournalEntryAsync(entry, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/DeleteJournalEntry", async (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteJournalEntryAsync(entry, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapGet("/GetConsecutiveNumber/{postingPeriodId:int}", async (HttpContext http, int postingPeriodId, IFinancialService svc) =>
                Results.Ok(await svc.CreateJournalEntryConsecutiveAsync(postingPeriodId, http.RequestAborted)));

            group.MapGet("/GetJournalEntries/{postingPeriodId:int}", async (HttpContext http, int postingPeriodId, IFinancialService svc) =>
                Results.Ok(await svc.GetJournalEntriesAsync(postingPeriodId, http.RequestAborted)));

            group.MapGet("/GetJournalEntryById/{id:int}", async (HttpContext http, int id, IFinancialService svc) =>
            {
                var entry = await svc.GetJournalEntryByIdAsync(id, http.RequestAborted);
                return entry == null ? Results.NotFound() : Results.Ok(entry);
            });

            group.MapPost("/UpdatedJournalEntryPeriod", async (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.UpdatedJournalEntryPeriodAsync(entry, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/RestoreJournalEntry", async (HttpContext http, JournalEntry entry, IFinancialService svc) =>
            {
                await svc.RestoreJournalEntryAsync(entry, http.RequestAborted);
                return Results.Ok();
            });

            return app;
        }
    }
}
