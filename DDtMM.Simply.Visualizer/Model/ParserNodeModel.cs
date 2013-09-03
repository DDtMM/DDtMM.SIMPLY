using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Collections.ObjectModel;

namespace DDtMM.SIMPLY.Visualizer.Model
{
    public class ParserNodeModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public bool IsExpanded
        {
            get { return propmgr.Get<bool>("IsExpanded");  }
            set { propmgr.Set("IsExpanded", value); }
        }

        public Rule Rule {
            get { return propmgr.Get<Rule>("Rule"); }
            set { propmgr.Set("Rule", value); }
        }

        public Token Token
        {
            get { return propmgr.Get<Token>("Token"); }
            set { propmgr.Set("Token", value); }
        }

        
        public ObservableCollection<ParserNodeModel> Children { get; set; }

        private PropertyManager propmgr;

        public ParserNodeModel(SyntaxNode node)
        {
            propmgr = new PropertyManager(this);
            Rule = node.Rule;
            Token = node.StartToken;
            Children = new ObservableCollection<ParserNodeModel>();
            node.ForEach(n => Children.Add(new ParserNodeModel(n)));
        }

        void ParserNodeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
        }


        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (ParserNodeModel child in Children)
            {
                child.ExpandAll();
            }
        }

       
    }
}
