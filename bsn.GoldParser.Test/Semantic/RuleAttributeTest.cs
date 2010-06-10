using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic
{
	[TestFixture]
	public class RuleAttributeTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new RuleAttribute(null);
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
		public void ConstructWithString() {
			new RuleAttribute("<Negate Exp> ::= '-' <Value>");
		}

		[Test]
		public void BindToGrammar() {
			Expect(new RuleAttribute("<Negate Exp> ::= '-' <Value>").Bind(CompiledGrammarTest.LoadTestGrammar()), Not.Null);
		}
	}
}
