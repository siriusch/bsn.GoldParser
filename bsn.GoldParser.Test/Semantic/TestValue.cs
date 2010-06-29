using System;

namespace bsn.GoldParser.Semantic {
	public abstract class TestValue: TestToken {
		public abstract double Compute();
	}
}
