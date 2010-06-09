namespace bsn.GoldParser.Semantic {
	[Terminal("*")]
	public class TestMultiply: TestOperation {
		public TestMultiply() {}

		public override double Compute(double left, double right) {
			return left*right;
		}
	}
}