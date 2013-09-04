using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class SyntaxNode : IEnumerable<SyntaxNode>
    {
        static public SyntaxNode Create(Rule rule, Token token)
        {
            return new SyntaxNode(rule, token);
        }

        public int Count
        {
            get { return nodes.Count; }
        }

        public SyntaxNode this[int index]
        {
            get { return nodes[index]; }
            set { nodes[index] = value; }
        }

        public Rule Rule { get; set; }
        public Token StartToken { get; set; }
        public SyntaxNode Parent { get; private set; }

        /// <summary>
        /// base list of nodes
        /// </summary>
        private List<SyntaxNode> nodes;

        public SyntaxNode(Rule rule, Token startToken)
        {
            Rule = rule;
            StartToken = startToken;
            nodes = new List<SyntaxNode>();
        }

        public virtual void Add(SyntaxNode node)
        {
            if (node.Parent != null) node.Parent.Remove(this);
            node.Parent = this;
            nodes.Add(node);
        }

        public void AddRange(IEnumerable<SyntaxNode> nodes)
        {
            foreach (SyntaxNode node in nodes)
            {
                Add(node);
            }
        }

        public virtual void Remove(SyntaxNode node)
        {
            if (node.Parent == this)
            {
                nodes.Remove(node);
                node.Parent = null;
            }
        }

        /// <summary>
        /// Removes self from parent
        /// </summary>
        public void Remove()
        {
            this.Parent.Remove(this);
        }

        public virtual void RemoveAt(int index)
        {
            nodes[index].Parent = null;
            nodes.RemoveAt(index);
        }


        public void Insert(int index, SyntaxNode node)
        {
            if (node.Parent != null) node.Parent.Remove(this);
            nodes.Insert(index, node);
        }

        public void ForEach(Action<SyntaxNode> a)
        {
            nodes.ForEach(n => a(n));
        }
        public IEnumerator<SyntaxNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        public Token GetEndToken()
        {
            if (nodes.Count > 0)
            {
                return nodes.Last().GetEndToken();
            }
            return StartToken;
        }

        public int GetTokenCount() 
        {
            Token endToken = GetEndToken();
            return (endToken != null) ? endToken.Index - StartToken.Index + 1 : 0;
        }

        public SyntaxNode ReduceToProductionNames()
        {
            SyntaxNode node;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                node = nodes[i];
                node.ReduceToProductionNames();
                if (node.Rule.ProductionName == null)
                {
                    this.RemoveAt(i);
                    for (int j = node.Count - 1; j >= 0; j--)
                    {
                        this.Insert(i, node[j]);
                    }
                }
            }
            return this;
        }

        public SyntaxNode RemoveWhitespaceOnlyNodes()
        {
            SyntaxNode node;
            for (int i = Count - 1; i >= 0; i--)
            {
                node = this[i];
                node.RemoveWhitespaceOnlyNodes();
                if (node.Count == 0 && (node.StartToken == null || String.IsNullOrWhiteSpace(node.StartToken.Text)))
                {
                    this.RemoveAt(i);
                }
            }
            return this;
        }


    }
}
