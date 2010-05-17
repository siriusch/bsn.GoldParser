using System;
using System.Collections.Generic;
using System.Text;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class RuleAttribute: Attribute {
		public RuleAttribute(string rule) {
			
		}
	}
}
