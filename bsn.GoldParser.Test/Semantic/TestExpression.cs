using System;

namespace bsn.GoldParser.Semantic {
	public class TestExpression<T>: TestValue where T: TestOperation {
		private readonly TestValue left;
		private readonly TestOperation operation;
		private readonly TestValue right;

		[Rule("<Expression> ::= <Expression> '+' <Mult Exp>", typeof(TestAdd))]
		[Rule("<Expression> ::= <Expression> '-' <Mult Exp>", typeof(TestSubtract))]
		[Rule("<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>", typeof(TestMultiply))]
		[Rule("<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>", typeof(TestDivide))]
		public TestExpression(TestValue left, T operation, TestValue right) {
			this.left = left;
			this.operation = operation;
			this.right = right;
		}

		public override double Compute() {
			return operation.Compute(left.Compute(), right.Compute());
		}
	}
}
