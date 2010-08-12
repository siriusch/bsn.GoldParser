using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	public class Negate: Computable {
		private readonly Computable computable;

		[Rule("<Negate Exp>  ::= '-' <Value>", ConstructorParameterMapping = new[] {1})]
		public Negate(Computable computable) {
			this.computable = computable;
		}

		public override double GetValue() {
			return -computable.GetValue();
		}
	}
}
