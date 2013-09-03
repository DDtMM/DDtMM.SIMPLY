using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.GroupingTypes
{
    public interface IGroupingType
    {
        string Delimiter { get; }

        TokenParseResult ParseRule(Grouping group, Token startToken, ParserSettings settings);
    }
}
