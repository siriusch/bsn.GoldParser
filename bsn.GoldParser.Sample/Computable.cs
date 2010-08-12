using System;
using System.Linq;

using bsn.GoldParser.Sample;
using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<Value> ::= '(' <Expression> ')'", "<Expression>", SemanticTokenType = typeof(CalculatorToken))]

namespace bsn.GoldParser.Sample {
	public abstract class Computable: CalculatorToken {
		public abstract double GetValue();
	}
}
