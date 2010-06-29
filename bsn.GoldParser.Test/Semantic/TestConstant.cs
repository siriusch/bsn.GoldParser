using System;
using System.Xml;

namespace bsn.GoldParser.Semantic {
	[Terminal("Integer")]
	[Terminal("Float")]
	public class TestConstant: TestValue {
		private readonly double constant;

		public TestConstant(string constant) {
			this.constant = XmlConvert.ToDouble(constant);
		}

		public override double Compute() {
			return constant;
		}
	}
}
