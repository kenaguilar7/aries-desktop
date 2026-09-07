using System.Security.Claims;

namespace Aries.WebAPI.Infrastructure
{
    public static class HttpUser
    {
        public static int? TryGetUserId(this HttpContext http)
        {
            var raw = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? http.User.FindFirstValue("UserId");
            return int.TryParse(raw, out var id) ? id : (int?)null;
        }

        public static int GetUserId(this HttpContext http)
        {
            return TryGetUserId(http)
                   ?? throw new InvalidOperationException("Token sin claim UserId");
        }
    }
}
