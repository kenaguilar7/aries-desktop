namespace Aries.WebAPI.Infrastructure
{
    internal static class EndpointRun
    {
        public static IResult Try(Func<IResult> action)
        {
            try
            {
                return action();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        public static async Task<IResult> TryAsync(Func<Task<IResult>> action)
        {
            try
            {
                return await action();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}
