using System;
using System.Linq;

namespace bsn.GoldParser.Semantic {
	public abstract class TestOperation: TestToken {
		protected TestOperation() {}

		public abstract double Compute(double left, double right);
	}
}
