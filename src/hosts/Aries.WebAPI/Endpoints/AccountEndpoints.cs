using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/account").RequireAuthorization();

            group.MapGet("/{companyId}/accounts", async (HttpContext http, string companyId, IFinancialService svc) =>
                Results.Ok(await svc.GetAccountsAsync(companyId, http.RequestAborted)));

            group.MapGet("/FindAccount/{accountId:int}", async (HttpContext http, int accountId, IFinancialService svc) =>
            {
                var account = await svc.FindAccountAsync(accountId, http.RequestAborted);
                return account == null ? Results.NotFound() : Results.Ok(account);
            });

            group.MapGet("/balance/{accountId:int}", async (HttpContext http, int accountId, IFinancialService svc) =>
            {
                var account = await svc.FindAccountAsync(accountId, http.RequestAborted);
                return account == null ? Results.NotFound() : Results.Ok(account);
            });

            group.MapPost("/Create", async (HttpContext http, Account account, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && account.CreatedBy == 0)
                        account.CreatedBy = userId.Value;
                    await svc.CreateAccountAsync(account, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/Update", async (HttpContext http, Account account, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        account.UpdatedBy = userId.Value;
                    await svc.UpdateAccountAsync(account, http.RequestAborted);
                    return Results.Ok();
                }));

            group.MapPost("/Delete", async (HttpContext http, Account account, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    await svc.DeleteAccountAsync(account, http.RequestAborted);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
