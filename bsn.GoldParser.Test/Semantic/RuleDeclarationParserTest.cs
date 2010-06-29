using System;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleDeclarationParserTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new RuleDeclarationParser(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullRuleString() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			parser.TryParse(null, out rule);
		}

		[Test]
		public void ValidRuleStringComplex() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Value> ::= '(' <Expression> ')'", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleStringEmpty() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Empty> ::=", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleStringSimple() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Value> ::= Float", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}
	}
}
