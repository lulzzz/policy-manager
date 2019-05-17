using PolicyManager.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyManager.DataAccess.Repositories
{
    public interface IAuthorizationService
    {
        Task<IEnumerable<PolicyResult>> EvaluateAsync(InitialState initialState);
    }

    public class AuthorizationService
        : IAuthorizationService
    {
        private readonly IDataRepository<Policy> policyRepository;

        public AuthorizationService(IDataRepository<Policy> policyRepository)
        {
            this.policyRepository = policyRepository;
        }

        public async Task<IEnumerable<PolicyResult>> EvaluateAsync(InitialState initialState)
        {
            // Fetch from gremlin
            var matchingPolicies = await policyRepository.FetchAllAsync();

            // Use Flee to evaluate expression https://github.com/mparlak/Flee/blob/master/test/Flee.Console/Program.cs
        }
    }
}
