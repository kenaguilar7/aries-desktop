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

            group.MapGet("/GetAllUsers", (IAdministrationService svc) =>
                Results.Ok(svc.GetAllUsers()));

            group.MapPost("/Create", (HttpContext http, User user, IAdministrationService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && user.UpdatedBy == 0)
                        user.UpdatedBy = userId.Value;
                    svc.CreateUser(user);
                    user.Password = null;
                    return Results.Ok(user);
                }));

            group.MapPost("/Update", (HttpContext http, User user, IAdministrationService svc) =>
                EndpointRun.Try(() =>
                {
                    var userId = http.TryGetUserId();
                    if (userId.HasValue && user.UpdatedBy == 0)
                        user.UpdatedBy = userId.Value;
                    svc.UpdateUser(user);
                    return Results.Ok();
                }));

            return app;
        }
    }
}
