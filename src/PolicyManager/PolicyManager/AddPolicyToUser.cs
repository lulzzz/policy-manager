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
    public static class AddPolicyToUser
    {
        [FunctionName("AddPolicyToUser")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Add Policy to User Invoked.");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.FetchPropertyValue("preferred_username");
            var userObjectId = claimsPrincipal.FetchPropertyValue("oid");

            var dataRepository = ServiceLocator.GetRequiredService<IDataRepository<string, UserPolicy>>();

            var userPolicy = await req.Content.ReadAsAsync<UserPolicy>();
            userPolicy.UserPrincipalName = userPrincipalName;
            userPolicy.UserObjectId = userObjectId;
            userPolicy.CreatedBy = userPrincipalName;
            userPolicy.CreatedDate = DateTime.UtcNow;
            userPolicy.LastModifiedBy = userPrincipalName;
            userPolicy.ModifiedDate = DateTime.UtcNow;

            var resultUserPolicy = await dataRepository.CreateItemAsync(userPolicy.Partition, userPolicy);

            return new OkObjectResult(resultUserPolicy);
        }
    }
}
