using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class TokenParseResult
    {
        public static TokenParseResult Failed(Token nextToken)
        {
            return new TokenParseResult()
            {
                Node = null,
                IsSuccesful = false,
                NextToken = nextToken
            };     
        }

        public static TokenParseResult Success(SyntaxNode node, Token nextToken)
        {
            return new TokenParseResult()
            {
                Node = node,
                IsSuccesful = true,
                NextToken = nextToken
            };
        }

        public static TokenParseResult Success(Rule rule, Token startToken, List<SyntaxNode> nodes, Token nextToken)
        {
            SyntaxNode node = new SyntaxNode(rule, startToken);
            if (nodes != null) node.AddRange(nodes);

            return new TokenParseResult()
            {
                Node = node,
                IsSuccesful = true,
                NextToken = nextToken
            };
        }

        static public implicit operator SyntaxNode(TokenParseResult result)
        {
            return (result) ? result.Node : null;
        }

        static public implicit operator bool(TokenParseResult result)
        {
            return (result != null && result.IsSuccesful);
        }

        public bool IsSuccesful;
        public SyntaxNode Node;
        public Token NextToken;


    }
}
