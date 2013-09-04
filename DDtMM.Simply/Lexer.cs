using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class Lexer
    {
        /// <summary>
        /// Replacements for keywords in all modes
        /// </summary>
        public DefinitionCollection Substitutions { get; set; }

        public Dictionary<string, DefinitionCollection> Modes { get; set; }

        public string DefaultMode { get; set; }

        public RegexOptions RegexOptions { get; set; }

        public Lexer()
        {
            Substitutions = new DefinitionCollection();
            Modes = new Dictionary<string, DefinitionCollection>();
            RegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
        }


        public List<Token> Tokenize(string str)
        {
            DefinitionCollection compiledSubs = Substitutions.Compile(RegexOptions);
            
            Dictionary<string, DefinitionCollection> compiledModes = new Dictionary<string, DefinitionCollection>();
 
            foreach (var mode in Modes)
            {
                compiledModes.Add(mode.Key, mode.Value.Compile(RegexOptions, compiledSubs));
            }
            
            List<Token> tokens = new List<Token>();
            Token currentToken;
            Token lastToken = new Token(null, null, -1);
            Match match;
            int strLength = str.Length;
            int currentIndex = 0;
            string defaultMode = DefaultMode ?? compiledModes.First().Key;
            DefinitionCollection currentMode = compiledModes[defaultMode];

            while (strLength >= currentIndex && (match = currentMode.RegexInfo.Tokenizer.Match(str, currentIndex)).Success)
            {
                currentToken = match.Groups.Cast<Group>()
                    .Select((g, i) => new { Group = g, Index = i })
                    .Where((g => g.Group.Success && currentMode.RegexInfo.GroupNames.ContainsKey(g.Index)))
                    .Select(g => new Token(currentMode[currentMode.RegexInfo.GroupNames[g.Index]], g.Group.Value, match.Index))
                    .FirstOrDefault();

                if (!currentToken.TokenType.IsIgnored)
                {
                    lastToken.Next = currentToken;
                    tokens.Add(currentToken);
                    lastToken = currentToken;
                }

                currentIndex = match.Index + match.Length;
            }

            return tokens;
        }


        public TokenType FindTokenType(string typeName)
        {
            int foundIndex = -1;
            foreach (var mode in Modes)
            {
                if ((foundIndex = mode.Value.IndexOfKey(typeName)) >= 0) return mode.Value[foundIndex];
            }
            return null;
        }
    }

 
 

}
