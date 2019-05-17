using Microsoft.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PolicyManager.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PolicyManager.RulesEngine.Tests
{
    [TestClass]
    public class MatchesGroupRuleTests
    {
        [TestMethod]
        public void RunHappyPath()
        {
            var initialState = Substitute.For<InitialState>();
            initialState.Groups = new List<Group>() { new Group() { DisplayName = "Users" } };

            var policies = new List<Policy>()
            {
                new Policy()
                {
                    Name = "Test Policy 1",
                    Description = "This is a test policy",
                    Actions = new List<string>() { "Read" },
                    Attributes = new List<string>() { "test" },
                    Groups = new List<string>() { "Users" },
                    Resource = "/location/1/devices",
                    Target = "Devices",
                },
            };

            var expectedResult = policies.First().Groups.Intersect(initialState.Groups.Select(g => g.DisplayName)).Count();

            var ruleService = new RuleService();
            ruleService.RunRules(initialState, policies);

            Assert.AreEqual(expectedResult, initialState.PolicyResults.Count());
        }
    }
}
