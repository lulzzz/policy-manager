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
    public class FetchPolicies
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;

        public FetchPolicies(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
        }

        [FunctionName(nameof(FetchPolicies))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Fetch Policies Invoked.");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var queryString = req.RequestUri.ParseQueryString();
            var partition = Convert.ToString(queryString["category"]);

            var policyRules = await policyRuleRepository.ReadItemsAsync(partition);
            return new OkObjectResult(policyRules);
        }
    }
}
