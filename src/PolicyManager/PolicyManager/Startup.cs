using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PolicyManager;
using PolicyManager.Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PolicyManager
{
    public class Startup
        : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        }
    }
}
