using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class ExpressionToken: SemanticToken {
		private readonly ExpressionToken left;
		private readonly OperatorToken op;
		private readonly ExpressionToken right;

		internal ExpressionToken(Symbol symbol, ExpressionToken left, OperatorToken op, ExpressionToken right): base(symbol, left.Position) {
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public ExpressionToken Left {
			get {
				return left;
			}
		}

		public OperatorToken Op {
			get {
				return op;
			}
		}

		public ExpressionToken Right {
			get {
				return right;
			}
		}
	}
}