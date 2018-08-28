using System.Security.Claims;

namespace PolicyManager.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string FetchPropertyValue(this ClaimsPrincipal claimsPrincipal, string key)
        {
            foreach (var claim in claimsPrincipal.Claims)
            {
                if (claim.Type == key)
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }
    }
}
