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
    public class AddUserPolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<UserPolicy> userPolicyRepository;

        public AddUserPolicy(IAuthenticationService authenticationService, IDataRepository<UserPolicy> userPolicyRepository)
        {
            this.authenticationService = authenticationService;
            this.userPolicyRepository = userPolicyRepository;
        }

        [FunctionName(nameof(AddUserPolicy))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(AddUserPolicy)} Invoked");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var userPrincipalName = claimsPrincipal.Identity.Name;
            var userPolicy = await req.Content.ReadAsAsync<UserPolicy>();
            userPolicy.RowKey = Guid.NewGuid().ToString();
            userPolicy.PartitionKey = userPrincipalName;
            userPolicy.UserPrincipalName = userPrincipalName;
            userPolicy.CreatedBy = userPrincipalName;
            userPolicy.CreatedDate = DateTime.UtcNow;
            userPolicy.LastModifiedBy = userPrincipalName;
            userPolicy.ModifiedDate = DateTime.UtcNow;

            var resultUserPolicy = await userPolicyRepository.CreateItemAsync(userPolicy);
            return new OkObjectResult(resultUserPolicy);
        }
    }
}
