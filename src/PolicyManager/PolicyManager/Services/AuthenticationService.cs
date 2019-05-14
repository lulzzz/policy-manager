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

namespace PolicyManager.Services
{
    public interface IAuthenticationService
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue authenticationHeaderValue);
    }

    public class AuthenticationService
        : IAuthenticationService
    {
        private readonly IConfigurationManager<OpenIdConnectConfiguration> configurationManager;
        private readonly ILogger logger;

        public AuthenticationService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AuthenticationService>();

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

        public async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue authenticationHeaderValue)
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
                    logger.LogError(ex.Message);
                    return result;
                }
            }

            return result;
        }
    }
}
