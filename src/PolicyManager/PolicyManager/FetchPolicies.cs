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
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public static class FetchPolicies
    {
        [FunctionName("FetchPolicies")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Fetch Policies Invoked.");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.FetchPropertyValue("preferred_username");

            var queryString = req.RequestUri.ParseQueryString();
            var category = Convert.ToString(queryString["category"]);
            var partition = category.ToPolicyRulePartitionKey();

            var dataRepository = ServiceLocator.GetRequiredService<IDataRepository<string, PolicyRule>>();

            var policyRules = await dataRepository.FetchItemsAsync(partition);

            return new OkObjectResult(policyRules);
        }
    }
}
