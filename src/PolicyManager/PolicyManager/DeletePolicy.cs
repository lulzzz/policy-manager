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
    public class DeletePolicy
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;

        public DeletePolicy(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
        }

        [FunctionName(nameof(DeletePolicy))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(DeletePolicy)} Invoked");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var queryString = req.RequestUri.ParseQueryString();
            var id = Convert.ToString(queryString["id"]);
            var partition = Convert.ToString(queryString["category"]);

            await policyRuleRepository.DeleteItemAsync(partition, id);
            return new OkResult();
        }
    }
}
