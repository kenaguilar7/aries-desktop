using Aries.WebAPI.Infrastructure;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Services;
using AriesContador.Services;

namespace Aries.WebAPI.Endpoints
{
    public static class PosEndpoints
    {
        public static IEndpointRouteBuilder MapPosEndpoints(this IEndpointRouteBuilder app)
        {
            MapProduct(app);
            MapRegisters(app);
            MapCaja(app);
            MapSales(app);
            MapReports(app);
            return app;
        }

        private static void MapProduct(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/product").RequireAuthorization();

            group.MapGet("/byCompany/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetProductsAsync(companyId, http.RequestAborted))));

            group.MapGet("/barcode/{companyId}/{barcode}", async (HttpContext http, string companyId, string barcode, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var product = await svc.FindProductByBarcodeAsync(companyId, barcode, http.RequestAborted);
                    return product == null ? Results.NotFound() : Results.Ok(product);
                }));

            group.MapPost("/Create", async (HttpContext http, Product product, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && product.CreatedBy == 0)
                        product.CreatedBy = userId.Value;
                    product.UpdatedBy = product.CreatedBy;
                    await svc.CreateProductAsync(product, http.RequestAborted);
                    return Results.Ok(product);
                }));

            group.MapPost("/Update", async (HttpContext http, Product product, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        product.UpdatedBy = userId.Value;
                    await svc.UpdateProductAsync(product, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapDelete("/delete/{id:int}", async (HttpContext http, int id, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteProductAsync(id, http.RequestAborted);
                    return Results.Ok();
                }));
        }

        private static void MapRegisters(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/salesRegister").RequireAuthorization();

            group.MapGet("/byCompany/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetRegistersAsync(companyId, http.RequestAborted))));

            group.MapPost("/Create", async (HttpContext http, SalesRegister register, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && register.CreatedBy == 0)
                        register.CreatedBy = userId.Value;
                    register.UpdatedBy = register.CreatedBy;
                    await svc.CreateRegisterAsync(register, http.RequestAborted);
                    return Results.Ok(register);
                }));

            group.MapPost("/Update", async (HttpContext http, SalesRegister register, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        register.UpdatedBy = userId.Value;
                    await svc.UpdateRegisterAsync(register, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapDelete("/delete/{id:int}", async (HttpContext http, int id, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteRegisterAsync(id, http.RequestAborted);
                    return Results.Ok();
                }));
        }

        private static void MapCaja(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/caja").RequireAuthorization();

            group.MapGet("/estado/{registerId:int}", async (HttpContext http, int registerId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var session = await svc.GetOpenSessionAsync(registerId, http.RequestAborted);
                    return Results.Ok(session == null
                        ? CashRegisterStatus.Closed()
                        : CashRegisterStatus.Open(session));
                }));

            group.MapPost("/apertura", async (HttpContext http, OpenCashRegisterRequest request, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    var session = await svc.OpenSessionAsync(
                        request.RegisterId, request.OpeningAmount, request.Notes, userId, http.RequestAborted);
                    return Results.Ok(session);
                }));

            group.MapPost("/cierre", async (HttpContext http, CloseCashRegisterRequest request, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId() ?? 0;
                    var session = await svc.CloseSessionAsync(
                        request.RegisterId, request.DeclaredAmount, request.Notes, userId, http.RequestAborted);
                    return Results.Ok(session);
                }));

            group.MapGet("/historial/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetSessionHistoryAsync(companyId, http.RequestAborted))));
        }

        private static void MapSales(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/sale").RequireAuthorization();

            group.MapGet("/byCompany/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetSalesByCompanyAsync(companyId, http.RequestAborted))));

            group.MapGet("/bySession/{sessionId:int}", async (HttpContext http, int sessionId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetSalesBySessionAsync(sessionId, http.RequestAborted))));

            group.MapPost("/Create", async (HttpContext http, Sale sale, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && sale.CreatedBy == 0)
                        sale.CreatedBy = userId.Value;
                    sale.UpdatedBy = sale.CreatedBy;
                    await svc.CreateSaleAsync(sale, http.RequestAborted);
                    return Results.Ok(sale);
                }));
        }

        private static void MapReports(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/posReport").RequireAuthorization();

            group.MapGet("/ventasHoy/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc, DateTime? date) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetTodaySalesAsync(companyId, date, http.RequestAborted))));

            group.MapGet("/stockBajo/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc, decimal? minimo) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetLowStockAsync(
                        companyId, minimo ?? PointOfSaleService.DefaultLowStockMinimum, http.RequestAborted))));

            group.MapGet("/cierres/{companyId}", async (HttpContext http, string companyId, IPointOfSaleService svc) =>
                await EndpointRun.TryAsync(async () =>
                    Results.Ok(await svc.GetClosingsAsync(companyId, http.RequestAborted))));
        }
    }
}
