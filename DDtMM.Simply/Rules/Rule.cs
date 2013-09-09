using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{
    public abstract class Rule : IFormattable
    {
        public static implicit operator string(Rule rule)
        {
            return (rule != null) ? rule.Render() : "";
        }

        public Quantifier Quantifier { get; set; }
        public string ProductionName { get; set; }
        public Grouping Parent { get; set; }
        public bool IsException { get; set; }

        public Rule()
        {
            IsException = false;
            Quantifier = Quantifier.One;
            Parent = null;
            ProductionName = null;
        }

        public Rule(Rule copyFrom)
        {
            IsException = copyFrom.IsException;
            Quantifier = copyFrom.Quantifier;
            Parent = null;
            ProductionName = copyFrom.ProductionName;
        }

        /// <summary>
        /// Renders the part of the rule between exception char and quantifier 
        /// </summary>
        protected abstract string RenderRuleContents();
        public string Render()
        {
            return string.Format("{0}{1}{2:q}", IsException ? " - " : "", RenderRuleContents(), Quantifier);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ToString("n"), ToString("r"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">
        /// "n" production name or type name
        /// "r" uses render method, otherwise ToString() is returned
        /// "p" for path
        /// </param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(this.GetType())
                    as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            switch (format)
            {
                case "n":
                    return this.ProductionName ?? string.Format("({0})", this.GetType().Name + ")");
                case "r": 
                    return Render();
                case "p":
                    return string.Format(@"\{0:p}\{1}",
                        this.Parent,
                        this.ProductionName ?? string.Format("({0})", this.GetType().Name));
                default:
                    return this.ToString();
            }
        }

        /// <summary>
        /// Deep Copy
        /// </summary>
        /// <returns></returns>
        public abstract Rule DeepCopy();

        /// <summary>
        /// Change to abstract
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract TokenParseResult Parse(Token token, ParserSettings settings);
    }
}
