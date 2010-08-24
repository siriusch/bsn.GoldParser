using System;

namespace bsn.GoldParser.Sample {
	public abstract class Operator: CalculatorToken {
		public abstract double Calculate(double left, double right);
	}
}