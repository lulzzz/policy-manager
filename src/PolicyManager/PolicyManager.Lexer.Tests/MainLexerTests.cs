using System.Collections.Generic;
using Xunit;

namespace PolicyManager.Lexer.Tests
{
    public class MainLexerTests
    {
        [Fact]
        public void TestBasicIfStatement()
        {
            var initialState = new Dictionary<string, string>
            {
                { "userName", "juswen@microsoft.com" }
            };
            var configuration = @"if (userName == ""juswen@microsoft.com"") { return ""allow""; } else { return ""deny""; }";

            var lexerProvider = new LexerProvider();
            var policyVisitorResult = lexerProvider.RunLexer(initialState, configuration);

            Assert.Equal("allow", policyVisitorResult.StringResult);
        }

        [Fact]
        public void TestBasicIfNotEqualStatement()
        {
            var initialState = new Dictionary<string, string>
            {
                { "userName", "juswen@microsoft.com" }
            };
            var configuration = @"if (userName != ""juswen@microsoft.com"") { return ""allow""; } else { return ""deny""; }";

            var lexerProvider = new LexerProvider();
            var policyVisitorResult = lexerProvider.RunLexer(initialState, configuration);

            Assert.Equal("deny", policyVisitorResult.StringResult);
        }
    }
}
