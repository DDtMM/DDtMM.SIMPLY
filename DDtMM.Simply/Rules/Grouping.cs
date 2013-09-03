using DDtMM.SIMPLY.Rules.GroupingTypes;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Rules
{
    public class Grouping : Rule, IEnumerable<Rule>
    {
        private List<Rule> rules;

        public IGroupingType GroupingType { get; set; }
        public int Count { get { return rules.Count; } }
        public int LastRequiredRuleIndex { get; private set; }
        public bool IsEmpty { get { return rules.Count == 0; } }

        public Rule this[int index] 
        {
            get { return rules[index]; }
            set 
            {
                // unlink existing rule
                Rule oldRule = rules[index];
                if (oldRule != null) oldRule.Parent = null;

                rules[index] = value;
                if (value != null)
                {
                    if (value.Parent != null) value.Parent.Remove(value);
                    value.Parent = this;
                }
                DetermineLastRequiredRuleIndex();
            }
        }

        public Grouping(IGroupingType groupingType) 
            : base()
        {
            rules = new List<Rule>();
            DetermineLastRequiredRuleIndex();
            GroupingType = groupingType;
        }

        public Grouping(Grouping copyFrom)
            : base(copyFrom)
        {
            rules = new List<Rule>();
            copyFrom.rules.ForEach(r => Add(r.DeepCopy()));
            DetermineLastRequiredRuleIndex();
            GroupingType = copyFrom.GroupingType;
        }

        public void Add(Rule rule)
        {
            if (rule.Parent != null)
            {
                rule.Parent.Remove(rule);
            }
            rules.Add(rule);
            rule.Parent = this;
            DetermineLastRequiredRuleIndex();
        }

        public void Remove(Rule rule)
        {
            rules.Remove(rule);
            rule.Parent = null;
            DetermineLastRequiredRuleIndex();
        }

        /// <summary>
        /// Removes element at index, severing link to this.
        /// </summary>
        /// <param name="index">index where rule is at</param>
        /// <returns>Removed rule</returns>
        public Rule RemoveAt(int index)
        {
            Rule rule = rules[index];
            if (rule != null) rule.Parent = null;
            rules.RemoveAt(index);
            DetermineLastRequiredRuleIndex();
            return rule;
        }

        /// <summary>
        /// Replaces rule with replacement.  If rule does not exist no action is taken.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="replacement"></param>
        public void Replace(Rule rule, Rule replacement)
        {
            int index = rules.IndexOf(rule);
            if (index >= 0)
            {
                rules[index] = replacement;
            }
        }

        protected override string RenderRuleContents()
        {
            string template = ((Parent == null && Quantifier == Quantifier.One) 
                || (Parent != null && Parent.GroupingType is Alternation && Quantifier == Quantifier.One)) ?
                 "{0}" : "( {0} )"  ;
            return string.Format(template, string.Join(GroupingType.Delimiter, rules.Select(rg => rg.Render())));
        }

        public override Rule DeepCopy()
        {
            return new Grouping(this);
        }

        public override TokenParseResult Parse(Token token, ParserSettings settings)
        {
            return GroupingType.ParseRule(this, token, settings);
        }
        public IEnumerator<Rule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Determines the last required rule by checking each element from the end of
        /// the rules list until a non options rule is found.
        /// </summary>
        private void DetermineLastRequiredRuleIndex()
        {
            int idx = rules.Count - 1;
            while (idx >= 0 && rules[idx].Quantifier.IsOptional)
            {
                idx--;
            }
            LastRequiredRuleIndex = idx;
        }
    }
}
