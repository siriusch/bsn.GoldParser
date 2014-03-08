using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;

using Xunit;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Xml {
	public class TokenXmlReaderTest {
		private static readonly CompiledGrammar grammar = CompiledGrammar.Load(typeof(EgtCompiledGrammarTest), "TestGrammar.egt");

		[Fact]
		public void SmokeTest() {
			LalrProcessor processor = new LalrProcessor(new Tokenizer(new StringReader("((100+5.0)/\r\n(4.5+.5))-\r\n12345.4e+1"), grammar), true);
			Assert.Equal(ParseMessage.Accept, processor.ParseAll());
			XPathDocument doc;
			using (TokenXmlReader reader = new TokenXmlReader(null, processor.CurrentToken)) {
				doc = new XPathDocument(reader);
			}
			Trace.Write("XML: " + doc.CreateNavigator().OuterXml);
		}
	}
}
