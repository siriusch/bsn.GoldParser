using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	[Terminal("NULL")]
	public class TestEmpty: TestToken {
		[Rule("<Empty> ::= NULL", AllowTruncationForConstructor = true)]
		public TestEmpty() {}

		[Rule("<Empty> ::=", ConstructorParameterMapping = new[] {-1})]
		public TestEmpty(CompiledGrammar dummy) {
			if (dummy != null) {
				throw new InvalidOperationException("Expected a null dummy");
			}
		}
	}
}