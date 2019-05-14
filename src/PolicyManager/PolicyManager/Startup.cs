using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PolicyManager;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Services;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PolicyManager
{
    public class Startup
        : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IDataRepository<PolicyRule>, StorageRepository<PolicyRule>>((sp) =>
            {
                return new StorageRepository<PolicyRule>(new StorageSettings()
                {
                    ConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString"),
                });
            });
            builder.Services.AddScoped<IDataRepository<UserPolicy>, StorageRepository<UserPolicy>>((sp) =>
            {
                return new StorageRepository<UserPolicy>(new StorageSettings()
                {
                    ConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString"),
                });
            });
        }
    }
}
