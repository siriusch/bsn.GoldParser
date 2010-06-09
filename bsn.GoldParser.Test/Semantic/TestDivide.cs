namespace bsn.GoldParser.Semantic {
	[Terminal("/")]
	public class TestDivide: TestOperation {
		public TestDivide() {}

		public override double Compute(double left, double right) {
			return left/right;
		}
	}
}