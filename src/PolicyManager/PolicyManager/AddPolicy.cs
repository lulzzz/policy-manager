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
    public class AddPolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;

        public AddPolicy(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
        }

        [FunctionName(nameof(AddPolicy))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(AddPolicy)} Invoked");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var userPrincipalName = claimsPrincipal.Identity.Name;
            var policyRule = await req.Content.ReadAsAsync<PolicyRule>();
            policyRule.RowKey = Guid.NewGuid().ToString();
            policyRule.PartitionKey = policyRule.Category;
            policyRule.CreatedBy = userPrincipalName;
            policyRule.CreatedDate = DateTime.UtcNow;
            policyRule.LastModifiedBy = userPrincipalName;
            policyRule.ModifiedDate = DateTime.UtcNow;

            var resultPolicyRule = await policyRuleRepository.CreateItemAsync(policyRule);
            return new OkObjectResult(resultPolicyRule);
        }
    }
}
