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
    public static class AddUserPolicy
    {
        [FunctionName(nameof(AddUserPolicy))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(AddUserPolicy)} Invoked");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.Identity.Name;

            var userPolicy = await req.Content.ReadAsAsync<UserPolicy>();
            userPolicy.RowKey = Guid.NewGuid().ToString();
            userPolicy.PartitionKey = userPrincipalName;
            userPolicy.UserPrincipalName = userPrincipalName;
            userPolicy.CreatedBy = userPrincipalName;
            userPolicy.CreatedDate = DateTime.UtcNow;
            userPolicy.LastModifiedBy = userPrincipalName;
            userPolicy.ModifiedDate = DateTime.UtcNow;

            var dataRepository = ServiceLocator.GetRequiredService<IDataRepository<UserPolicy>>();
            var resultUserPolicy = await dataRepository.CreateItemAsync(userPolicy);
            return new OkObjectResult(resultUserPolicy);
        }
    }
}
