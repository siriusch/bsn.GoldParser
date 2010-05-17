using System;
using System.Globalization;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class NumberToken: SemanticToken {
		private double value;

		public NumberToken(TextToken token): base(token.Symbol, token.Position) {
			value = double.Parse(token.Text, NumberFormatInfo.InvariantInfo);
		}

		public double Value {
			get {
				return value;
			}
		}
	}
}