using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{

    public class Literal : Rule
    {
        private static readonly Regex quotedRegex = new Regex(@"^\s*'((?:\\\\|\\'|[^'])+)'|""((?:\\\\|\\""|[^""])+)""\s*$");

        /// <summary>
        /// Creates new literal rule from string in single quotes, otherwise returns null
        /// </summary>
        /// <param name="text">Any String</param>
        /// <returns></returns>
        public static Literal FromQuoted(string text)
        {
            Literal rule = null;
            Match match = quotedRegex.Match(text);
            if (match.Success)
            {
                string matchedText = match.Groups[1].Success ?
                    match.Groups[1].Value.Replace(@"\'", "'") :
                    match.Groups[2].Value.Replace(@"\""", "\"");
                rule = new Literal(matchedText);
            }
            return rule;
        }

        public string Text { get; set; }

        public Literal(string text) : base()
        {
            Text = text;
        }

        public Literal(Literal copyFrom)
            : base(copyFrom)
        {
            Text = copyFrom.Text;
        }

        protected override string RenderRuleContents()
        {

            return string.Format("\"{0}\"", Text.Replace("\"", @"\"""));
        }

        public override Rule DeepCopy()
        {
            return new Literal(this);
        }

        public override TokenParseResult Parse(Token token, ParserSettings settings)
        {
            TokenParseResult result;
            if (Text == token.Text)
            {
                result = TokenParseResult.Success(new SyntaxNode(this, token), token.Next);
            }
            else
            {
                result = TokenParseResult.Failed(token);
            }

            return result;
        } 
    }
}
