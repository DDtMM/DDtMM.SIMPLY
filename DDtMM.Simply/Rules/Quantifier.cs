using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{
    public struct Quantifier: IFormattable
    {
        public static readonly Quantifier One = new Quantifier(1, 1);
        public static readonly Quantifier ZeroOrMore = new Quantifier(0, int.MaxValue);
        public static readonly Quantifier OneOrMore = new Quantifier(1, int.MaxValue);
        public static readonly Quantifier ZeroOrOne = new Quantifier(0, 1);

        private static Dictionary<Quantifier, string> conversions;
        private static readonly Regex parseRegex;

        static Quantifier()
        {
            parseRegex = new Regex(@"^\s*\{\s*(\d+)\s*(?:(,)\s*(\d+))?\s*\}\s*$",
                RegexOptions.Multiline);

            conversions = new Dictionary<Quantifier, string>();
            conversions.Add(One, "");
            conversions.Add(ZeroOrMore, "*");
            conversions.Add(OneOrMore, "+");
            conversions.Add(ZeroOrOne, "?");

        }

        public static implicit operator Quantifier(string str)
        {
            return Parse(str);
        }

        public static implicit operator string(Quantifier qty)
        {
            return qty.ToString("q");
        }

        public static Quantifier Parse(string str)
        {
            if (str == null || conversions.ContainsValue(str.Trim()))
            {
                return conversions.First(q => q.Value == str).Key;
            }
            Match match = parseRegex.Match(str);
            if (match.Success)
            {
                int minValue = int.Parse(match.Groups[1].Value);
                int maxValue = (!match.Groups[2].Success) ? minValue :
                        (match.Groups[3].Success) ? int.Parse(match.Groups[3].Value) : int.MaxValue;
                return new Quantifier(minValue, maxValue);
            }

            throw new Exception(string.Format("Unable to convert {0} to Quantifier.", str));
        }


        private int minValue;
        public int MinValue 
        {
            get { return minValue; }
            set
            {
                if (value < 0 || value > maxValue) 
                    throw new Exception("MinValue can not be negative or greater than MaxValue.");

                minValue = value;
            }
        }

        private int maxValue;
        public int MaxValue 
        {
            get { return maxValue; }
            set
            {
                if (value < 1 || value < minValue)
                    throw new Exception("MaxValue can not be less than 1 or MinValue");

                maxValue = value;
            }
        }
        
        public bool IsOptional { get { return MinValue == 0; } }

        public Quantifier(int minValue, int maxValue)
            : this()
        {

            MaxValue = maxValue;
            MinValue = minValue;
        }

        public override string ToString()
        {
            return "(Quantifier): " + ToString("q");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">"q" for just the quantifier, 
        /// anything else returns ToString() implementation</param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat( this.GetType())
                    as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            switch (format)
            {
                case "q": 
                    return ((conversions.ContainsKey(this)) ? conversions[this] :
                        string.Format("{{{0},{1}}}", MinValue, MaxValue));
                default: 
                    return this.ToString();
            }
        }
    }
}
