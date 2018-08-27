using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public static class AddPolicy
    {
        [FunctionName("AddPolicy")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Add Policy Invoked...");

            var policyRule = await req.Content.ReadAsAsync<PolicyRule>();

            return new OkObjectResult(policyRule);
        }
    }
}
