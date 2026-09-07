using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/user").RequireAuthorization();

            group.MapGet("/GetAllUsers", async (HttpContext http, IAdministrationService svc) =>
                Results.Ok(await svc.GetAllUsersAsync(http.RequestAborted)));

            group.MapPost("/Create", async (HttpContext http, User user, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && user.UpdatedBy == 0)
                        user.UpdatedBy = userId.Value;
                    await svc.CreateUserAsync(user, http.RequestAborted);
                    user.Password = null;
                    return Results.Ok(user);
                }));

            group.MapPost("/Update", async (HttpContext http, User user, IAdministrationService svc) =>
                await EndpointRun.TryAsync(async () =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && user.UpdatedBy == 0)
                        user.UpdatedBy = userId.Value;
                    await svc.UpdateUserAsync(user, http.RequestAborted);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
