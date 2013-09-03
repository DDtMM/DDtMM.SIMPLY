using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDtMM.SIMPLY.Tokens;
using System.Text.RegularExpressions;
namespace DDtMM.SIMPLY
{
    public class ParserDocument
    {
        static public Parser documentParser;

        static ParserDocument()
        {
            documentParser = new Parser();

            documentParser.Lexer = new Lexer();
            documentParser.Lexer.RegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            documentParser.Lexer.Substitutions = DefinitionCollection.Parse(@"
noteol: [^\r\n];
eol: \r\n|\n\r|\r|\n;
");
            documentParser.Lexer.Modes.Add("default", DefinitionCollection.Parse(@"
-COMMENT: /\*[^*]*\*+(?:[^/][^*]*\*+)*/ ;
NAME : ^#[a-zA-Z]\w* ;
LONGVAL : (?:^[^#]{noteol}*{eol})+ ;
S: [ \t]+;
SPECIALCHAR: [.=] ;
SHORTVAL: {noteol}+{eol} ;
"));
            documentParser.Grammar = Grammar.Parse(@"
section      : name ( value | S* '=' S* value ) ;
name         : NAME;
value        : LONGVAL | SHORTVAL;
");
        }

        static public Parser CreateParser(string text)
        {
            ParserResult result = documentParser.Parse(text);
            SyntaxNode root = result.Root.ReduceToProductionNames().RemoveWhitespaceOnlyNodes();
            Parser parser = new Parser();

            foreach (SyntaxNode node in root)
            {
                ProcessSection(node, parser);
            }

            return parser;
        }

        static private void ProcessSection(SyntaxNode section, Parser parser) 
        {
            if (section.Rule.ProductionName != "section") throw InvalidTokenException.Create(section, "section");

            string name = null;
            string value = null;

            foreach (SyntaxNode node in section)
            {
                switch (node.Rule.ProductionName)
                {
                    case "name":
                        name = node.StartToken.Text;
                        break;
                    case "value":
                        value = node.StartToken.Text;
                        break;
                }
            }

            switch (name)
            {
                case "#AlternateCritera":
                    parser.Settings.AlternateCritera = (ParserSettings.SelectionCriteria)
                        Enum.Parse(typeof(ParserSettings.SelectionCriteria), value);
                    break;
                case "#RootProductionNames":
                    parser.Settings.RootProductionNames = new List<string>(
                        Regex.Split(value, @"\s*,\s*"));
                    break;
                case "#ZeroLengthRulesOK":
                    parser.Settings.ZeroLengthRulesOK = bool.Parse(value);
                    break;
                case "#TokenSubs":
                    parser.Lexer.Substitutions = DefinitionCollection.Parse(value);
                    break;
                case "#Tokens":
                    parser.Lexer.Modes.Add("default", DefinitionCollection.Parse(value));
                    break;
                case "#Productions":
                    parser.Grammar = Grammar.Parse(value);
                    break;
            }
        }


    }
}
