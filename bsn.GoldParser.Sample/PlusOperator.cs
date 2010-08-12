using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	[Terminal("+")]
	public class PlusOperator: Operator {
		public override double Calculate(double left, double right) {
			return left+right;
		}
	}
}
