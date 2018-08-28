using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Extensions;
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
            var userPrincipalName = claimsPrincipal.FetchPropertyValue("preferred_username");

            var queryString = req.RequestUri.ParseQueryString();
            var partition = Convert.ToString(queryString["partition"]);
            var id = Convert.ToString(queryString["id"]);

            var dataRepository = ServiceLocator.GetRequiredService<IDataRepository<string, PolicyRule>>();
            var dataPolicyRule = await dataRepository.FetchItemAsync(partition, id);

            var policyRule = await req.Content.ReadAsAsync<PolicyRule>();
            dataPolicyRule.LastModifiedBy = userPrincipalName;
            dataPolicyRule.ModifiedDate = DateTime.UtcNow;
            dataPolicyRule.Rule = policyRule.Rule;

            var resultPolicyRule = await dataRepository.UpdateItemAsync(dataPolicyRule.Partition, dataPolicyRule.Id, dataPolicyRule);

            return new OkObjectResult(resultPolicyRule);
        }
    }
}
