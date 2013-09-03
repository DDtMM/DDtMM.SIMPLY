using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using DDtMM.SIMPLY;

namespace DDtMM.SIMPLY.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleGrammarTest()
        {
            Dictionary<string, string> productions = new Dictionary<string, string>();
            productions.Add("simple", @"a b | c");
            productions.Add("prod1", @"a? ( b | c+ d{2,3} )*");

            StringBuilder sb = new StringBuilder();
            foreach (var kvp in productions)
            {
                sb.Append(string.Format("{0}= {1};\n", kvp.Key, kvp.Value));
            }
            string text = sb.ToString();
            
            List<string> ruleGrammar = productions.Values.ToList();

            Grammar grammar = Grammar.Parse(text);

            Assert.AreEqual(productions.Count, grammar.Productions.Count);
    
            for (int i = 0; i < productions.Count; i++)
            {
                Assert.AreEqual(ruleGrammar[i], grammar.Productions[i].ToString("r"));
            }
        }
        [TestMethod]
        public void TestGrammarComments()
        {
            Grammar commentLaden = Grammar.Parse(@"
/* comment /* comment */rule/*comm=nent  */=a? ( /*coomnet*/b | c/*comm)ent*/);");
            Grammar simple = Grammar.Parse("rule = a? ( b | c );");
            Assert.AreEqual(commentLaden.Productions[0].ToString(), simple.Productions[0].ToString());
        }
        //[TestMethod]
        //public void TestCss3Parser()
        //{
        //    Css3Parser parser = new Css3Parser();
        //    List<Tokens.Token> tokens = parser.Lexer.Tokenize("b { abc: 123; }");
        //    parser.Grammar = parser.Grammar.Compile(parser.Lexer);
        //    TokenParseResult result = parser.ParseRule(parser.Grammar.Productions.First(r => r.ProductionName == "selector"), tokens[0]);
        //    Assert.IsTrue(result.IsSuccesful);
        //    Assert.AreEqual("selector", result.Node.Rule.ProductionName);
        //}
    }
}
