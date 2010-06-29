using System;

namespace bsn.GoldParser.Semantic {
	public class TestExpression: TestValue {
		private readonly TestValue left;
		private readonly TestOperation operation;
		private readonly TestValue right;

		[Rule("<Expression> ::= <Expression> '+' <Mult Exp>")]
		[Rule("<Expression> ::= <Expression> '-' <Mult Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
		public TestExpression(TestValue left, TestOperation operation, TestValue right) {
			this.left = left;
			this.operation = operation;
			this.right = right;
		}

		public override double Compute() {
			return operation.Compute(left.Compute(), right.Compute());
		}
	}
}
