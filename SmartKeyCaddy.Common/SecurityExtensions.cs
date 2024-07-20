using System.Security.Claims;

namespace SmartKeyCaddy.Common
{
    public static class SecurityExtensions
    {
        public static string GetSubjectId(ClaimsPrincipal user)
        {
            var claim = user.Claims.SingleOrDefault(claim => string.Equals(claim.Type, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase));
            return claim?.Value ?? string.Empty;
        }

        public static string GetClaim(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.SingleOrDefault(claim => string.Equals(claim.Type, claimType, StringComparison.OrdinalIgnoreCase));
            return claim?.Value;
        }
    }
}
