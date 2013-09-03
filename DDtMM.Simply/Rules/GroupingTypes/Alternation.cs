using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.GroupingTypes
{
    public class Alternation : IGroupingType
    {
        static public readonly Alternation Instance = new Alternation();

        public string Delimiter
        {
            get { return " | ";  }
        }


        public TokenParseResult ParseRule(Grouping group, Token startToken, ParserSettings settings)
        {
            List<SyntaxNode> nodes = new List<SyntaxNode>();
            TokenParseResult groupResult = TokenParseResult.Failed(startToken);
            Token token;
            TokenParseResult ruleResult;

            token = startToken;

            foreach (Rule rule in group)
            {
                ruleResult = rule.Parse(token, settings);
                if (ruleResult)
                {
                    if (!groupResult || settings.AlternateCritera == ParserSettings.SelectionCriteria.First
                        || (groupResult.Node.GetTokenCount() < ruleResult.Node.GetTokenCount()))
                    {
                        groupResult = ruleResult;
                        groupResult.Node.Rule = group;
                        if (settings.AlternateCritera == ParserSettings.SelectionCriteria.First) break;

                    }
                }
            }

            return groupResult;
        }
    }
}
