using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    public class ObservableParserNode : SyntaxNode
    {
        static new public SyntaxNode Create(Rule rule, Token token)
        {
            return new ObservableParserNode(rule, token);
        }

        public ObservableParserNode(Rule rule, Token token) :
            base(rule, token) { }

        public override void Add(SyntaxNode node) 
        {
            base.Add(node);
            ParserNodeReporter.Current.OnNodeAdded(this);
        }

        public override void Remove(SyntaxNode node)
        {
            ParserNodeReporter.Current.OnNodeRemoved(this);
            base.Remove(node);
        }

    }

    public class ParserNodeReporter
    {
        public static ParserNodeReporter Current = new ParserNodeReporter();

        public class ParserNodeEventArgs : EventArgs
        {
            public SyntaxNode Node;
        }

        public event EventHandler<ParserNodeEventArgs> NodeAdded;
        public event EventHandler<ParserNodeEventArgs> NodeRemoved;

        public void OnNodeAdded(ObservableParserNode node)
        {
            if (NodeAdded != null)
            {
                NodeAdded(this, new ParserNodeEventArgs() { Node = node });
            }
        }

        public void OnNodeRemoved(ObservableParserNode node)
        {
            if (NodeRemoved != null)
            {
                NodeRemoved(this, new ParserNodeEventArgs() { Node = node });
            }
        }

    }
}
