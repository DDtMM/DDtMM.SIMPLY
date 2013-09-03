using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.ReferrerTypes
{
    public class TokenReference : IReference
    {
        public TokenType TokenType;
        public Referrer Source { get; private set; }

        public TokenReference(TokenType tokenType, Referrer source) 
            : base()
        {
            TokenType = tokenType;
            Source = source;
        }

        public TokenParseResult ParseRule(Token token, ParserSettings settings)
        {
            TokenParseResult result;

            if (TokenType.Name == token.TokenType.Name)
            {
                result = TokenParseResult.Success(new SyntaxNode(Source, token), token.Next);
            }
            else
            {
                result = TokenParseResult.Failed(token);
            }

            return result;
        }
    }
}
