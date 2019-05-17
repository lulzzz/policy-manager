using System.Threading.Tasks;

namespace PolicyManager.Setup
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddScoped<IDataRepository<PolicyRule>, StorageRepository<PolicyRule>>();
            //serviceCollection.AddScoped<IDataRepository<UserPolicy>, StorageRepository<UserPolicy>>();
            //var serviceProvider = serviceCollection.BuildServiceProvider();

            //var policyRuleRepository = serviceProvider.GetRequiredService<IDataRepository<PolicyRule>>();
            //await policyRuleRepository.InitializeDatabaseAsync();

            //var userPolicyRepository = serviceProvider.GetRequiredService<IDataRepository<UserPolicy>>();
            //await userPolicyRepository.InitializeDatabaseAsync();
        }
    }
}
