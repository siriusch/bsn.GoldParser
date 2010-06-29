using System;
using System.Linq;

namespace bsn.GoldParser.Semantic {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	public class TestSpecial: TestToken {
		private readonly string text;

		public TestSpecial(string text) {
			this.text = text;
		}

		public string Text {
			get {
				return text;
			}
		}
	}
}
