using System;

namespace bsn.GoldParser.Semantic {
	public class TestExpression: TestValue {
		[Rule("<Expression> ::= <Expression> '+' <Mult Exp>")]
		[Rule("<Expression> ::= <Expression> '-' <Mult Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
		public TestExpression(TestValue left, TestOperation operation, TestValue right) {}
	}
}
