using System;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleDeclarationParserTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new RuleDeclarationParser(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullRuleString() {
			RuleDeclarationParser parser = new RuleDeclarationParser(CompiledGrammarTest.LoadTestGrammar());
			Rule rule;
			parser.TryParse(null, out rule);
		}

		[Test]
		public void ValidRuleStringComplex() {
			RuleDeclarationParser parser = new RuleDeclarationParser(CompiledGrammarTest.LoadTestGrammar());
			Rule rule;
			Expect(parser.TryParse("<Value> ::= '(' <Expression> ')'", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleStringSimple() {
			RuleDeclarationParser parser = new RuleDeclarationParser(CompiledGrammarTest.LoadTestGrammar());
			Rule rule;
			Expect(parser.TryParse("<Value> ::= Float", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleStringEmpty() {
			// we have no empty rule in the test grammar, so we take the Rule grammar which has the empty <Handle>
			RuleDeclarationParser parser = new RuleDeclarationParser(RuleDeclarationParser.RuleGrammar);
			Rule rule;
			Expect(parser.TryParse("<Handle> ::=", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}
	}
}