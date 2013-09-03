using DDtMM.SIMPLY;
using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Rules.GroupingTypes;
using DDtMM.SIMPLY.Rules.ReferrerTypes;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class Parser
    {
        public Lexer Lexer { get; set; }
        public Grammar Grammar { get; set; }
        public ParserSettings Settings;
 
        public Parser(ParserSettings settings = null)
        {
            Grammar = new Grammar();
            Lexer = new Lexer();
            Settings = settings ?? new ParserSettings();
        }

        public ParserResult Parse(string text)
        {
            List<Token> tokens = Lexer.Tokenize(text);
            
            return Parse(tokens);
        }

        public virtual ParserResult Parse(List<Token> tokens)
        {
            TokenParseResult tokenResult;
            ParserResult docResult = new ParserResult();
            Token token = tokens.FirstOrDefault();
            Token startToken;
            List<Rule> rules = Grammar.Compile(Lexer).Productions
                .Where(r => Settings.RootProductionNames.Count == 0 || Settings.RootProductionNames.Contains(r.ProductionName)).ToList();

            while (token != null)
            {
                startToken = token;
                tokenResult = TokenParseResult.Failed(token);

                foreach (Rule rule in rules)
                {
                    tokenResult = ParseRule(rule, token, Settings);

                    if (tokenResult && (startToken != tokenResult.NextToken || Settings.ZeroLengthRulesOK))
                    {
                        break;
                    }
                }
                if (tokenResult)
                {
                    docResult.Root.Add(tokenResult);
                    // if 0 length result move next.
                    token = (tokenResult.NextToken != startToken) ? tokenResult.NextToken : startToken.Next;
                }
                else
                {
                    docResult.PassedTokens.Add(token);
                    // no result then advanced to next token
                    token = token.Next;
                    
                }
            }

            return docResult;
        }



        public TokenParseResult ParseRule(Rule rule, Token startToken, ParserSettings settings)
        {

            TokenParseResult ruleResult = TokenParseResult.Failed(startToken);
            Token token = startToken;
            TokenParseResult result;
            List<SyntaxNode> nodes = new List<SyntaxNode>();

            token = startToken;
            do
            {
                result = rule.Parse(token, settings);
                if (result)
                {
                    nodes.Add(result.Node);
                    token = result.NextToken;
                }
                /* the while checks
                 * - the last result was good 
                 * - at least one node returned (indicative of an empty if 0)
                 * - haven't exceeded or equaled max value
                 * - tokens remain */
            } while (result && result.Node.Count > 0 && nodes.Count < rule.Quantifier.MaxValue && token != null);

            if (nodes.Count >= Math.Max(1, rule.Quantifier.MinValue))
            {
                if (nodes.Count > 1)
                {
                    SyntaxNode parent = new SyntaxNode(rule, startToken);
                    parent.AddRange(nodes);
                    ruleResult = TokenParseResult.Success(parent, token);
                }
                else
                {
                    ruleResult = TokenParseResult.Success(nodes[0], token);
                }
            }
            else if (result && nodes.Count == 0)
            {
                ruleResult = TokenParseResult.Success(new SyntaxNode(rule, token), result.NextToken);
            }
            return ruleResult;
        }


    }


}
