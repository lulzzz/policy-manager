using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess;
using PolicyManager.DataAccess.Extensions;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Extensions;
using PolicyManager.Helpers;
using PolicyManager.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public static class Validate
    {
        [FunctionName("Validate")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Validate Invoked.");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.FetchPropertyValue("preferred_username");
            var userObjectId = claimsPrincipal.FetchPropertyValue("oid");
            var partition = userPrincipalName.ToUserPolicyPartitionKey();

            var queryString = req.RequestUri.ParseQueryString();
            var context = Convert.ToString(queryString["context"]);

            var userPolicyRepository = ServiceLocator.GetRequiredService<IDataRepository<string, UserPolicy>>();
            var policyRuleRepository = ServiceLocator.GetRequiredService<IDataRepository<string, PolicyRule>>();

            var policyRulePartitions = await policyRuleRepository.FetchPartitionKeys();
            var userPolicy = await userPolicyRepository.FindItemAsync(partition, up => up.UserObjectId == userObjectId);

            var policyRules = new List<PolicyRule>();
            foreach (var partitionItem in policyRulePartitions)
            {
                var results = await policyRuleRepository.FindItemsAsync(partitionItem.Partition, pr => userPolicy.PolicyIds.Contains(pr.Id));
                policyRules.AddRange(results);
            }

            var validateResults = new List<ValidateResult>();

            return new OkObjectResult(validateResults);
        }
    }
}
