using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SemanticTypeActionsTest: AssertionHelper {
		[Test]
		public void Create() {
			CreateSemanticActions();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new SemanticTypeActions<TestToken>(null);
		}

		internal static SemanticTypeActions<TestToken> CreateSemanticActions() {
			return new SemanticTypeActions<TestToken>(CompiledGrammarTest.LoadTestGrammar());
		}
	}
}
