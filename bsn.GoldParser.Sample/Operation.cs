using System;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	public class Operation: Computable {
		private readonly Computable left;
		private readonly Operator op;
		private readonly Computable right;

		[Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
		[Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
		[Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
		[Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
		public Operation(Computable left, Operator op, Computable right) {
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override double GetValue() {
			return op.Calculate(left.GetValue(), right.GetValue());
		}
	}
}
