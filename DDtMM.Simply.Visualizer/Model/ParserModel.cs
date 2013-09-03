using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System.ComponentModel;

namespace DDtMM.SIMPLY.Visualizer.Model
{
    public class ParserModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public ObservableCollection<Rule> Rules 
        {
            get { return propmgr.Get<ObservableCollection<Rule>>("Rules"); }
            set { propmgr.Set("Rules", value); }
        }
        public ObservableCollection<TokenType> TokenDefinitions
        {
            get { return propmgr.Get<ObservableCollection<TokenType>>("TokenDefinitions"); }
            set { propmgr.Set("TokenDefinitions", value); }
        }
        public ObservableCollection<TokenType> TokenDefinitionsCompiled
        {
            get { return propmgr.Get<ObservableCollection<TokenType>>("TokenDefinitionsCompiled"); }
            set { propmgr.Set("TokenDefinitionsCompiled", value); }
        }
        public ObservableCollection<Token> Tokenized
        {
            get { return propmgr.Get<ObservableCollection<Token>>("Tokenized"); }
            set { propmgr.Set("Tokenized", value); }
        }
        public ParserNodeModel ParseTree
        {
            get { return propmgr.Get<ParserNodeModel>("ParseTree"); }
            set { propmgr.Set("ParseTree", value); }
        }
        public string Grammar
        {
            get { return propmgr.Get<string>("Grammar"); }
            set { propmgr.Set("Grammar", value); }
        }
        public string Code
        {
            get { return propmgr.Get<string>("Code"); }
            set { propmgr.Set("Code", value); }
        }
        public Exception LastException
        {
            get { return propmgr.Get<Exception>("LastException"); }
            set { propmgr.Set("LastException", value); }
        }
        public Parser Parser
        {
            get { return propmgr.Get<Parser>("Parser"); }
            set
            {
                propmgr.Set("Parser", value);
                if (value.Lexer.Modes.Count > 0)
                {
                    TokenDefinitions = new ObservableCollection<TokenType>(
                        value.Lexer.Modes.First().Value);
                    TokenDefinitionsCompiled = new ObservableCollection<TokenType>(
                        value.Lexer.Modes.First().Value.Compile(
                        value.Lexer.RegexOptions,
                        value.Lexer.Substitutions.Compile(value.Lexer.RegexOptions)));
                    Rules = new ObservableCollection<Rule>(value.Grammar.Compile(value.Lexer).Productions);
                }
                else
                {
                    TokenDefinitions = new ObservableCollection<TokenType>();
                    TokenDefinitionsCompiled = new ObservableCollection<TokenType>();
                    Rules = new ObservableCollection<Rule>();
                }
                Tokenized = new ObservableCollection<Token>();
                ParseTree = new ParserNodeModel(new SyntaxNode(null, null));
            }
        }
        private PropertyManager propmgr;

        public ParserModel(Parser parser)
        {
            propmgr = new PropertyManager(this);
            Parser = parser;
            Grammar = "";
            Code = "";
        }

        public void BuildGrammar()
        {
            try
            {
                Parser = ParserDocument.CreateParser(Grammar);
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
        }

        public void ParseCode()
        {
            try
            {
                List<Token> tokens = Parser.Lexer.Tokenize(Code);
                Tokenized = new ObservableCollection<Token>(tokens);
                ParseTree = new ParserNodeModel(Parser.Parse(tokens).Root.ReduceToProductionNames());
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
        }



        
    }
}
