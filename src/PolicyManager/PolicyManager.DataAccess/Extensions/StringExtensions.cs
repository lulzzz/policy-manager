using System.Linq;

namespace PolicyManager.DataAccess.Extensions
{
    public static class StringExtensions
    {
        public static string ToPolicyRulePartitionKey(this string category)
        {
            return category?.ToUpperInvariant();
        }

        public static string ToUserPolicyPartitionKey(this string userPrincipalName)
        {
            return userPrincipalName?.Split('@').FirstOrDefault();
        }
    }
}
