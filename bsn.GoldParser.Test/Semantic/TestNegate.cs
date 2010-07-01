using System;

namespace bsn.GoldParser.Semantic {
	public class TestNegate: TestValue {
		private readonly TestValue value;

		[Rule("<Negate Exp> ::= '-' <Value>", ParameterMapping = new [] {1})]
		public TestNegate(TestValue value) {
			this.value = value;
		}

		public override double Compute() {
			return -value.Compute();
		}
	}
}
