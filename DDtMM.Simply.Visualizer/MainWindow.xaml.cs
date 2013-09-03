using DDtMM.SIMPLY.Parsers;
using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using DDtMM.SIMPLY.Visualizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace DDtMM.SIMPLY.Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            


            ParserNodeReporter.Current.NodeAdded += Current_NodeAdded;
            ParserNodeReporter.Current.NodeRemoved += Current_NodeRemoved;
            ParserModel parserModel = new ParserModel(new Parser());
            parserModel.Grammar =  @"
#AlternateCritera = First
#RootProductionNames = stylesheet, statement
#ZeroLengthRulesOK = true

#TokenSubs
h			: [0-9a-f];
nonascii	: [\x80-\xff];
unicode		: \\{h}{1,6}[ \t\r\n\f]?;
escape		: {unicode}|\\[ -~\x80-\xff];
nmstart		: [a-z]|{nonascii}|{escape};
nmchar		: [a-z0-9\-]|{nonascii}|{escape};
string1		: ""([\t !\#$%&(-~]|\\{nl}|'|{nonascii}|{escape})*"";
string2		: '([\t !\#$%&(-~]|\\{nl}|""|{nonascii}|{escape})*';
ident		: \-?{nmstart}{nmchar}*;
name		: {nmchar}+;
num			: [0-9]+|[0-9]*\.[0-9]+;
string		: {string1}|{string2};
url			: ([!\#$%&*-~]|{nonascii}|{escape})*;
w			: [ \t\r\n\f]*;
nl			: \n|\r\n|\r|\f;
range		: \?{1,6}|{h}(\?{0,5}|{h}(\?{0,4}|{h}(\?{0,3}|{h}(\?{0,2}|{h}(\??|{h})))));


#Tokens
S				: [ \t\r\n\f]+;
-COMMENT		: /\*[^*]*\*+([^/][^*]*\*+)*/;
CDO				: <!--;
CDC				: -->;
INCLUDES		: ~=;
DASHMATCH		: \|=;
HASH			: \#{name};
IMPORT_SYM		: @import;
PAGE_SYM		: @page;
MEDIA_SYM		: @media;
FONT_FACE_SYM	: @font-face;
CHARSET_SYM		: @charset;
NAMESPACE_SYM	: @namespace;
IMPORTANT_SYM	: !{w}important;
EMS				: {num}em;
EXS				: {num}ex;
LENGTH			: {num}px;
LENGTH			: {num}cm;
LENGTH			: {num}mm;
LENGTH			: {num}in;
LENGTH			: {num}pt;
LENGTH			: {num}pc;
ANGLE			: {num}deg;
ANGLE			: {num}rad;
ANGLE			: {num}grad;
TIME			: {num}ms;
TIME			: {num}s;
FREQ			: {num}Hz;
FREQ			: {num}kHz;
DIMEN			: {num}{ident};
PERCENTAGE		: {num}%;
NUMBER			: {num};
URI				: url{w}\({w}{string}{w}\);
URI				: url{w}\({w}{url}{w}\);
FUNCTION		: {ident}\(;
UNICODERANGE	: U\+{range};
UNICODERANGE	: U\+{h}{1,6}-{h}{1,6};
STRING			: {string};
IDENT			: {ident};
yytext			: .;


#Productions
stylesheet
  : ( CHARSET_SYM S* STRING S* ';' )?
    (S|CDO|CDC)* ( import (S|CDO|CDC)* )*
    ( namespace (S|CDO|CDC)* )*
    ( ( ruleset | media | page | font_face ) (S|CDO|CDC)* )*
  ;
import
  : IMPORT_SYM S*
    (STRING|URI) S* ( medium ( ',' S* medium)* )? ';' S*
  ;
namespace
  : NAMESPACE_SYM S* (namespace_prefix S*)? (STRING|URI) S* ';' S*
  ;
namespace_prefix
  : IDENT
  ;
media
  : MEDIA_SYM S* medium ( ',' S* medium )* '{' S* ruleset* '}' S*
  ;
medium
  : IDENT S*
  ;
page
  : PAGE_SYM S* IDENT? pseudo_page? S*
    '{' S* declaration ( ';' S* declaration )* '}' S*
  ;
pseudo_page
  : ':' IDENT
  ;
font_face
  : FONT_FACE_SYM S*
    '{' S* declaration ( ';' S* declaration )* '}' S*
  ;
operator
  : '/' S* | ',' S* | /* empty */
  ;
combinator
  : '+' S* | '>' S* | /* empty */
  ;
unary_operator
  : '-' | '+'
  ;
property
  : IDENT S*
  ;
ruleset
  : selector ( ',' S* selector )*
    '{' S* declaration ( ';' S* declaration )* '}' S*
  ;
selector
  : simple_selector ( combinator simple_selector )*
  ;
simple_selector
  : element_name? ( HASH | class | attrib | pseudo )* S*
  ;
class
  : '.' IDENT
  ;
element_name
  : IDENT | '*'
  ;
attrib
  : '(' S* IDENT S* ( ( '=' | INCLUDES | DASHMATCH ) S*
    ( IDENT | STRING ) S* )? ')'
  ;
pseudo
  : ':' ( IDENT | FUNCTION S* IDENT S* ')' )
  ;
declaration
  : property ':' S* expr prio?
  | /* empty */
  ;
prio
  : IMPORTANT_SYM S*
  ;
expr
  : term ( operator term )*
  ;
term
  : unary_operator?
    ( NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* |
      TIME S* | FREQ S* | function )
  | STRING S* | IDENT S* | URI S* | UNICODERANGE S* | hexcolor
  ;
function
  : FUNCTION S* expr ')' S*
  ;
/*
 * There is a constraint on the color that it must
 * have either 3 or 6 hex-digits (i.e., [0-9a-fA-F))
 * after the #; e.g., #'000 is OK, but #abcd is not.
 */
hexcolor
  : HASH S*
  ;
";

            parserModel.Code = @"
b { background: url(img0) top }
b { background: url(""img1"") }
b { background: url('img2') }
b { background: url( img3 ) }
b { background: url( ""img4"" ) }
b { background: url( 'img5' ) }
b { background: url (img6) }
b { background: url (""img7"") }
b { background: url ('img8') }
{ background: url('noimg0) }
{ background: url(noimg1') }
/*b { background: url(noimg2) }*/
b { color: url(noimg3) }
b { content: 'url(noimg4)' }
@media screen and (max-width: 1280px) { b { background: url(img9) } }
b { background: url(img10) }
";

            DataContext = parserModel;

        }

        void Current_NodeRemoved(object sender, ParserNodeReporter.ParserNodeEventArgs e)
        {
            Console.WriteLine("Removed " + e.Node);
        }

        void Current_NodeAdded(object sender, ParserNodeReporter.ParserNodeEventArgs e)
        {
            Console.WriteLine("Added " + e.Node.StartToken);
        }

        private void BuildParserButton_Click(object sender, RoutedEventArgs e)
        {
            ((ParserModel)DataContext).BuildGrammar();
        }

        private void ParseCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ((ParserModel)DataContext).ParseCode();
        }

        
    }
}
