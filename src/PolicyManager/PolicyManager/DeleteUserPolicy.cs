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
    public class DeleteUserPolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<UserPolicy> userPolicyRepository;

        public DeleteUserPolicy(IAuthenticationService authenticationService, IDataRepository<UserPolicy> userPolicyRepository)
        {
            this.authenticationService = authenticationService;
            this.userPolicyRepository = userPolicyRepository;
        }

        [FunctionName(nameof(DeleteUserPolicy))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(DeletePolicy)} Invoked");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var queryString = req.RequestUri.ParseQueryString();
            var id = Convert.ToString(queryString["id"]);
            var partition = claimsPrincipal.Identity.Name;

            await userPolicyRepository.DeleteItemAsync(partition, id);

            return new OkResult();
        }
    }
}
