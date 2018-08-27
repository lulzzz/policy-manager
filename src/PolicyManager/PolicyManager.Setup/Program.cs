using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using System.Threading.Tasks;

namespace PolicyManager.Setup
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var documentSettings = new DocumentSettings(configuration);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IDataRepository<string, PolicyRule>, DataRepository<string, PolicyRule>>((sp) =>
            {
                return new DataRepository<string, PolicyRule>(documentSettings);
            });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var policyRuleRepository = serviceProvider.GetRequiredService<IDataRepository<string, PolicyRule>>();
            await policyRuleRepository.InitializeDatabaseAsync("/partition");
        }
    }
}
