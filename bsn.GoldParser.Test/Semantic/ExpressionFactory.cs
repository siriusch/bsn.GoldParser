using System;
using System.Collections.Generic;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	internal class ExpressionFactory: SemanticTokenFactory<ExpressionToken> {
		public override ICollection<Type> InputTypes {
			get {
				return new[] {typeof(ExpressionToken), typeof(OperatorToken), typeof(ExpressionToken)};
			}
		}

		protected override ExpressionToken Create(Rule rule, Token[] tokens) {
			return new ExpressionToken(rule.RuleSymbol, (ExpressionToken)tokens[0], (OperatorToken)tokens[1], (ExpressionToken)tokens[2]);
		}
	}
}