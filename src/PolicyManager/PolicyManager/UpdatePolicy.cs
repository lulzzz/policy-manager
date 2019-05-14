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
    public class UpdatePolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;

        public UpdatePolicy(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
        }

        [FunctionName(nameof(UpdatePolicy))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Update Policy Invoked.");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var userPrincipalName = claimsPrincipal.Identity.Name;
            var policyRule = await req.Content.ReadAsAsync<PolicyRule>();
            var dataPolicyRule = await policyRuleRepository.ReadItemAsync(policyRule.PartitionKey, policyRule.RowKey);
            dataPolicyRule.LastModifiedBy = userPrincipalName;
            dataPolicyRule.ModifiedDate = DateTime.UtcNow;
            dataPolicyRule.Rule = policyRule.Rule;

            var resultPolicyRule = await policyRuleRepository.UpdateItemAsync(dataPolicyRule);
            return new OkObjectResult(resultPolicyRule);
        }
    }
}
