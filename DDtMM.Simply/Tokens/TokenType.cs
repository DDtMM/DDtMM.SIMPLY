using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Tokens
{
    public class TokenType : IEquatable<TokenType>
    {
        static private readonly Regex defRegex = new Regex(@"\s*(-)?([a-z]\w*)\s*:\s*(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Creates token definition from text.  Assumes no comments in text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static public TokenType Parse(string text)
        {
            text = Common.UnescapeLineTerminators(Common.StripComments(text));
            Match match = defRegex.Match(text);
            TokenType def = null;
            if (match.Success)
            {
                def = new TokenType(match.Groups[2].Value, match.Groups[3].Value.Trim()
                    , match.Groups[1].Success);
            }
            else
            {
                throw new Exception(string.Format("Unable to parse: {0} ", text));
            }
            return def;
        }

        public string Name { get; set; }
        public string Regex { get; set; }
        public bool IsIgnored { get; set; }

        public TokenType(string name, string regex, bool isIgnored)
        {
            Name = name;
            Regex = regex;
            IsIgnored = isIgnored;
        }

        public TokenType Copy()
        {
            return new TokenType(Name, Regex, IsIgnored);
        }

        public bool Equals(TokenType other)
        {
            return other != null && other.Name == Name;
        }
    }
}
