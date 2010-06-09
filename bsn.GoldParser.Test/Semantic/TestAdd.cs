namespace bsn.GoldParser.Semantic {
	[Terminal("+")]
	public class TestAdd: TestOperation {
		public TestAdd() {}

		public override double Compute(double left, double right) {
			return left+right;
		}
	}
}