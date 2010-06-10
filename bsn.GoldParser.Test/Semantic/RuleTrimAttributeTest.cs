using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleTrimAttributeTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new RuleTrimAttribute(null, 0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithEmptyString() {
			new RuleTrimAttribute(string.Empty, 0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ConstructWithNegativeIndex() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", -1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ConstructWithOverflowIndex() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 2);
		}

		[Test]
		public void BindToGrammar() {
			RuleTrimAttribute attribute = new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 1);
			Expect(attribute.Bind(CompiledGrammarTest.LoadTestGrammar()), Not.Null);
			Expect(attribute.TrimSymbolIndex, EqualTo(1));
		}
	}
}