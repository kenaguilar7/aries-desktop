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

            group.MapGet("/{companyId}/accounts", (string companyId, IFinancialService svc) =>
                Results.Ok(svc.GetAccounts(companyId)));

            group.MapGet("/FindAccount/{accountId:int}", (int accountId, IFinancialService svc) =>
            {
                var account = svc.FindAccount(accountId);
                return account == null ? Results.NotFound() : Results.Ok(account);
            });

            group.MapGet("/balance/{accountId:int}", (int accountId, IFinancialService svc) =>
            {
                var account = svc.FindAccount(accountId);
                return account == null ? Results.NotFound() : Results.Ok(account);
            });

            group.MapPost("/Create", (HttpContext http, Account account, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && account.CreatedBy == 0)
                        account.CreatedBy = userId.Value;
                    svc.CreateAccount(account);
                    return Results.Ok();
                }));

            group.MapPost("/Update", (HttpContext http, Account account, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                        account.UpdatedBy = userId.Value;
                    svc.UpdateAccount(account);
                    return Results.Ok();
                }));

            group.MapPost("/Delete", (Account account, IFinancialService svc) =>
                EndpointRun.Try(() =>
                {
                    svc.DeleteAccount(account);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
