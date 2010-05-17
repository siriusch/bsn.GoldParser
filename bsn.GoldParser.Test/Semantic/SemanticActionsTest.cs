using System;
using System.Diagnostics;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SemanticActionsTest: AssertionHelper {
		internal SemanticActions<ExpressionToken> CreateActions() {
			SemanticActions<ExpressionToken> result = new SemanticActions<ExpressionToken>(CompiledGrammarTest.LoadTestGrammar());
			result.RegisterSemanticTokenCreator(result.FindRule("<Expression>", "<Expression>", "'+'", "<Mult Exp>"), new ExpressionFactory());
			return result;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new SemanticActions<ExpressionToken>(null);
		}

		[Test]
		public void ParseExpression() {
			SemanticActions<ExpressionToken> semanticActions = CreateActions();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, semanticActions.Grammar);
				TextToken token;
				SemanticProcessor<ExpressionToken> processor = new SemanticProcessor<ExpressionToken>(tokenizer, semanticActions);
				while (CompiledGrammar.CanContinueParsing(processor.Parse())) {}
				Debugger.Break();
			}
		}
	}
}