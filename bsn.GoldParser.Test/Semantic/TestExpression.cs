namespace bsn.GoldParser.Semantic {
	public class TestExpression: TestToken {
		[Rule("<Expression> ::= <Expression> '+' <Mult Exp>")]
		[Rule("<Expression> ::= <Expression> '-' <Mult Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
		[Rule("<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
		public TestExpression(TestExpression left, TestOperation operation, TestExpression right) {
		}
	}
}