using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Rules.GroupingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class Productions : List<Rule>
    {
        static public Productions Parse(string text)
        {
            Productions g = new Productions();
            g.ParseProductions(text);
            return g;
        }

        /// <summary>
        /// Group 1: words
        /// Group 2: single quote
        /// Group 3: Groups and alternators
        /// Group 4: Quantifier
        /// Group 5: Ignorables
        /// Group 6: invalid
        /// </summary>
        private Regex ruleTokenizer;
        private Regex grammarTokenizer;

        public Productions()
        {
            grammarTokenizer = new Regex(@"\s*(\w+)\s*:s*((?:'(?:\\\\|\\'|\\|[^'\\])+'|""(?:\\\\|\\""|\\|[^""\\])+""|[^;])+);",
                RegexOptions.Multiline);

            ruleTokenizer = new Regex(@"
(\w+)|
('(?:\\\\|\\'|\\|[^'\\])+'|""(?:\\\\|\\""|\\|[^""\\])+"")|
([|()\-])|
(:?[*?+]|\{\d+(?:,\d*)?\})|
(/\*[^*]*\*+(?:[^/][^*]*\*+)*/|\s+)|
(.)", RegexOptions.IgnorePatternWhitespace);
        }

        public void ParseProductions(string text)
        {
            text = Common.StripComments(text);
            foreach (Match match in grammarTokenizer.Matches(text)) 
            {
                AddRule(match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <remarks>
        /// When parsing text, it is always assumed as the start that the rule is an
        /// alternating group of groups, and when a grouping token is encountered the same
        /// is assumed.  The simplify method is called at the end
        /// </remarks>
        public void AddRule(string name, string text)
        {
            int matchedGroup;
            Grouping currentAlternation = new Grouping(Alternation.Instance) { ProductionName = name };
            Grouping currentGroup = new Grouping(Concatenation.Instance);
            currentAlternation.Add(currentGroup);

            foreach (Match match in ruleTokenizer.Matches(text))
            {
                matchedGroup = match.Groups.Cast<System.Text.RegularExpressions.Group>()
                    .Select((g, i) => new { Group = g, Index = i })
                    .FirstOrDefault(g => g.Group.Success && g.Index > 0).Index;

                switch (matchedGroup)
                {
                    case 1:
                        currentGroup.Add(new Referrer(match.Value));
                        break;
                    case 2:
                        currentGroup.Add(Literal.FromQuoted(match.Value));
                        break;
                    case 3:
                        switch (match.Value)
                        {
                            case "(":
                                currentAlternation = new Grouping(Alternation.Instance);
                                currentGroup.Add(currentAlternation);
                                currentGroup = new Grouping(Concatenation.Instance);
                                currentAlternation.Add(currentGroup);
                                break;
                            case ")":
                                if (currentAlternation.IsException)
                                {
                                    currentGroup = currentAlternation.Parent;
                                    currentAlternation = currentGroup.Parent;
                                }
                                currentGroup = currentAlternation.Parent;
                                currentAlternation = currentGroup.Parent;
                                break;
                            case "|":
                                if (currentAlternation.IsException)
                                {
                                    currentGroup = currentAlternation.Parent;
                                    currentAlternation = currentGroup.Parent;
                                }
                                currentGroup = new Grouping(Concatenation.Instance);
                                currentAlternation.Add(currentGroup);
                                break;
                            case "-":
                                currentAlternation = new Grouping(Alternation.Instance) { IsException = true };
                                currentGroup.Add(currentAlternation);
                                currentGroup = new Grouping(Concatenation.Instance);
                                currentAlternation.Add(currentGroup);
                                break;
                        }
                        break;
                    case 4:
                        currentGroup.Last().Quantifier = match.Value;
                        break;
                    case 6:
                        throw new Exception(String.Format("Invalid token \"{0}\" at {1}", match.Value,
                            match.Index));

                }
            }

            Add(Simplify(currentAlternation));
        }

        /// <summary>
        /// Removes empty Groupings and replaces single element groupings
        /// with their child elements.
        /// </summary>
        /// <returns>This, or if there is only 1 other element, that element
        /// with its link to this removed and made to this's parent</returns>
        public Rule Simplify(Grouping group)
        {
            Rule rule;
            Rule simplified = null;
            // simplify child groups.
            for (int i = group.Count - 1; i >= 0; i--)
            {
                rule = group[i];
                if (rule is Grouping)
                {
                    Simplify((Grouping)rule);
                }
            }

            if (group.Count == 1 && group.Quantifier == group[0].Quantifier)
            {
                simplified = group.RemoveAt(0);
                
            }
            else if (group.Count == 0)
            {
                simplified = new Empty();
            }

            if (simplified != null)
            {
                simplified.ProductionName = group.ProductionName;
                simplified.IsException = group.IsException;

                if (group.Parent != null)
                {
                    group.Parent.Replace(group, simplified);
                }
                else
                {
                    simplified.Parent = null;
                }
                return simplified;
            }

            return group;
        }

        public Rule GetRule(string name)
        {
            return this.FirstOrDefault(r => r.ProductionName == name);
        }


        /// <summary>
        /// Creates a new grammar, replacing unknownTypeTokens with either rule types or token types
        /// </summary>
        public Productions Compile(Lexer t)
        {
            Productions compiled = new Productions();
            compiled.AddRange(this.Select(r => r.DeepCopy()));
            Compile(compiled, compiled, t);
            return compiled;
        }

        private void Compile(IEnumerable<Rule> rules, Productions g, Lexer t)
        {
            foreach (Rule rule in rules)
            {
                if (rule is Grouping) 
                {
                    Compile((Grouping)rule, g, t);
                }
                else if (rule is Referrer)
                {
                    ((Referrer)rule).CreateReference(g, t);
                }
            }
        }
        
    }





}
