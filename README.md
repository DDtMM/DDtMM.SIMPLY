DDtMM.SIMPLY
============

SIMPLY is a parser written in C# inspired by Regular Expressions, EBNF, and Flex.  The lexer relies on regular expressions to create a list of tokens from some text, and the grammar used by the pasrser works very much like a regular expression.

Document Syntax
-
The order in which items appear is not important.  They are case sensitive

#AlternateCritera = [ First | Longest ]
#RootProductionNames = [ str ( , str) * ] comma seperated list of product names, if ommmitted all products can appear at the root.
#ZeroLengthRulesOK = [ True | False ] Zero length rules are allowed and the token is incremented to next when encountered at the production level.

#TokenSubs
 List of substition expressions that can be used in a regular expression
[ substitution name } : [ ( regular expression | {substitution name} )* ] ;

#Tokens
 Tokens created by the Lexer.  A token that begins with "-" will not be sent to the parser.
 
 -?[ tokenname } : [ ( regular expression | {substitution name} )* ] ;

#Productions
 Grammar Rules go here.  Case sensitive.
 
 [ production name ] : [ rule ];
 
'''

Rule Syntax
-
'text' : Literal string
"text" : Literal string
|      : Alternation a | b => a or b
( )    : Concatenation ( a b ) => a then b
text   : Either a production name or token tame
+      : 1 or more quantifier.  a+ means at least 1 a
?      : optional quantifier.  a? means a zero or 1 times
*      : 0 or more quantifier.  a* means a zero or more times
{n, }  : at least n quantifer
{n,m}  : at least n, but not more than m quantifer.


Sample Grammar
-
The following is grammar for CSS Core Parser.

#TokenSubs
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

#Tokens
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

#Grammar
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
