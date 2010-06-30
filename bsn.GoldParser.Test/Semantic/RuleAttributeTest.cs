using System;
using System.Linq;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleAttributeTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void BindToGrammar() {
			Expect(new RuleAttribute("<Negate Exp> ::= '-' <Value>").Bind(grammar), Not.Null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithEmptyString() {
			new RuleAttribute(string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ConstructWithInvalidString() {
			new RuleAttribute("<Negate Exp> = '-' <Value>");
		}

		[Test]
		public void ConstructWithGenericArgument() {
			new RuleAttribute("<Negate Exp> ::= '-' <Value>", typeof(TestValue));
		}

		[Test]
		public void ConstructWithGenericArgument2() {
			new RuleAttribute("<Negate Exp> ::= '-' <Value>", typeof(TestValue), typeof(TestValue));
		}

		[Test]
		public void ConstructWithString() {
			new RuleAttribute("<Negate Exp> ::= '-' <Value>");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new RuleAttribute(null);
		}
	}
}
