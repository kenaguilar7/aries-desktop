using Aries.WebAPI.Infrastructure;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Services;

namespace Aries.WebAPI.Endpoints
{
    public static class PurchasingEndpoints
    {
        public static IEndpointRouteBuilder MapPurchasingEndpoints(this IEndpointRouteBuilder app)
        {
            MapSupplierEndpoints(app);
            MapPurchaseEndpoints(app);
            return app;
        }

        private static void MapSupplierEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/supplier").RequireAuthorization();

            group.MapGet("/GetAll/{companyId}", async (HttpContext http, string companyId, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetSuppliersAsync(companyId, http.RequestAborted))));

            group.MapGet("/Find/{id:int}", async (HttpContext http, int id, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var supplier = await svc.FindSupplierAsync(id, http.RequestAborted);
                    return supplier == null ? Results.NotFound() : Results.Ok(supplier);
                }));

            group.MapPost("/Create", async (HttpContext http, Supplier supplier, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && supplier.CreatedBy == 0)
                        supplier.CreatedBy = userId.Value;
                    supplier.UpdatedBy = supplier.CreatedBy;
                    await svc.CreateSupplierAsync(supplier, http.RequestAborted);
                    return Results.Ok(supplier);
                }));

            group.MapPost("/Update", async (HttpContext http, Supplier supplier, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        supplier.UpdatedBy = userId.Value;
                    await svc.UpdateSupplierAsync(supplier, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapDelete("/Delete/{id:int}", async (HttpContext http, int id, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteSupplierAsync(id, http.RequestAborted);
                    return Results.Ok();
                }));
        }

        private static void MapPurchaseEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/purchase").RequireAuthorization();

            group.MapGet("/GetAll/{companyId}", async (HttpContext http, string companyId, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetPurchasesAsync(companyId, http.RequestAborted))));

            group.MapGet("/Find/{id:int}", async (HttpContext http, int id, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var purchase = await svc.FindPurchaseAsync(id, http.RequestAborted);
                    return purchase == null ? Results.NotFound() : Results.Ok(purchase);
                }));

            group.MapPost("/Confirm", async (HttpContext http, Purchase purchase, IPurchasingService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && purchase.CreatedBy == 0)
                        purchase.CreatedBy = userId.Value;
                    purchase.UpdatedBy = purchase.CreatedBy;
                    var confirmed = await svc.ConfirmPurchaseAsync(purchase, http.RequestAborted);
                    return Results.Ok(confirmed);
                }));
        }
    }
}
