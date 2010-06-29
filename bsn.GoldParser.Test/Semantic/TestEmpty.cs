using System;

namespace bsn.GoldParser.Semantic {
	[Terminal("NULL")]
	public class TestEmpty: TestToken {
		[Rule("<Empty> ::=")]
		public TestEmpty() {}
	}
}
