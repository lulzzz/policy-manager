using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Table;
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
                serviceCollection.AddStorageRepositoryToCollection<PolicyRule>();
                serviceCollection.AddStorageRepositoryToCollection<UserPolicy>();
                serviceProvider = serviceCollection.BuildServiceProvider();
            }
        }

        public static T GetRequiredService<T>()
        {
            return serviceProvider.GetRequiredService<T>();
        }

        private static void AddStorageRepositoryToCollection<T>(this ServiceCollection serviceCollection)
            where T : class, ITableEntity, new()
        {
            serviceCollection.AddScoped<IDataRepository<T>, StorageRepository<T>>((sp) =>
            {
                return new StorageRepository<T>(new StorageSettings()
                {
                    ConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString"),
                });
            });
        }
    }
}
