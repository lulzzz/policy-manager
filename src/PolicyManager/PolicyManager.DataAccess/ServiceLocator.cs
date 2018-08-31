using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using System;

namespace PolicyManager.DataAccess
{
    public static class ServiceLocator
    {
        private static readonly IServiceProvider serviceProvider;

        static ServiceLocator()
        {
            if (serviceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddRepositoryToServiceCollection<PolicyRule>();
                serviceCollection.AddRepositoryToServiceCollection<UserPolicy>();
                serviceProvider = serviceCollection.BuildServiceProvider();
            }
        }

        public static T GetRequiredService<T>()
        {
            return serviceProvider.GetRequiredService<T>();
        }

        private static DocumentSettings DocumentSettings
        {
            get
            {
                if (Environment.GetEnvironmentVariables().Contains("DocumentEndpoint"))
                {
                    return new DocumentSettings()
                    {
                        DocumentEndpoint = new Uri(Environment.GetEnvironmentVariable("DocumentEndpoint")),
                        DocumentKey = Environment.GetEnvironmentVariable("DocumentKey"),
                        DatabaseId = Environment.GetEnvironmentVariable("DatabaseId")
                    };
                }

                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");

                var configuration = builder.Build();
                return new DocumentSettings()
                {
                    DocumentEndpoint = new Uri(configuration["DocumentEndpoint"]),
                    DocumentKey = configuration["DocumentKey"],
                    DatabaseId = configuration["DatabaseId"]
                };
            }
        }

        private static void AddRepositoryToServiceCollection<T>(this ServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection.AddScoped<IDataRepository<string, T>, DataRepository<string, T>>((sp) =>
            {
                return new DataRepository<string, T>(DocumentSettings);
            });
        }
    }
}
