using DDtMM.SIMPLY.Rules.ReferrerTypes;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{
    public class Referrer : Rule
    {
        public string Symbol;
        public IReference RefersTo;

        public Referrer(string symbol) 
            : base()
        {
            Symbol = symbol;
        }

        public Referrer(Referrer copyFrom)
            : base(copyFrom)
        {
            Symbol = copyFrom.Symbol;
        }

        protected override string RenderRuleContents()
        {
            return string.Format("{0}", Symbol);
        }

        
        /// <summary>
        /// Finds referred symbol in grammar and tokenizer
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="lexer"></param>
        public void CreateReference(Productions grammar, Lexer lexer)
        {
            object referredTo;

            if ((referredTo = grammar.GetRule(Symbol)) != null)
            {
                RefersTo = new RuleReference((Rule)referredTo, this);
            }
            else if ((referredTo = lexer.FindTokenType(Symbol)) != null)
            {
                RefersTo = new TokenReference((TokenType)referredTo, this);
            }
        }

        /// <summary>
        /// Does not copy reference,
        /// </summary>
        /// <returns></returns>
        public override Rule DeepCopy()
        {
            return new Referrer(this);
        }

        public override TokenParseResult Parse(Token token, ParserSettings settings)
        {
            return RefersTo.ParseRule(token, settings);
        }
    }
}
