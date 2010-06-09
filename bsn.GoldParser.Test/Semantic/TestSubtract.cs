namespace bsn.GoldParser.Semantic {
	[Terminal("-")]
	public class TestSubtract: TestOperation {
		public TestSubtract() {}

		public override double Compute(double left, double right) {
			return left-right;
		}
	}
}