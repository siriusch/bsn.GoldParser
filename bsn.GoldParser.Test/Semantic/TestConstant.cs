using System;
using System.Globalization;
using System.Xml;

namespace bsn.GoldParser.Semantic {
	[Terminal("Integer", typeof(int))]
	[Terminal("Float", typeof(double))]
	public class TestConstant<T>: TestValue where T: struct, IConvertible {
		private readonly T constant;

		public TestConstant(string constant) {
			this.constant = (T)Convert.ChangeType(constant, typeof(T), NumberFormatInfo.InvariantInfo);
		}

		public override double Compute() {
			return constant.ToDouble(NumberFormatInfo.InvariantInfo);
		}
	}
}
