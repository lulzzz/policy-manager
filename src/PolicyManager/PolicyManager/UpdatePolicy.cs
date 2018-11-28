using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public static class UpdatePolicy
    {
        [FunctionName("UpdatePolicy")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Update Policy Invoked.");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.Identity.Name;

            var policyRule = await req.Content.ReadAsAsync<PolicyRule>();
            var dataRepository = ServiceLocator.GetRequiredService<IDataRepository<PolicyRule>>();

            var dataPolicyRule = await dataRepository.ReadItemAsync(policyRule.PartitionKey, policyRule.RowKey);
            dataPolicyRule.LastModifiedBy = userPrincipalName;
            dataPolicyRule.ModifiedDate = DateTime.UtcNow;
            dataPolicyRule.Rule = policyRule.Rule;

            var resultPolicyRule = await dataRepository.UpdateItemAsync(dataPolicyRule);

            return new OkObjectResult(resultPolicyRule);
        }
    }
}
