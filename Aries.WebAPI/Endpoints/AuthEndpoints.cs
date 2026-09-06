using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using Aries.WebAPI.Infrastructure;

namespace Aries.WebAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").AllowAnonymous();
            group.MapPost("/login", (
                Login param,
                IAdministrationService administration,
                IJwtTokenService jwt) =>
            {
                var result = administration.Login(param);
                if (result.User == null)
                    return Results.Unauthorized();

                result.Token = jwt.Create(result.User);
                return Results.Ok(result);
            });
            return app;
        }
    }
}
