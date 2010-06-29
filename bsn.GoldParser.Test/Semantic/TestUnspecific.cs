using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	public class TestUnspecific: TestToken {
		private readonly string text;

		public TestUnspecific(string text) {
			this.text = text;
		}

		public string Text {
			get {
				return text;
			}
		}
	}
}
