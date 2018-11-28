using PolicyManager.DataAccess;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using System.Threading.Tasks;

namespace PolicyManager.Setup
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var policyRuleRepository = ServiceLocator.GetRequiredService<IDataRepository<PolicyRule>>();
            await policyRuleRepository.InitializeDatabaseAsync();

            var userPolicyRepository = ServiceLocator.GetRequiredService<IDataRepository<UserPolicy>>();
            await userPolicyRepository.InitializeDatabaseAsync();
        }
    }
}
