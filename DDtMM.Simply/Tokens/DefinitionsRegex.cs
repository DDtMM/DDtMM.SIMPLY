using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Tokens
{
    public class DefinitionsRegex
    {
        public Regex Tokenizer;
        public Dictionary<int, string> GroupNames;

        public DefinitionsRegex(DefinitionCollection defs, RegexOptions regexOptions)
        {
            Tokenizer = new Regex(
                string.Join("|", defs
                    .Select(d => string.Format("(?<{0}>{1})", d.Name, d.Regex))),
                    regexOptions);

            GroupNames =
                  Tokenizer.GetGroupNames().Select((g, i) => new { Name = g, Index = i })
                      .Where(g => !Regex.Match(g.Name, @"^\d+$").Success)
                      .ToDictionary(g => g.Index, g => g.Name);
        }
    }
}
