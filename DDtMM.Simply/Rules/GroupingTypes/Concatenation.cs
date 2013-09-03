using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.GroupingTypes
{
    public class Concatenation : IGroupingType
    {
        static public readonly  Concatenation Instance = new Concatenation();

        public string Delimiter
        {
            get { return " "; }
        }

        /// <summary>
        /// Parses a Group who's children are part of a chain and do not alternate
        /// </summary>
        /// <param name="group"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenParseResult ParseRule(Grouping group, Token startToken, ParserSettings settings)
        {

            List<SyntaxNode> nodes = new List<SyntaxNode>();
            TokenParseResult groupResult = TokenParseResult.Failed(startToken);

            TokenParseResult ruleResult;
            int ruleIdx;
            Rule rule;
            Token token;
            // got the minimum results and wasn't an exception
            bool lastRuleOK;
            List<SyntaxNode> ruleNodes = new List<SyntaxNode>();

            ruleIdx = 0;
            token = startToken;
            lastRuleOK = true;

            while (lastRuleOK && ruleIdx < group.Count && token != null)
            {
                // every rule has its own scope
                rule = group[ruleIdx];
                ruleNodes.Clear();
                lastRuleOK = false;
                while (token != null)
                {
                    if (ruleResult = rule.Parse(token, settings))
                    {
                        ruleNodes.Add(ruleResult);
                        token = ruleResult.NextToken;
                        if (ruleNodes.Count >= rule.Quantifier.MaxValue)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                lastRuleOK = (rule.Quantifier.MinValue <= ruleNodes.Count) ^ rule.IsException;


                // if we achieved min qty then add to nodes.
                if (lastRuleOK)
                {
                    nodes.AddRange(ruleNodes);
                    ruleIdx++;
                }
            }

            if (lastRuleOK && ruleIdx >= group.LastRequiredRuleIndex && nodes.Count > 0)
            {
                SyntaxNode parent = new SyntaxNode(group, startToken);
                parent.AddRange(nodes);
                // token should be set to the last succesful nextToken
                groupResult = TokenParseResult.Success(parent, token);
            }

            return groupResult;
        }
    }
}
