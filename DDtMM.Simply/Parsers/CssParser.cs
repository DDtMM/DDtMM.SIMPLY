using DDtMM.SIMPLY;
using DDtMM.SIMPLY.Rules;
using DDtMM.SIMPLY.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY.Parsers
{
    public class CssParser : Parser
    {
        public CssParser()
        {
            Lexer = new Lexer();

            Lexer.Substitutions = DefinitionCollection.Parse(@"
ident 		= 	-? {nmstart} {nmchar}*;
name 		= 	{nmchar}+;
nmstart 	= 	[a-zA-Z] | _ | {nonascii} | {escape};
nonascii 	= 	[\x80-\uD7FF\uE000-\uFFFD];
unicode 	= 	\\ [0-9a-fA-F]{1,6} {wc}?;
escape 		= 	{unicode} | \\ [\x20-\x7E\x80-\uD7FF\uE000-\uFFFD];
nmchar 		= 	[a-zA-Z0-9] | - | _ | {nonascii} | {escape};
num 		= 	[0-9]+ | [0-9]* \. [0-9]+;
string 		= 	"" ({stringchar} | ')* "" | ' ({stringchar} | "")* ';
stringchar 	= 	{urlchar} | \x020 | \\ {nl};
urlchar 	= 	[\x09\x21\x23-\x26\x27-\x7E] | {nonascii} | {escape};
nl 			= 	\x0A | \x0D \x0A | \x0D | \x0C;
w 			= 	{wc}*;
wc 			= 	\x09 | \x0A | \x0C | \x0D | \x20;
");

            Lexer.Modes.Add("default", DefinitionCollection.Parse(@"
HASH    	    = 	\# {name};
PERCENTAGE 	    = 	{num} %;
DIMENSION 	    = 	{num} {ident};
NUMBER  	    = 	{num};
URI 	        = 	url {w} \( {w} ({string} | {urlchar}* ) {w} \);
UNICODE_RANGE 	= 	U\+ [0-9A-F?]{1,6} (- [0-9A-F]{1,6})?;
CDO 	        = 	<!--;
CDC 	        = 	-->;
S 	            = 	{wc}+;
COMMENT     	= 	/\* [^*]* \*+ ([^/] [^*]* \*+)* /;
INCLUDES 	    = 	~=;
DASHMATCH 	    = 	\|=;
PREFIXMATCH 	= 	\^=;
SUFFIXMATCH 	= 	\$=;
SUBSTRINGMATCH 	= 	\*=;
ATKEYWORD 	    = 	@ {ident};
FUNCTION 	    = 	{ident} \(;
IDENT 	        = 	{ident};
STRING 	        = 	{string};
CHAR 	        = 	[^""'];
BOM 	        = 	\uFEFF;
"));

            Grammar = Grammar.Parse(
@"stylesheet= ( CDO | CDC | S | statement )*;
statement   = ruleset | at_rule;
/* AT RULE kept getting skipped
at_rule     = ATKEYWORD S* ( any | '(' declaration ')' )* S* ( block | ';' S* );
block       = '{' S* statement* '}' S*;
ruleset     = selector? '{' S* declaration? ( ';' S* declaration? )* '}' S*;
selector    = any+;
declaration = property ':' S* value;
property    = IDENT S*;
value       = ( ( any | block | ATKEYWORD S* )+;
any         = ( IDENT | NUMBER | PERCENTAGE | DIMENSION | STRING
              | URI | HASH | UNICODE_RANGE | INCLUDES
              | FUNCTION S* any* ')' | DASHMATCH | '(' S* any* ')'
              | '(' S* any* ')' ) S*;");
        }

        public override ParserResult Parse(List<Token> tokens)
        {
            ParserResult docResult = base.Parse(tokens);


            return docResult;
        }

    }
}
