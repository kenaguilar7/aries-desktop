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

            group.MapPost("/CreateJournalEntryLine", (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && line.CreatedBy == 0)
                        line.CreatedBy = userId.Value;
                    svc.CreateJournalEntryLine(line);
                    return Results.Ok(line.Id);
                }));

            group.MapPost("/UpdateJournalEntryLine", (HttpContext http, JournalEntryLine line, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        line.UpdatedBy = userId.Value;
                    svc.UpdateJournalEntryLine(line);
                    return Results.Ok();
                }));

            group.MapPost("/DeleteJournalEntryLine", (JournalEntryLine line, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.DeleteJournalEntryLine(line);
                    return Results.Ok();
                }));

            group.MapGet("/FindJournalEntryLine/{journalEntryId:int}", (int journalEntryId, IFinancialService svc) =>
                Results.Ok(svc.GetJournalEntryLineByJournalEntryId(journalEntryId)));

            group.MapPost("/RestoreJournalEntryLine", (JournalEntryLine line, IFinancialService svc) =>
            {
                svc.RestoreJournalEntryLine(line);
                return Results.Ok();
            });

            return app;
        }
    }
}
