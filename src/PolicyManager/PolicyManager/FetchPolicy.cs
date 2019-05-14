using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public class FetchPolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;

        public FetchPolicy(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
        }

        [FunctionName("FetchPolicy")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Fetch Policy Invoked.");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var queryString = req.RequestUri.ParseQueryString();
            var id = Convert.ToString(queryString["id"]);
            var partition = Convert.ToString(queryString["category"]);

            var policyRule = await policyRuleRepository.ReadItemAsync(partition, id);
            return new OkObjectResult(policyRule);
        }
    }
}
