using System;

namespace bsn.GoldParser.Semantic {
	[Terminal("NULL")]
	public class TestEmpty: TestToken {
		[Rule("<Empty> ::=")]
		[Rule("<Empty> ::= NULL", AllowTruncation = true)]
		public TestEmpty() {
		}
	}
}
