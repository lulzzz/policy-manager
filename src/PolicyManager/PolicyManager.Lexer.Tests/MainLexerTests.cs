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
            var returnValue = lexerProvider.RunLexer(initialState, configuration);

            Assert.Equal("allow", returnValue.ToString());
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
            var returnValue = lexerProvider.RunLexer(initialState, configuration);

            Assert.Equal("deny", returnValue.ToString());
        }
    }
}
