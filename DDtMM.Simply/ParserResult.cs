using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class ParserResult
    {
        public SyntaxNode Root;
        public List<Token> PassedTokens;

        public ParserResult()
        {
            PassedTokens = new List<Token>();
            Root = new SyntaxNode(null, null);
        }
    }
}
