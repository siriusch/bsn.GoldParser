using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class TerminalAttributeTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void BindToGrammar() {
			Expect(new TerminalAttribute("Integer").Bind(grammar), Not.Null);
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new TerminalAttribute(null);
		}
	}
}
