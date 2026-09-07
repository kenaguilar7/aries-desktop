namespace Aries.WebAPI.Infrastructure
{
    internal static class EndpointRun
    {
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
