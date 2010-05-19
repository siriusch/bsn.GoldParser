using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class TerminalAttributeTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new TerminalAttribute(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithEmptyString() {
			new TerminalAttribute(string.Empty);
		}

		[Test]
		public void ConstructWithString() {
			new TerminalAttribute("Integer");
		}

		[Test]
		public void BindToGrammar() {
			Expect(new TerminalAttribute("Integer").Bind(CompiledGrammarTest.LoadTestGrammar()), Not.Null);
		}
	}
}
