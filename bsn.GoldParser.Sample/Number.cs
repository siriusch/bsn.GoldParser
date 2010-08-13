using System;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	[Terminal("Integer")]
	[Terminal("Float")]
	public class Number: Computable {
		private readonly double value;

		public Number(string value) {
			this.value = Double.Parse(value, NumberFormatInfo.InvariantInfo);
		}

		public override double GetValue() {
			return value;
		}
	}
}
