using System.Collections.Generic;
using Xunit;

namespace PolicyManager.Lexer.Tests
{
    public class ContextLexerTests
    {
        [Fact]
        public void TestContextExactMatch()
        {
            var initialState = new Dictionary<string, string>
            {
                { "context", "/user/profile" },
                { "userName", "juswen@microsoft.com" }
            };
            var configuration = @"if (userName == ""juswen@microsoft.com"" && context == ""/user/profile"") { return ""allow""; } else { return ""deny""; }";

            var lexerProvider = new LexerProvider();
            var returnValue = lexerProvider.RunLexer(initialState, configuration);

            Assert.Equal("allow", returnValue.ToString());
        }
    }
}
