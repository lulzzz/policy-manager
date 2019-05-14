using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Lexer;
using PolicyManager.Results;
using PolicyManager.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolicyManager
{
    public class Validate
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDataRepository<PolicyRule> policyRuleRepository;
        private readonly IDataRepository<UserPolicy> userPolicyRepository;

        public Validate(IAuthenticationService authenticationService, IDataRepository<PolicyRule> policyRuleRepository, IDataRepository<UserPolicy> userPolicyRepository)
        {
            this.authenticationService = authenticationService;
            this.policyRuleRepository = policyRuleRepository;
            this.userPolicyRepository = userPolicyRepository;
        }

        [FunctionName("Validate")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(Validate)} Invoked");

            var claimsPrincipal = await authenticationService.ValidateTokenAsync(req?.Headers.Authorization);
            if (claimsPrincipal == null) return new UnauthorizedResult();

            var userPrincipalName = claimsPrincipal.Identity.Name;
            var queryString = req.RequestUri.ParseQueryString();
            var context = Convert.ToString(queryString["context"]);

            var userPolicies = await userPolicyRepository.ReadItemsAsync(userPrincipalName);
            var validateResults = new List<ValidateResult>();
            foreach (var userPolicy in userPolicies)
            {
                var policyRule = await policyRuleRepository.ReadItemAsync(userPolicy.PolicyCategory, userPolicy.PolicyId);
                var initialState = new Dictionary<string, string>()
                {
                    { "context", context },
                    { "userName", userPrincipalName }
                };

                var lexerProvider = new LexerProvider();
                var returnValue = lexerProvider.RunLexer(initialState, policyRule.Rule);
                validateResults.Add(new ValidateResult() { Id = policyRule.RowKey, Category = policyRule.Category, PolicyName = policyRule.DisplayName, Description = policyRule.Description, Result = returnValue.ToString() });
            }

            return new OkObjectResult(validateResults);
        }
    }
}
