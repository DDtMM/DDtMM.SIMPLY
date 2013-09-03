using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.ReferrerTypes
{
    public class RuleReference : IReference
    {
        public Rule Rule;
        public Referrer Source { get; private set; }

        public RuleReference(Rule referredRule, Referrer source) 
            : base()
        {
            Rule = referredRule;
            Source = source;
        }
 
        /// <summary>
        /// Calls Rule's Parse and adds as child result if succesful.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenParseResult ParseRule(Token token, ParserSettings settings)
        {
            TokenParseResult result = Rule.Parse(token, settings);
            if (result.IsSuccesful)
            {
                SyntaxNode parentNode = new SyntaxNode(Source, token);
                parentNode.Add(result.Node);
                result.Node = parentNode;
            }
            return result;
        }
   
    }
}
