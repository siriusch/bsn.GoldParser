using System;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	[Terminal("-")]
	public class MinusOperator: Operator {
		public override double Calculate(double left, double right) {
			return left-right;
		}
	}
}