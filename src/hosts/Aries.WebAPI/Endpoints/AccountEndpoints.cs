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

            group.MapGet("/balance/{accountId:int}", async (
                HttpContext http,
                int accountId,
                IFinancialService svc,
                string companyId = null,
                DateTime? startMonth = null,
                DateTime? endMonth = null) =>
            {
                var account = await svc.FindAccountAsync(accountId, http.RequestAborted);
                if (account == null)
                    return Results.NotFound();

                var cid = string.IsNullOrEmpty(companyId) ? account.CompanyId : companyId;
                var from = startMonth ?? DateTime.Today;
                var to = endMonth ?? from;
                var accounts = (await svc.GetAccountsAsync(cid, http.RequestAborted)).ToList();
                await svc.FillAccountsWithBalancesAsync(accounts, from, to, http.RequestAborted);
                var match = accounts.FirstOrDefault(a => a.Id == accountId);
                return match == null ? Results.NotFound() : Results.Ok(match);
            });

            group.MapGet("/{accountId:int}/movements", async (
                HttpContext http,
                int accountId,
                IFinancialService financial,
                IFinancialReportService reports) =>
            {
                var account = await financial.FindAccountAsync(accountId, http.RequestAborted);
                if (account == null)
                    return Results.NotFound();

                var auxiliar = account.AccountType == AccountType.Cuenta_Auxiliar;
                var table = await reports.GetAccountMovementReportAsync(accountId, auxiliar, http.RequestAborted);
                return Results.Ok(AccountMovementMapper.FromTable(table));
            });

            group.MapPost("/EvaluateParent", async (HttpContext http, Account parent, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var result = await svc.EvaluateParentForNewChildAsync(parent, http.RequestAborted);
                    if (result.Message == null)
                        result.Message = string.Empty;
                    return Results.Ok(result);
                }));

            group.MapPost("/Create", async (HttpContext http, Account account, IFinancialService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue)
                    {
                        if (account.CreatedBy == 0)
                            account.CreatedBy = userId.Value;
                        if (account.UpdatedBy == 0)
                            account.UpdatedBy = userId.Value;
                    }
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
