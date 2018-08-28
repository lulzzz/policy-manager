using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PolicyManager.Helpers
{
    public static class AuthHelper
    {
        private static readonly IConfigurationManager<OpenIdConnectConfiguration> configurationManager;

        static AuthHelper()
        {
            var issuer = Environment.GetEnvironmentVariable("Issuer");

            var documentRetriever = new HttpDocumentRetriever
            {
                RequireHttps = issuer.StartsWith("https://")
            };

            configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public static async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue authenticationHeaderValue, ILogger log)
        {
            if (authenticationHeaderValue?.Scheme != "Bearer")
            {
                return null;
            }

            var configuration = await configurationManager.GetConfigurationAsync(CancellationToken.None);
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            var audience = Environment.GetEnvironmentVariable("Audience");

            var validationParameter = new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                ValidAudience = audience,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = configuration.SigningKeys
            };

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(authenticationHeaderValue.Parameter, validationParameter, out var token);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    configurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException ex)
                {
                    log.LogError(ex.Message);
                    return null;
                }
            }

            return result;
        }
    }
}