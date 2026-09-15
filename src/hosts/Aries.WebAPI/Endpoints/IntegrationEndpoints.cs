using Aries.WebAPI.Infrastructure;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Endpoints
{
    public static class IntegrationEndpoints
    {
        public static IEndpointRouteBuilder MapIntegrationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/integration").RequireAuthorization();

            group.MapGet("/account-map/{companyId}", async (HttpContext http, string companyId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetAccountMapAsync(companyId, http.RequestAborted))));

            group.MapPost("/account-map", async (HttpContext http, PosAccountMap map, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                    {
                        if (map.CreatedBy == 0)
                            map.CreatedBy = userId.Value;
                        map.UpdatedBy = userId.Value;
                    }
                    await svc.SaveAccountMapAsync(map, http.RequestAborted);
                    return Results.Ok(map);
                }));

            group.MapPost("/account-map/{companyId}/ensure-accounts", async (HttpContext http, string companyId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    return Results.Ok(await svc.EnsureSuggestedAccountsAsync(companyId, userId, http.RequestAborted));
                }));

            group.MapGet("/pos-accounting/{companyId}/unposted", async (HttpContext http, string companyId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetUnpostedSessionsAsync(companyId, http.RequestAborted))));

            group.MapGet("/pos-accounting/preview/{sessionId:int}", async (HttpContext http, int sessionId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.PreviewSessionAsync(sessionId, http.RequestAborted))));

            group.MapPost("/pos-accounting/post/{sessionId:int}", async (HttpContext http, int sessionId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    return Results.Ok(await svc.PostSessionAsync(sessionId, userId, http.RequestAborted));
                }));

            group.MapGet("/reconciliation/{companyId}", async (HttpContext http, string companyId, IPosAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetReconciliationAsync(companyId, http.RequestAborted))));

            group.MapGet("/purchase-account-map/{companyId}", async (HttpContext http, string companyId, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetAccountMapAsync(companyId, http.RequestAborted))));

            group.MapPost("/purchase-account-map", async (HttpContext http, PurchaseAccountMap map, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                    {
                        if (map.CreatedBy == 0)
                            map.CreatedBy = userId.Value;
                        map.UpdatedBy = userId.Value;
                    }
                    await svc.SaveAccountMapAsync(map, http.RequestAborted);
                    return Results.Ok(map);
                }));

            group.MapPost("/purchase-account-map/{companyId}/ensure-accounts", async (HttpContext http, string companyId, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    return Results.Ok(await svc.EnsureSuggestedAccountsAsync(companyId, userId, http.RequestAborted));
                }));

            group.MapGet("/purchase-accounting/{companyId}/unposted", async (HttpContext http, string companyId, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetUnpostedPurchasesAsync(companyId, http.RequestAborted))));

            group.MapGet("/purchase-accounting/preview/{purchaseId:int}", async (HttpContext http, int purchaseId, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.PreviewPurchaseAsync(purchaseId, http.RequestAborted))));

            group.MapPost("/purchase-accounting/post/{purchaseId:int}", async (HttpContext http, int purchaseId, IPurchaseAccountingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    return Results.Ok(await svc.PostPurchaseAsync(purchaseId, userId, http.RequestAborted));
                }));

            return app;
        }
    }
}
