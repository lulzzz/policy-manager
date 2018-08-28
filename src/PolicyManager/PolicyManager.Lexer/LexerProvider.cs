using Antlr4.Runtime;
using PolicyManager.Lexer.Models;
using PolicyManager.Lexer.Visitors;
using System.Collections.Generic;

namespace PolicyManager.Lexer
{
    public class LexerProvider
    {
        public ReturnValue RunLexer(Dictionary<string, string> initialState, string configuration)
        {
            var antlrInputStream = new AntlrInputStream(configuration);
            var policyManagerLexer = new PolicyManagerLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(policyManagerLexer);
            var policyManagerParser = new PolicyManagerParser(commonTokenStream);
            var parserContext = policyManagerParser.parse();

            var visitor = new PolicyManagerVisitor(initialState);
            return visitor.Visit(parserContext);
        }
    }
}
