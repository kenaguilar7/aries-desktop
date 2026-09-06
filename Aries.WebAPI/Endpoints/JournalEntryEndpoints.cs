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

            group.MapPost("/CreateJournalEntry", (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && entry.CreatedBy == 0)
                        entry.CreatedBy = userId.Value;
                    svc.CreateJournalEntry(entry);
                    return Results.Ok(entry.Id);
                }));

            group.MapPost("/UpdateJournalEntry", (HttpContext http, JournalEntry entry, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        entry.UpdatedBy = userId.Value;
                    svc.UpdateJournalEntry(entry);
                    return Results.Ok();
                }));

            group.MapPost("/DeleteJournalEntry", (JournalEntry entry, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.DeleteJournalEntry(entry);
                    return Results.Ok();
                }));

            group.MapGet("/GetConsecutiveNumber/{postingPeriodId:int}", (int postingPeriodId, IFinancialService svc) =>
                Results.Ok(svc.CreateJournalEntryConsecutive(postingPeriodId)));

            group.MapGet("/GetJournalEntries/{postingPeriodId:int}", (int postingPeriodId, IFinancialService svc) =>
                Results.Ok(svc.GetJournalEntries(postingPeriodId)));

            group.MapGet("/GetJournalEntryById/{id:int}", (int id, IFinancialService svc) =>
            {
                var entry = svc.GetJournalEntryById(id);
                return entry == null ? Results.NotFound() : Results.Ok(entry);
            });

            group.MapPost("/UpdatedJournalEntryPeriod", (JournalEntry entry, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.UpdatedJournalEntryPeriod(entry);
                    return Results.Ok();
                }));

            group.MapPost("/RestoreJournalEntry", (JournalEntry entry, IFinancialService svc) =>
            {
                svc.RestoreJournalEntry(entry);
                return Results.Ok();
            });

            return app;
        }
    }
}
