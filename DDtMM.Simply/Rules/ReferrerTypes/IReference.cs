using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules.ReferrerTypes
{
    public interface IReference
    {
        Referrer Source { get; }

        TokenParseResult ParseRule(Token token, ParserSettings settings);
    }
}
