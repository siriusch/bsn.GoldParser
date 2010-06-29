using System;

namespace bsn.GoldParser.Semantic {
	public class TestNegate: TestValue {
		[Rule("<Negate Exp> ::= '-' <Value>")]
		public TestNegate(TestSubtract negate, TestValue value) {}
	}
}
