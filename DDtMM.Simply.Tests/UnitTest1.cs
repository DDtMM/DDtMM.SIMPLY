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
                sb.Append(string.Format("{0}: {1};\n", kvp.Key, kvp.Value));
            }
            string text = sb.ToString();
            
            List<string> ruleGrammar = productions.Values.ToList();

            Productions grammar = Productions.Parse(text);

            Assert.AreEqual(productions.Count, grammar.Count);
    
            for (int i = 0; i < productions.Count; i++)
            {
                Assert.AreEqual(ruleGrammar[i], grammar[i].ToString("r"));
            }
        }
        [TestMethod]
        public void TestGrammarComments()
        {
            Productions commentLaden = Productions.Parse(@"
/* comment /* comment */rule/*comm:nent  */:a? ( /*coomnet*/b | c/*comm)ent*/);");
            Productions simple = Productions.Parse("rule : a? ( b | c );");
            Assert.AreEqual(commentLaden[0].ToString(), simple[0].ToString());
        }

    }
}
