using System.Security.Claims;

namespace SecondBrain.Utils
{
    public static class ClaimsPrincipalHelper
    {
        public static Guid GetCurrentUserId(ClaimsPrincipal claims)
        {
            return Guid.Parse(claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
