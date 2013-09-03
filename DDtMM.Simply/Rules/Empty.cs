using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{
    /// <summary>
    /// An empty rule
    /// </summary>
    public class Empty : Rule
    {
        public new Quantifier Quantifier
        {
            get { return Quantifier.One; }
        }

        protected override string RenderRuleContents()
        {
            return "/* empty */";
        }

        public override Rule DeepCopy()
        {
            return new Empty()
            {
                ProductionName = ProductionName
            };
        }

        public override TokenParseResult Parse(Token token, ParserSettings settings)
        {
            return TokenParseResult.Success(this, null, null, token);
        }
    }
}
