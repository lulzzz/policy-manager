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
            var policyRuleRepository = ServiceLocator.GetRequiredService<IDataRepository<string, PolicyRule>>();
            await policyRuleRepository.InitializeDatabaseAsync("/partition");

            var userPolicyRepository = ServiceLocator.GetRequiredService<IDataRepository<string, UserPolicy>>();
            await userPolicyRepository.InitializeDatabaseAsync("/partition");
        }
    }
}
