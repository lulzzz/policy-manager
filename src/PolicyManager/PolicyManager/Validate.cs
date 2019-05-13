using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManager.DataAccess;
using PolicyManager.DataAccess.Models;
using PolicyManager.DataAccess.Repositories;
using PolicyManager.Extensions;
using PolicyManager.Helpers;
using PolicyManager.Lexer;
using PolicyManager.Results;
using System;
using System.Collections.Generic;
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
            log.LogInformation($"{nameof(Validate)} Invoked");

            var claimsPrincipal = await AuthHelper.ValidateTokenAsync(req?.Headers?.Authorization, log);
            if (claimsPrincipal == null) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var userPrincipalName = claimsPrincipal.Identity.Name;
            var partition = userPrincipalName;

            var queryString = req.RequestUri.ParseQueryString();
            var context = Convert.ToString(queryString["context"]);

            var policyRuleRepository = ServiceLocator.GetRequiredService<IDataRepository<PolicyRule>>();
            var userPolicyRepository = ServiceLocator.GetRequiredService<IDataRepository<UserPolicy>>();
            var userPolicies = await userPolicyRepository.ReadItemsAsync(partition);

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
