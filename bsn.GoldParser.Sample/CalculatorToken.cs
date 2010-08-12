using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(")]
	[Terminal(")")]
	public class CalculatorToken: SemanticToken {}
}
