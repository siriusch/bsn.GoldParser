using System;
using System.Linq;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SemanticTypeActionsTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		internal SemanticTypeActions<TestToken> CreateSemanticActions() {
			return new SemanticTypeActions<TestToken>(grammar);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new SemanticTypeActions<TestToken>(null);
		}

		[Test]
		public void Create() {
			CreateSemanticActions();
		}

		[Test]
		public void Initialize() {
			CreateSemanticActions().Initialize();
		}
	}
}
